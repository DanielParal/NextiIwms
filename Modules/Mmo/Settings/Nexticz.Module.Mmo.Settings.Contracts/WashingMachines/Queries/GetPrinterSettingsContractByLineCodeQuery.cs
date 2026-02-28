using MediatR;
using ErrorOr;

namespace Nexticz.Module.Mmo.Settings.Contracts.WashingMachines.Queries;

public record GetPrinterSettingsContractByLineCodeQuery(string LineCode) : IRequest<ErrorOr<PrinterSettingsContract>>;