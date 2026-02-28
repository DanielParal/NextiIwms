using Nexticz.Module.Mmo.SharedKernel;
using Nexticz.Module.Mmo.SharedKernel.DomainCore;

namespace Nexticz.Module.Mmo.Washing.Domain.SpecialInformationEntity;

public class SpecialInformationConfirmation : Entity
{
    public string WorkerName { get; private set; }
    public DateTimeOffset DateConfirmed { get; private set; }
    
    // We need private constructor due to Marten deserialization
    private SpecialInformationConfirmation() {}

    public SpecialInformationConfirmation(
        string workerName,
        DateTimeOffset dateConfirmed,
        Guid? id = null)
        : base(id ?? Guid.NewGuid())
    {
        WorkerName = workerName;
        DateConfirmed = dateConfirmed;
    }
}