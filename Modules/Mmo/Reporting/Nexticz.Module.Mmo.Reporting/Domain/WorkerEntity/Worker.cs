using Nexticz.Module.Mmo.SharedKernel;
using Nexticz.Module.Mmo.SharedKernel.DomainCore;

namespace Nexticz.Module.Mmo.Reporting.Domain.WorkerEntity;

public class Worker : Entity
{
    public string Name { get; private set; }
    
    // We need private constructor due to Marten deserialization
    private Worker() {}
    
    public Worker(
        string name,
        Guid? id = null) : base(id ?? Guid.NewGuid())
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("Name cannot be null or empty", nameof(name));
        
        Name = name;
    }
}