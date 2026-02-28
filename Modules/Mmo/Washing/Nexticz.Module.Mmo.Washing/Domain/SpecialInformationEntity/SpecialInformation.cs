using Nexticz.Module.Mmo.SharedKernel;
using Nexticz.Module.Mmo.SharedKernel.DomainCore;

namespace Nexticz.Module.Mmo.Washing.Domain.SpecialInformationEntity;

public class SpecialInformation : Entity
{
    public string Title { get; private set; }
    public string Description { get; private set; }
    public bool HasFile { get; private set; }
    private readonly List<SpecialInformationConfirmation> _confirmations = [];
    public IReadOnlyCollection<SpecialInformationConfirmation> Confirmations => _confirmations.AsReadOnly();
    
    // We need private constructor due to Marten deserialization
    private SpecialInformation() {}

    public SpecialInformation(
        string title,
        string description,
        bool hasFile,
        SpecialInformationConfirmation[] confirmations,
        Guid? id = null)
        : base(id ?? Guid.NewGuid())
    {
        Title = title;
        Description = description;
        HasFile = hasFile;
        _confirmations = confirmations.ToList();
    }

    public void AddConfirmation(string workerName, DateTimeOffset dateConfirmed)
    {
        var confirmation = new SpecialInformationConfirmation(workerName, dateConfirmed);
        _confirmations.Add(confirmation);
    }
}