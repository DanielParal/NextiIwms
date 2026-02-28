using Nexticz.Module.Mmo.SharedKernel;
using Nexticz.Module.Mmo.SharedKernel.DomainCore;

namespace Nexticz.Module.Mmo.Reporting.Domain.SpecialInformationEntity;

public class SpecialInformation : Entity
{
    public string Title { get; private set; }
    public string Description { get; private set; }
    public bool HasFile { get; private set; }
    public string WorkerName { get; private set; }
    public DateTimeOffset DateConfirmed { get; private set; }
    
    // We need private constructor due to Marten deserialization
    private SpecialInformation() {}

    public SpecialInformation(
        string title,
        string description,
        bool hasFile,
        string workerName,
        DateTimeOffset dateConfirmed,
        Guid? id = null)
        : base(id ?? Guid.NewGuid())
    {
        Title = title;
        Description = description;
        HasFile = hasFile;
        WorkerName = workerName;
        DateConfirmed = dateConfirmed;
    }
}