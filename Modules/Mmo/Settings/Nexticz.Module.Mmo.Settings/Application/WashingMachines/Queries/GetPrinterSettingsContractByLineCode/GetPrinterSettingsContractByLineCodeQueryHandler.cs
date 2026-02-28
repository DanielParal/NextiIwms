using ErrorOr;
using MediatR;
using Nexticz.Module.Mmo.Settings.Contracts.WashingMachines;
using Nexticz.Module.Mmo.Settings.Contracts.WashingMachines.Queries;
using Nexticz.Module.Mmo.Settings.Application.Interfaces;

namespace Nexticz.Module.Mmo.Settings.Application.WashingMachines.Queries.GetPrinterSettingsContractByLineCode;

internal class GetPrinterSettingsContractByLineCodeQueryHandler(IWashingMachineReadOnlyRepository washingMachineReadOnlyRepository)
    : IRequestHandler<GetPrinterSettingsContractByLineCodeQuery, ErrorOr<PrinterSettingsContract>>
{
    public async Task<ErrorOr<PrinterSettingsContract>> Handle(GetPrinterSettingsContractByLineCodeQuery request, CancellationToken cancellationToken)
    {
        var washingMachine = await washingMachineReadOnlyRepository.GetByLineCodeAsync(request.LineCode, cancellationToken);
        if (washingMachine is null)
            return WashingMachineErrors.CodeDoesNotExist;
        
        var printerSettings = washingMachine.WashingMachineLines
            .First(x => string.Equals(x.Code, request.LineCode, StringComparison.InvariantCultureIgnoreCase))
            .PrinterSettings;
        
        return PrinterSettingsContractFactory.Create(printerSettings);
    }
}