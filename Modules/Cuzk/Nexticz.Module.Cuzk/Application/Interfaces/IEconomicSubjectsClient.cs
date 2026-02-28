using Nexticz.Module.Cuzk.Application.EconomicSubjects.Models;

namespace Nexticz.Module.Cuzk.Application.Interfaces;

public interface IEconomicSubjectsClient
{
    Task<EconomicSubject?> GetByIcoAsync(string ico, CancellationToken ct);

}