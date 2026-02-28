using System.Net.Http.Json;
using MediatR;
using Microsoft.Extensions.Logging;
using Nexticz.Module.Cuzk.Application.EconomicSubjects.Models;
using Nexticz.Module.Cuzk.Contracts.EconomicSubjects;
using Nexticz.Module.Cuzk.Application.AddressLocations.Queries.GetAddressLocationByAdmCode;
using Nexticz.Module.Cuzk.Application.Interfaces;
using Nexticz.Module.Cuzk.Domain.AddressLocationAggregate;

namespace Nexticz.Module.Cuzk.Application.EconomicSubjects.Queries.GetEconomicSubjectByIco;

public class GetEconomicSubjectByIcoQueryHandler(
    ILogger<GetEconomicSubjectByIcoQueryHandler> logger,
    ISender sender,
    IEconomicSubjectsClient economicSubjectsClient) : IRequestHandler<GetEconomicSubjectByIcoQuery, EconomicSubjectResponse?>
{
    public async Task<EconomicSubjectResponse?> Handle(GetEconomicSubjectByIcoQuery query, CancellationToken cancellationToken)
    {
        logger.LogInformation("[Cuzk] [Start] [GetEconomicSubjectByIcoQueryHandler] - ico: {ico}", query.Ico);

        var economicSubject = await economicSubjectsClient.GetByIcoAsync(query.Ico, cancellationToken);

        if (economicSubject is null)
        {
            logger.LogInformation("[Cuzk] [End] [GetEconomicSubjectByIcoQueryHandler] - ico: {ico}. Economic subject is null", query.Ico);
            return null;
        }
        
        var admCode = economicSubject.Sidlo?.KodAdresnihoMista.ToString();
        
        var address = await GetAddressLocationAsync(admCode, cancellationToken);
        
        logger.LogInformation("[Cuzk] [End] [GetEconomicSubjectByIcoQueryHandler] - ico: {ico}", query.Ico);

        return new EconomicSubjectResponse
        {
            Name = economicSubject.ObchodniJmeno,
            Ico = economicSubject.Ico,
            Dic = economicSubject.Dic,
            Street = address?.StreetName,
            ZipCode = address?.ZipCode,
            Municipality = address?.MunicipalityName,
            District = address?.DistrictName,
            Country = address?.CountryName,
        };
    }

    private async Task<AddressLocation?> GetAddressLocationAsync(string? admCode, CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(admCode))
        {
            logger.LogInformation("[Cuzk] [GetEconomicSubjectByIcoQueryHandler] - adm code is null or whitespace. Cannot return address location");
            return null;
        }
        
        var addressResult = await sender.Send(new GetAddressLocationByAdmCodeQuery(admCode), cancellationToken);

        if (addressResult.IsError)
        {
            logger.LogInformation("[Cuzk] [GetEconomicSubjectByIcoQueryHandler] - cannot return address location. ErrorCode: {ErrorCode}, ErrorDescription: {ErrorDescription}",
                addressResult.FirstError.Code, addressResult.FirstError.Description);
            return null;
        }
        
        return addressResult.Value;
    }
}