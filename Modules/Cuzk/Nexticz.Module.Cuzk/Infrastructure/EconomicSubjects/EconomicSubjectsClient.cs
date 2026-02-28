using System.Net.Http.Json;
using Microsoft.Extensions.Logging;
using Nexticz.Module.Cuzk.Application.EconomicSubjects.Models;
using Nexticz.Module.Cuzk.Application.Interfaces;

namespace Nexticz.Module.Cuzk.Infrastructure.EconomicSubjects;

internal class EconomicSubjectsClient(
    ILogger<EconomicSubjectsClient> logger, 
    IHttpClientFactory httpClientFactory) : IEconomicSubjectsClient
{
    public const string HttpClientGovEconomicSubjects = "GovEconomicSubjectsClient";
    
    
    public async Task<EconomicSubject?> GetByIcoAsync(string ico, CancellationToken ct)
    {
        logger.LogInformation("[Cuzk] [Start] [EconomicSubjectsClient] - ico: {ico}", ico);

        try
        {
            var client = httpClientFactory.CreateClient(HttpClientGovEconomicSubjects);
            var response = await client.GetAsync($"ekonomicke-subjekty/{ico}", ct);
            if (!response.IsSuccessStatusCode)
            {
                var responseContent = await response.Content.ReadAsStringAsync(ct);
                logger.LogInformation("[Cuzk] [End] [EconomicSubjectsClient] - ico: {ico}. Response status code: {ResponseStatusCode}, responseContent: {ResponseContent}", 
                    ico, response.StatusCode, responseContent);
                return null;
            }
        
            var economicSubject = await response.Content.ReadFromJsonAsync<EconomicSubject>(ct);

            if (economicSubject is null)
            {
                logger.LogInformation("[Cuzk] [End] [EconomicSubjectsClient] - ico: {ico}. Economic subject is null", ico);
                return null;
            }
        
            logger.LogInformation("[Cuzk] [End] [EconomicSubjectsClient] - ico: {ico}, obchodniJmeno: {obchodniJmeno}", 
                economicSubject.Ico, economicSubject.ObchodniJmeno);
        
            return economicSubject;
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "[Cuzk] [Error] [EconomicSubjectsClient] - ico: {ico}, ErrorMessage: {ErrorMessage}", ico, ex.Message);
            return null;
        }
        
    }

}