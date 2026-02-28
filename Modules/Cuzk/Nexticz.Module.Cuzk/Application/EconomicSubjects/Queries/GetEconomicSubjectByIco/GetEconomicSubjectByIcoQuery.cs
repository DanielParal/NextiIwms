using MediatR;
using Nexticz.Module.Cuzk.Contracts.EconomicSubjects;

namespace Nexticz.Module.Cuzk.Application.EconomicSubjects.Queries.GetEconomicSubjectByIco;

public record GetEconomicSubjectByIcoQuery(string Ico) : IRequest<EconomicSubjectResponse?>;