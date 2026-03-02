# Lang Module Refactoring Instructions

## Goals

1. **Replace EF Core + SQL Server with Marten + PostgreSQL** — follow the Portal module pattern (event sourcing, projections, document store)
2. Two complete CRUD systems: **Language** and **Translation** (no separate Section entity — `SectionSlug` is a plain string on Translation)
3. **Remove Google Translate API** dependency entirely
4. **Reflection-based seed** that scans all assemblies for `IErrorComponentSlugProvider` and `ITranslatable` implementations
5. **MassTransit event-driven JSON regeneration** — when a translation within a section is created or updated, publish a MassTransit event; a consumer regenerates that section's JSON file
6. **Feature flag** `LangJsonGenerationEnabled` to control whether JSON file generation is active
7. **Serve JSON files** to the FE via static file middleware

### Why no Section entity?

The section (`SectionSlug`) is just a categorization label — e.g. `"auth"`, `"lang"`, `"shared"`, `"mmo-settings"`. It has no meaningful metadata beyond the slug itself. The FE determines which section a translation belongs to when creating it. Storing it as a plain `string` on `Translation` avoids unnecessary CRUD, FK constraints, and complexity. If the FE needs a list of available sections, a `SELECT DISTINCT SectionSlug FROM Translations` query provides it without a dedicated table.

---

## Step 1: Replace EF Core with Marten (Portal Pattern)

The Lang module currently uses EF Core + SQL Server. Migrate to Marten (PostgreSQL event store + document projections), matching the existing Portal module infrastructure.

### New shared infrastructure wiring

Follow Portal's pattern exactly:

**New file**: `Nexticz.Module.Lang.Infrastructure/ILangDocumentStore.cs`

```csharp
using Marten;

namespace Nexticz.Module.Lang.Infrastructure;

public interface ILangDocumentStore : IDocumentStore;
```

**New file**: `Nexticz.Module.Lang.Application/Interfaces/ILangDocumentSessionProvider.cs`

```csharp
using Nexticz.Lib.Shared.DataAccess.Marten;

namespace Nexticz.Module.Lang.Application.Interfaces;

internal interface ILangDocumentSessionProvider : IMartenDocumentSessionProvider;
```

**New file**: `Nexticz.Module.Lang.Application/Interfaces/ILangUnitOfWork.cs`

```csharp
using Nexticz.Lib.Shared.DataAccess.Marten;

namespace Nexticz.Module.Lang.Application.Interfaces;

internal interface ILangUnitOfWork : IMartenUnitOfWork;
```

**New file**: `Nexticz.Module.Lang.Application/Interfaces/ILangReadOnlyEventStoreRepository.cs`

```csharp
using Nexticz.Lib.Shared.DataAccess.Marten;

namespace Nexticz.Module.Lang.Application.Interfaces;

internal interface ILangReadOnlyEventStoreRepository : IMartenReadOnlyEventStoreRepository;
```

**New file**: `Nexticz.Module.Lang.Application/ILangCommand.cs`

```csharp
using MediatR;

namespace Nexticz.Module.Lang.Application;

internal interface ILangCommand<out TResponse> : IRequest<TResponse>;
```

### Infrastructure implementations (same as Portal)

**New file**: `Nexticz.Module.Lang.Infrastructure/BaseRepositories/LangDocumentSessionProvider.cs`

```csharp
using Nexticz.Lib.Shared.DataAccess.Marten;
using Nexticz.Module.Lang.Application.Interfaces;

namespace Nexticz.Module.Lang.Infrastructure.BaseRepositories;

internal class LangDocumentSessionProvider(ILangDocumentStore store)
    : MartenDocumentSessionProvider(store), ILangDocumentSessionProvider;
```

**New file**: `Nexticz.Module.Lang.Infrastructure/BaseRepositories/LangUnitOfWork.cs`

```csharp
using Nexticz.Lib.Shared.DataAccess.Marten;
using Nexticz.Lib.Shared.UserProviders;
using Nexticz.Module.Lang.Application.Interfaces;

namespace Nexticz.Module.Lang.Infrastructure.BaseRepositories;

internal class LangUnitOfWork(ILangDocumentSessionProvider documentSessionProvider, ICurrentUserProvider currentUserProvider)
    : MartenUnitOfWork(documentSessionProvider, currentUserProvider), ILangUnitOfWork;
```

**New file**: `Nexticz.Module.Lang.Infrastructure/BaseRepositories/LangReadOnlyEventStoreRepository.cs`

```csharp
using Nexticz.Lib.Shared.DataAccess.Marten;
using Nexticz.Module.Lang.Application.Interfaces;

namespace Nexticz.Module.Lang.Infrastructure.BaseRepositories;

internal class LangReadOnlyEventStoreRepository(ILangDocumentSessionProvider documentSessionProvider)
    : MartenReadOnlyEventStoreRepository(documentSessionProvider), ILangReadOnlyEventStoreRepository;
```

### PostCommandBehavior (auto-save after commands)

**New file**: `Nexticz.Module.Lang.Application/PipelineBehaviors/LangPostCommandBehavior.cs`

```csharp
using MediatR;
using Nexticz.Lib.Shared.MediatR;
using Nexticz.Module.Lang.Application.Interfaces;
using Nexticz.Module.Lang.Application.NotificationCollectors;

namespace Nexticz.Module.Lang.Application.PipelineBehaviors;

internal class LangPostCommandBehavior<TRequest, TResponse>(
    ILangUnitOfWork unitOfWork,
    INotificationCollector notificationCollector)
    : MediatRPostCommandBehavior<TRequest, TResponse, ILangCommand<TResponse>>(unitOfWork, notificationCollector)
    where TRequest : IRequest<TResponse>;
```

### DependencyInjection (Infrastructure)

**Rewrite**: `Nexticz.Module.Lang.Infrastructure/DependencyInjection.cs`

```csharp
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Nexticz.Lib.Shared.DataAccess.Marten.Configuration;
using Nexticz.Module.Lang.Application.Interfaces;
using Nexticz.Module.Lang.Infrastructure.BaseRepositories;

namespace Nexticz.Module.Lang.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddLangInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        var postgresConnectionString = configuration.GetConnectionString("Lang") ??
            throw new InvalidOperationException("Lang PostgresDb connection string not found.");

        services.AddMarten<ILangDocumentStore>("lang", postgresConnectionString, configuration);

        services.AddScoped<ILangUnitOfWork, LangUnitOfWork>();
        services.AddScoped<ILangDocumentSessionProvider, LangDocumentSessionProvider>();
        services.AddScoped<ILangReadOnlyEventStoreRepository, LangReadOnlyEventStoreRepository>();

        return services;
    }
}
```

### What to delete (EF Core artifacts)

| File | Reason |
|------|--------|
| `Infrastructure/Common/Persistence/DataContext.cs` | Replaced by Marten document store |
| `Infrastructure/Common/Persistence/UnitOfWork.cs` | Replaced by `LangUnitOfWork` |
| `Infrastructure/Translations/Persistance/TranslationConfigurations.cs` | No EF entity configs with Marten |
| `Infrastructure/Languages/Persistance/LanguageConfigurations.cs` | No EF entity configs with Marten |
| `Infrastructure/Translations/Persistance/TranslationsRepository.cs` | Replaced by `ILangReadOnlyEventStoreRepository` |
| `Infrastructure/Languages/Persistance/LanguagesRepository.cs` | Replaced by `ILangReadOnlyEventStoreRepository` |
| `Infrastructure/Migrations/*` | No EF migrations |
| `Application/Common/Interfaces/IUnitOfWork.cs` | Replaced by `ILangUnitOfWork` |
| `Application/Common/Interfaces/ITranslationsRepository.cs` | Replaced by generic Marten repo |
| `Application/Common/Interfaces/ILanguagesRepository.cs` | Replaced by generic Marten repo |
| `Application/Common/Helpers/StringHelper.cs` | DB schema/migration constants no longer needed |

### Add PostgreSQL connection string

In `appsettings.*.json`:

```json
"ConnectionStrings": {
    "Lang": "host=postgres.local.dev;port=5432;database=lang-dev;username=admin;password=Password1!;"
}
```

---

## Step 2: Domain — Language Aggregate (Event-Sourced)

### Aggregate

**Rewrite**: `Nexticz.Module.Lang.Domain/Languages/Language.cs`

```csharp
using ErrorOr;
using Nexticz.Module.Lang.Domain.Languages.Events;

namespace Nexticz.Module.Lang.Domain.Languages;

public class Language : AggregateRoot
{
    public string Name { get; private set; }
    public string Shortcut { get; private set; }    // "Cs", "En", "De", etc.
    public bool IsActive { get; private set; }
    public DateTimeOffset CreatedAt { get; private set; }

    private Language() { }

    private Language(string name, string shortcut, bool isActive, DateTimeOffset createdAt, Guid? id = null)
        : base(id ?? Guid.NewGuid())
    {
        Name = name;
        Shortcut = shortcut;
        IsActive = isActive;
        CreatedAt = createdAt;
    }

    public static ErrorOr<Language> CreateFrom(string name, string shortcut, bool isActive, DateTimeOffset createdAt)
    {
        if (string.IsNullOrWhiteSpace(name))
            return LanguageErrors.CreateLanguageError;

        return new Language(name, shortcut, isActive, createdAt);
    }

    public ErrorOr<Success> Update(bool isActive)
    {
        IsActive = isActive;
        return Result.Success;
    }

    // Marten event Apply methods
    public void Apply(LanguageCreatedEvent e)
    {
        Name = e.Name;
        Shortcut = e.Shortcut;
        IsActive = e.IsActive;
        CreatedAt = e.CreatedAt;
    }

    public void Apply(LanguageUpdatedEvent e)
    {
        IsActive = e.IsActive;
    }
}
```

### Events

**New file**: `Domain/Languages/Events/LanguageCreatedEvent.cs`

```csharp
using Nexticz.Lib.Shared.DataAccess.Marten.Configuration;

namespace Nexticz.Module.Lang.Domain.Languages.Events;

public record LanguageCreatedEvent(
    Guid Id, string Name, string Shortcut, bool IsActive, DateTimeOffset CreatedAt) : IMartenEvent;
```

**New file**: `Domain/Languages/Events/LanguageUpdatedEvent.cs`

```csharp
using Nexticz.Lib.Shared.DataAccess.Marten.Configuration;

namespace Nexticz.Module.Lang.Domain.Languages.Events;

public record LanguageUpdatedEvent(
    Guid Id, bool IsActive, DateTimeOffset UpdatedAt) : IMartenEvent;
```

**New file**: `Domain/Languages/Events/LanguageDeletedEvent.cs`

```csharp
using Nexticz.Lib.Shared.DataAccess.Marten.Configuration;

namespace Nexticz.Module.Lang.Domain.Languages.Events;

public record LanguageDeletedEvent(Guid Id, DateTimeOffset DeletedAt) : IMartenEvent;
```

### Projection

**New file**: `Infrastructure/Languages/LanguageProjection.cs`

```csharp
using JasperFx.Events;
using Marten.Events.Aggregation;
using Nexticz.Module.Lang.Domain.Languages;
using Nexticz.Module.Lang.Domain.Languages.Events;

namespace Nexticz.Module.Lang.Infrastructure.Languages;

public class LanguageProjection : SingleStreamProjection<Language, Guid>
{
    public LanguageProjection()
    {
        DeleteEvent<LanguageDeletedEvent>();
    }

    public void Apply(IEvent<LanguageCreatedEvent> @event, Language language)
        => language.Apply(@event.Data);

    public void Apply(IEvent<LanguageUpdatedEvent> @event, Language language)
        => language.Apply(@event.Data);
}
```

### Configurator

**New file**: `Infrastructure/Languages/LanguageConfigurator.cs`

```csharp
using JasperFx.Events.Projections;
using Marten;
using Nexticz.Lib.Shared.DataAccess.Marten.Configuration;

namespace Nexticz.Module.Lang.Infrastructure.Languages;

internal class LanguageConfigurator : IMartenConfigurator
{
    public void Configure(StoreOptions options)
    {
        options.Projections.Add<LanguageProjection>(ProjectionLifecycle.Inline);
        options.Schema.For<Domain.Languages.Language>().UniqueIndex(x => x.Shortcut);
        options.Schema.For<Domain.Languages.Language>()
            .DocumentAlias(MartenConfigurationOrchestrator.PluralizeTableName<Domain.Languages.Language>());
    }
}
```

### AggregateRoot base

**New file**: `Domain/AggregateRoot.cs`

```csharp
namespace Nexticz.Module.Lang.Domain;

public class AggregateRoot : Lib.Shared.DomainCore.AggregateRoot
{
    protected AggregateRoot(Guid id) : base(id) { }
    protected AggregateRoot() { }
}
```

---

## Step 3: Domain — Translation Aggregate (Event-Sourced)

### Aggregate

**Rewrite**: `Domain/Translations/Translation.cs`

```csharp
using ErrorOr;
using Nexticz.Module.Lang.Domain.Translations.Events;

namespace Nexticz.Module.Lang.Domain.Translations;

public class Translation : AggregateRoot
{
    public string Module { get; private set; }
    public string Feature { get; private set; }
    public string Component { get; private set; }
    public string Name { get; private set; }
    public string Slug { get; private set; }
    public string Value { get; private set; }
    public string LanguageShortcut { get; private set; }
    public string SectionSlug { get; private set; }       // plain string — e.g. "auth", "lang", "shared"
    public DateTimeOffset CreatedAt { get; private set; }
    public DateTimeOffset? UpdatedAt { get; private set; }

    private Translation() { }

    private Translation(
        string module, string feature, string component, string name,
        string slug, string value, string languageShortcut,
        string sectionSlug,
        DateTimeOffset createdAt, Guid? id = null) : base(id ?? Guid.NewGuid())
    {
        Module = module; Feature = feature; Component = component; Name = name;
        Slug = slug; Value = value; LanguageShortcut = languageShortcut;
        SectionSlug = sectionSlug;
        CreatedAt = createdAt;
    }

    public static ErrorOr<Translation> CreateFrom(
        string module, string feature, string component, string name,
        string slug, string value, string languageShortcut,
        string sectionSlug, DateTimeOffset createdAt)
    {
        if (string.IsNullOrWhiteSpace(slug))
            return TranslationErrors.CreateTranslationError;

        if (string.IsNullOrWhiteSpace(sectionSlug))
            return TranslationErrors.CreateTranslationError;

        return new Translation(module, feature, component, name, slug, value,
            languageShortcut, sectionSlug, createdAt);
    }

    public ErrorOr<Success> Update(string value)
    {
        Value = value;
        return Result.Success;
    }

    public void Apply(TranslationCreatedEvent e)
    {
        Module = e.Module; Feature = e.Feature; Component = e.Component; Name = e.Name;
        Slug = e.Slug; Value = e.Value; LanguageShortcut = e.LanguageShortcut;
        SectionSlug = e.SectionSlug;
        CreatedAt = e.CreatedAt;
    }

    public void Apply(TranslationUpdatedEvent e)
    {
        Value = e.Value;
        UpdatedAt = e.UpdatedAt;
    }
}
```

### Events

**New file**: `Domain/Translations/Events/TranslationCreatedEvent.cs`

```csharp
using Nexticz.Lib.Shared.DataAccess.Marten.Configuration;

namespace Nexticz.Module.Lang.Domain.Translations.Events;

public record TranslationCreatedEvent(
    Guid Id, string Module, string Feature, string Component, string Name,
    string Slug, string Value, string LanguageShortcut,
    string SectionSlug, DateTimeOffset CreatedAt) : IMartenEvent;
```

**New file**: `Domain/Translations/Events/TranslationUpdatedEvent.cs`

```csharp
using Nexticz.Lib.Shared.DataAccess.Marten.Configuration;

namespace Nexticz.Module.Lang.Domain.Translations.Events;

public record TranslationUpdatedEvent(
    Guid Id, string Value, string SectionSlug, string LanguageShortcut,
    DateTimeOffset UpdatedAt) : IMartenEvent;
```

**New file**: `Domain/Translations/Events/TranslationDeletedEvent.cs`

```csharp
using Nexticz.Lib.Shared.DataAccess.Marten.Configuration;

namespace Nexticz.Module.Lang.Domain.Translations.Events;

public record TranslationDeletedEvent(
    Guid Id, string SectionSlug, string LanguageShortcut,
    DateTimeOffset DeletedAt) : IMartenEvent;
```

### Projection + Configurator

**New file**: `Infrastructure/Translations/TranslationProjection.cs`

```csharp
using JasperFx.Events;
using Marten.Events.Aggregation;
using Nexticz.Module.Lang.Domain.Translations;
using Nexticz.Module.Lang.Domain.Translations.Events;

namespace Nexticz.Module.Lang.Infrastructure.Translations;

public class TranslationProjection : SingleStreamProjection<Translation, Guid>
{
    public TranslationProjection() { DeleteEvent<TranslationDeletedEvent>(); }
    public void Apply(IEvent<TranslationCreatedEvent> @event, Translation t) => t.Apply(@event.Data);
    public void Apply(IEvent<TranslationUpdatedEvent> @event, Translation t) => t.Apply(@event.Data);
}
```

**New file**: `Infrastructure/Translations/TranslationConfigurator.cs`

```csharp
using JasperFx.Events.Projections;
using Marten;
using Nexticz.Lib.Shared.DataAccess.Marten.Configuration;

namespace Nexticz.Module.Lang.Infrastructure.Translations;

internal class TranslationConfigurator : IMartenConfigurator
{
    public void Configure(StoreOptions options)
    {
        options.Projections.Add<TranslationProjection>(ProjectionLifecycle.Inline);

        options.Schema.For<Domain.Translations.Translation>().Index(x => x.Slug);
        options.Schema.For<Domain.Translations.Translation>().Index(x => x.SectionSlug);
        options.Schema.For<Domain.Translations.Translation>().Index(x => x.LanguageShortcut);
        options.Schema.For<Domain.Translations.Translation>()
            .DocumentAlias(MartenConfigurationOrchestrator.PluralizeTableName<Domain.Translations.Translation>());
    }
}
```

---

## Step 4: CRUD Command/Query Handlers (Marten Style)

All commands implement `ILangCommand<TResponse>` so the `LangPostCommandBehavior` auto-saves after each handler. Queries use `ILangReadOnlyEventStoreRepository` for reads.

### Language CRUD

**CreateLanguageCommandHandler** — pattern:

```csharp
internal class CreateLanguageCommandHandler(
    ILangReadOnlyEventStoreRepository readOnlyRepository,
    ILangUnitOfWork unitOfWork,
    IClock clock) : IRequestHandler<CreateLanguageCommand, ErrorOr<Language>>
{
    public async Task<ErrorOr<Language>> Handle(CreateLanguageCommand request, CancellationToken cancellationToken)
    {
        var existing = await readOnlyRepository.GetFirstByConditionAsync<Language>(
            l => l.Shortcut == request.Shortcut, cancellationToken);

        if (existing is not null)
            return LanguageErrors.CreateLanguageError;

        var language = Language.CreateFrom(request.Name, request.Shortcut, request.IsActive, clock.UtcNowOffset);
        if (language.IsError) return language.Errors;

        var createdEvent = new LanguageCreatedEvent(language.Value.Id, language.Value.Name,
            language.Value.Shortcut, language.Value.IsActive, language.Value.CreatedAt);

        unitOfWork.StartStream<LanguageCreatedEvent, Language>(language.Value.Id, createdEvent);
        return language;
    }
}
```

**UpdateLanguageCommandHandler** — simplified, no Google Translate:

```csharp
internal class UpdateLanguageCommandHandler(
    ILangReadOnlyEventStoreRepository readOnlyRepository,
    ILangUnitOfWork unitOfWork,
    IClock clock) : IRequestHandler<UpdateLanguageCommand, ErrorOr<Success>>
{
    public async Task<ErrorOr<Success>> Handle(UpdateLanguageCommand command, CancellationToken cancellationToken)
    {
        var language = await readOnlyRepository.GetFirstByConditionAsync<Language>(
            l => l.Shortcut == command.Shortcut, cancellationToken);

        if (language is null)
            return LanguageErrors.LanguageWithShortcutDoesnotExist;

        var result = language.Update(command.Request.IsActive);
        if (result.IsError) return result.Errors;

        unitOfWork.AppendEvent(language.Id, new LanguageUpdatedEvent(
            language.Id, language.IsActive, clock.UtcNowOffset));

        return Result.Success;
    }
}
```

**Queries** use `readOnlyRepository.GetFilteredAsync<Language>(...)` and `readOnlyRepository.GetFirstByConditionAsync<Language>(...)` — same as Portal.

### Section Slugs (no CRUD — derived from translations)

There is **no Section entity or CRUD**. The `SectionSlug` is a plain string on `Translation`. If the FE needs a list of available sections, use:

```csharp
// Query: ListSectionSlugsQuery
var sectionSlugs = await session
    .Query<Translation>()
    .Select(t => t.SectionSlug)
    .Distinct()
    .ToListAsync(cancellationToken);
```

This returns e.g. `["auth", "lang", "shared", "mmo-settings"]` — no dedicated table needed.

### Translation CRUD

**CreateTranslationsCommandHandler** — creates translations and publishes MassTransit event:

```csharp
internal class CreateTranslationsCommandHandler(
    ILangReadOnlyEventStoreRepository readOnlyRepository,
    ILangUnitOfWork unitOfWork,
    ILangPublisher langPublisher,
    IClock clock) : IRequestHandler<CreateTranslationsCommand, ErrorOr<CreateTranslationsResponse>>
{
    public async Task<ErrorOr<CreateTranslationsResponse>> Handle(
        CreateTranslationsCommand command, CancellationToken cancellationToken)
    {
        foreach (var item in command.Request.Items)
        {
            var existing = await readOnlyRepository.GetFirstByConditionAsync<Translation>(
                t => t.Slug == item.Slug && t.LanguageShortcut == command.Request.LanguageShortcut,
                cancellationToken);

            if (existing is not null) continue;

            LangHelper.SplitComponentSlug(item.Slug, out var module, out var feature, out var component);
            var name = item.Slug.Split('-').Last();

            var translation = Translation.CreateFrom(
                module, feature, component, name, item.Slug, item.Value,
                command.Request.LanguageShortcut, command.Request.SectionSlug, clock.UtcNowOffset);

            if (translation.IsError) continue;

            var createdEvent = new TranslationCreatedEvent(
                translation.Value.Id, module, feature, component, name,
                item.Slug, item.Value, command.Request.LanguageShortcut,
                command.Request.SectionSlug, clock.UtcNowOffset);

            unitOfWork.StartStream<TranslationCreatedEvent, Translation>(translation.Value.Id, createdEvent);
        }

        // Publish MassTransit event for JSON regeneration
        await langPublisher.PublishTranslationSectionChanged(
            command.Request.SectionSlug, command.Request.LanguageShortcut, cancellationToken);

        // ... return response
    }
}
```

**UpdateTranslationCommandHandler** — after updating, publishes MassTransit event:

```csharp
// After appending TranslationUpdatedEvent:
await langPublisher.PublishTranslationSectionChanged(
    translation.SectionSlug, translation.LanguageShortcut, cancellationToken);
```

**RemoveTranslationCommandHandler** — same pattern, publish after delete event.

---

## Step 5: MassTransit Event-Driven JSON Regeneration

### MassTransit message contract

**New file**: `Nexticz.Module.Lang.Contracts/Translations/TranslationSectionChanged.cs`

```csharp
namespace Nexticz.Module.Lang.Contracts.Translations;

public record TranslationSectionChanged(string SectionSlug, string LanguageShortcut);
```

This is the message published via RabbitMQ when any translation in a section is created, updated, or deleted.

### Publisher

**New file**: `Application/MassTransitPublishers/ILangPublisher.cs`

```csharp
using Nexticz.Lib.Shared.MessagePublishers;

namespace Nexticz.Module.Lang.Application.MassTransitPublishers;

internal interface ILangPublisher : IBaseMessagePublisher
{
    Task PublishTranslationSectionChanged(string sectionSlug, string languageShortcut,
        CancellationToken cancellationToken);
}
```

**New file**: `Application/MassTransitPublishers/LangPublisher.cs`

```csharp
using MassTransit;
using Nexticz.Lib.Shared.MessagePublishers;
using Nexticz.Module.Lang.Contracts.Translations;

namespace Nexticz.Module.Lang.Application.MassTransitPublishers;

internal class LangPublisher(IPublishEndpoint publishEndpoint)
    : BaseMessagePublisher(publishEndpoint), ILangPublisher
{
    public async Task PublishTranslationSectionChanged(string sectionSlug, string languageShortcut,
        CancellationToken cancellationToken)
    {
        await PublishAsync(new TranslationSectionChanged(sectionSlug, languageShortcut), cancellationToken);
    }
}
```

Register in DI: `services.AddScoped<ILangPublisher, LangPublisher>();`

### Consumer — regenerates JSON file

**New file**: `Infrastructure/Consumers/TranslationSectionChangedConsumer.cs`

```csharp
using MassTransit;
using Microsoft.Extensions.Logging;
using Microsoft.FeatureManagement;
using Nexticz.Module.Lang.Contracts.Translations;

namespace Nexticz.Module.Lang.Infrastructure.Consumers;

internal class TranslationSectionChangedConsumer(
    ILogger<TranslationSectionChangedConsumer> logger,
    IFeatureManager featureManager,
    ITranslationFileGenerator translationFileGenerator) : IConsumer<TranslationSectionChanged>
{
    public async Task Consume(ConsumeContext<TranslationSectionChanged> context)
    {
        if (!await featureManager.IsEnabledAsync("LangJsonGenerationEnabled"))
        {
            logger.LogTrace("LangJsonGenerationEnabled is disabled. Skipping JSON regeneration for section {Section}",
                context.Message.SectionSlug);
            return;
        }

        logger.LogInformation("Regenerating JSON file for section {Section}, language {Language}",
            context.Message.SectionSlug, context.Message.LanguageShortcut);

        await translationFileGenerator.RegenerateAsync(
            context.Message.SectionSlug, context.Message.LanguageShortcut, context.CancellationToken);
    }
}
```

**New file**: `Infrastructure/Consumers/TranslationSectionChangedConsumerDefinition.cs`

```csharp
using MassTransit;

namespace Nexticz.Module.Lang.Infrastructure.Consumers;

internal class TranslationSectionChangedConsumerDefinition : ConsumerDefinition<TranslationSectionChangedConsumer>
{
    private static string QueuePrefix => MassTransitModulePrefixName.ModulePrefixName;

    public TranslationSectionChangedConsumerDefinition()
    {
        EndpointName = $"{QueuePrefix}-{KebabCaseEndpointNameFormatter.Instance.Consumer<TranslationSectionChangedConsumer>()}";
    }
}
```

**New file**: `Infrastructure/MassTransitModulePrefixName.cs`

```csharp
namespace Nexticz.Module.Lang.Infrastructure;

internal static class MassTransitModulePrefixName
{
    public static string ModulePrefixName => "module-lang";
}
```

### Consumer registrar (auto-discovered by shared MassTransitRegistrator)

**New file**: `Infrastructure/MassTransitRegistrator.cs`

```csharp
using MassTransit;
using Nexticz.Lib.Shared.MassTransit;
using Nexticz.Module.Lang.Infrastructure.Consumers;

namespace Nexticz.Module.Lang.Infrastructure;

public class MassTransitRegistrator : IConsumerRegistrar
{
    public void Register(IBusRegistrationConfigurator cfg)
    {
        cfg.AddConsumer<TranslationSectionChangedConsumer, TranslationSectionChangedConsumerDefinition>();
    }
}
```

### Flow diagram

```
Command Handler                     RabbitMQ                    Consumer
┌──────────────────┐     publish    ┌──────────────┐    consume   ┌───────────────────────────┐
│ Create/Update/   │ ──────────→    │ Translation  │ ──────────→  │ TranslationSectionChanged │
│ Delete           │                │ Section      │              │ Consumer                  │
│ Translation      │                │ Changed      │              │                           │
│                  │                │ {SectionSlug,│              │ 1. Check feature flag     │
│ 1. Append event  │                │  LangShortcut│              │ 2. Query Marten for       │
│ 2. Publish msg   │                │ }            │              │    section+lang            │
└──────────────────┘                └──────────────┘              │ 3. Write JSON file        │
                                                                  └───────────────────────────┘
```

---

## Step 6: JSON File Generator (Reads from Marten)

**New file**: `Application/Common/Interfaces/ITranslationFileGenerator.cs`

```csharp
namespace Nexticz.Module.Lang.Application.Common.Interfaces;

public interface ITranslationFileGenerator
{
    Task RegenerateAsync(string sectionSlug, string languageShortcut, CancellationToken cancellationToken);
    Task RegenerateAllAsync(CancellationToken cancellationToken);
}
```

**New file**: `Infrastructure/Translations/TranslationFileGenerator.cs`

```csharp
using System.Text.Json;
using Marten;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Logging;
using Nexticz.Module.Lang.Application.Common.Interfaces;

namespace Nexticz.Module.Lang.Infrastructure.Translations;

internal class TranslationFileGenerator(
    ILangDocumentStore documentStore,
    IWebHostEnvironment env,
    ILogger<TranslationFileGenerator> logger) : ITranslationFileGenerator
{
    private readonly string _outputPath = Path.Combine(env.ContentRootPath, "wwwroot", "translations");

    public async Task RegenerateAsync(string sectionSlug, string languageShortcut,
        CancellationToken cancellationToken)
    {
        await using var session = documentStore.QuerySession();

        var translations = await session
            .Query<Domain.Translations.Translation>()
            .Where(t => t.SectionSlug == sectionSlug && t.LanguageShortcut == languageShortcut)
            .ToListAsync(cancellationToken);

        var dict = translations.ToDictionary(t => t.Slug, t => t.Value);

        var langDir = Path.Combine(_outputPath, languageShortcut.ToLower());
        Directory.CreateDirectory(langDir);

        var filePath = Path.Combine(langDir, $"{sectionSlug}.json");
        var json = JsonSerializer.Serialize(dict, new JsonSerializerOptions { WriteIndented = true });
        await File.WriteAllTextAsync(filePath, json, cancellationToken);

        logger.LogInformation("Regenerated translation file {FilePath} with {Count} entries",
            filePath, dict.Count);
    }

    public async Task RegenerateAllAsync(CancellationToken cancellationToken)
    {
        await using var session = documentStore.QuerySession();

        var activeLanguages = await session
            .Query<Domain.Languages.Language>()
            .Where(l => l.IsActive)
            .ToListAsync(cancellationToken);

        // No Section table — derive section slugs from translations
        var sectionSlugs = await session
            .Query<Domain.Translations.Translation>()
            .Select(t => t.SectionSlug)
            .Distinct()
            .ToListAsync(cancellationToken);

        foreach (var language in activeLanguages)
        {
            foreach (var sectionSlug in sectionSlugs)
            {
                await RegenerateAsync(sectionSlug, language.Shortcut, cancellationToken);
            }
        }
    }
}
```

Register: `services.AddScoped<ITranslationFileGenerator, TranslationFileGenerator>();`

### JSON file format

```json
// wwwroot/translations/cs/auth.json
{
    "auth-api-authService-invalidPassword": "Neplatné heslo",
    "auth-api-authService-userIsCurentlyLogged": "Neočekávaná chyba, byli jste odhlášeni",
    "auth-api-authService-validationTokenError": "JWT token není validní"
}
```

Full slug as key — FE error `slug` field maps directly.

---

## Step 7: Feature Flag — `LangJsonGenerationEnabled`

### Configuration

In `appsettings.*.json`:

```json
"FeatureManagement": {
    "LangJsonGenerationEnabled": true
}
```

The flag is checked in the MassTransit consumer (`TranslationSectionChangedConsumer`) before regenerating a JSON file. This follows the existing pattern used by `BaseBackgroundService` and other modules (e.g. `CuzkIsEnabled`, `WorkersEnabled`).

`Microsoft.FeatureManagement` is already registered via `builder.Services.AddFeatureManagement()` in `Program.cs` and available via DI (`IFeatureManager`).

### When to use the flag

| Scenario | Flag enabled | Flag disabled |
|----------|-------------|---------------|
| Translation CRUD | MassTransit event published, consumer writes JSON file | MassTransit event published, consumer skips file write |
| Seed | After seed completes, `RegenerateAllAsync` respects the flag | Seed writes to DB but no JSON files generated |
| Local dev without RabbitMQ | Set to `false` — translations still saved in Marten | No file generation errors |

---

## Step 8: Serve JSON Files to FE

### Static file middleware

In `UseLangModule`:

```csharp
public static void UseLangModule(this WebApplication app)
{
    var translationsPath = Path.Combine(app.Environment.ContentRootPath, "wwwroot", "translations");
    Directory.CreateDirectory(translationsPath);

    app.UseStaticFiles(new StaticFileOptions
    {
        FileProvider = new PhysicalFileProvider(translationsPath),
        RequestPath = "/translations",
        OnPrepareResponse = ctx =>
        {
            ctx.Context.Response.Headers.CacheControl = "public, max-age=3600, must-revalidate";
        }
    });

    app.MapLangApiEndpoints();
}
```

FE fetches: `GET /translations/cs/auth.json` — no auth required.

### FE integration

```
1. App init → GET /translations/{lang}/{section}.json for each section the app needs
2. Store in memory / state manager
3. On API error → error.slug → lookup in loaded translations → show translated message
4. If slug not found → fall back to error.message (Czech hardcoded text from ErrorOr description)
```

---

## Step 9: Remove Google Translate API

1. **Delete** `Application/Languages/Configurations/GoogleApisSettings.cs`
2. **Remove** Google Translate logic from `UpdateLanguageCommandHandler` — handler now only toggles `IsActive`
3. **Remove** `Google.Cloud.Translation.V2` package reference from csproj
4. **Remove** `GoogleApisSettings` section from `appsettings.*.json`

Translations for non-Czech languages are entered manually via the Translation CRUD endpoints.

---

## Step 10: Reflection-Based Seed (Unified)

Single `TranslationsSeed` in the Lang module scans **all loaded assemblies** for `IErrorComponentSlugProvider` and `ITranslatable`:

```csharp
public class TranslationsSeed
{
    public async Task RunSeed(IServiceProvider serviceProvider)
    {
        var mediatr = serviceProvider.GetRequiredService<ISender>();
        var logger = serviceProvider.GetRequiredService<ILogger<TranslationsSeed>>();

        var itemsToSeed = new List<CreateTranslationsItem>();

        // 1. Scan for IErrorComponentSlugProvider
        var errorTypes = AppDomain.CurrentDomain.GetAssemblies()
            .SelectMany(a => a.GetTypes())
            .Where(t => typeof(IErrorComponentSlugProvider).IsAssignableFrom(t) && !t.IsInterface);

        foreach (var errorType in errorTypes)
            itemsToSeed.AddRange(GetErrorsFromType(errorType));

        // 2. Scan for static ITranslatable properties
        var translatableTypes = AppDomain.CurrentDomain.GetAssemblies()
            .SelectMany(a => a.GetTypes())
            .Where(t => t.GetProperties(BindingFlags.Public | BindingFlags.Static)
                .Any(p => typeof(ITranslatable).IsAssignableFrom(p.PropertyType)));

        foreach (var type in translatableTypes)
            itemsToSeed.AddRange(GetTranslatablesFromType(type));

        // 3. Group by section slug (module part of slug) and seed
        var groupedBySection = itemsToSeed.GroupBy(x => x.Slug.Split('-')[0]);

        foreach (var group in groupedBySection)
        {
            // SectionSlug = group.Key (e.g. "auth", "lang", "mmo")
            // Create translations via MediatR command with SectionSlug = group.Key
        }
    }
}
```

### Remove per-module seeds

1. **Remove** `Auth/.../Seeds/TranslationsSeed.cs`
2. **Remove** `Auth/.../Interfaces/ILangApiService.cs` and its HTTP-based seeding
3. **Remove** translation seed calls from Auth's `DatabaseInitializer`
4. All seeding is now centralized in the Lang module

---

## Step 11: API Endpoints

### Language endpoints

| Method   | Route                            | Auth       | Description          |
|----------|----------------------------------|------------|----------------------|
| `GET`    | `/api/lang/languages`            | Anonymous  | List languages       |
| `GET`    | `/api/lang/languages/{shortcut}` | Authorized | Get by shortcut      |
| `PUT`    | `/api/lang/languages/{shortcut}` | Authorized | Toggle active        |

### Section slugs endpoint (derived from translations, no CRUD)

| Method | Route                    | Auth       | Description                              |
|--------|--------------------------|------------|------------------------------------------|
| `GET`  | `/api/lang/sections`     | Authorized | List distinct section slugs from translations |

### Translation endpoints

| Method   | Route                              | Auth       | Description           |
|----------|-------------------------------------|------------|----------------------|
| `GET`    | `/api/lang/translations`            | Anonymous  | List translations    |
| `GET`    | `/api/lang/translations/{id}`       | Authorized | Get by ID            |
| `POST`   | `/api/lang/translations`            | Anonymous  | Batch create         |
| `PUT`    | `/api/lang/translations/{id}`       | Authorized | Update value         |
| `DELETE` | `/api/lang/translations/{id}`       | Authorized | Remove translation   |

### Static file endpoint (no code needed — served by middleware)

| Method | Route                                | Auth      | Description         |
|--------|--------------------------------------|-----------|---------------------|
| `GET`  | `/translations/{lang}/{section}.json`| Anonymous | Pre-generated JSON  |

---

## Summary: Files to Create / Modify / Delete

### Create

| File | Layer | Purpose |
|------|-------|---------|
| `Domain/AggregateRoot.cs` | Domain | Module-level base class |
| `Domain/Languages/Language.cs` (rewrite) | Domain | Event-sourced aggregate |
| `Domain/Languages/Events/*.cs` | Domain | Created, Updated, Deleted events |
| `Domain/Translations/Translation.cs` (rewrite) | Domain | Event-sourced aggregate (with SectionSlug string) |
| `Domain/Translations/Events/*.cs` | Domain | Created, Updated, Deleted events |
| `Contracts/Translations/TranslationSectionChanged.cs` | Contracts | MassTransit message |
| `Application/ILangCommand.cs` | Application | Command marker interface |
| `Application/Interfaces/ILangDocumentSessionProvider.cs` | Application | Marten session provider |
| `Application/Interfaces/ILangUnitOfWork.cs` | Application | Marten UoW |
| `Application/Interfaces/ILangReadOnlyEventStoreRepository.cs` | Application | Marten read-only repo |
| `Application/PipelineBehaviors/LangPostCommandBehavior.cs` | Application | Auto-save behavior |
| `Application/MassTransitPublishers/ILangPublisher.cs` | Application | Publisher interface |
| `Application/MassTransitPublishers/LangPublisher.cs` | Application | Publisher implementation |
| `Application/Common/Interfaces/ITranslationFileGenerator.cs` | Application | JSON generation contract |
| `Infrastructure/ILangDocumentStore.cs` | Infrastructure | Marten store marker |
| `Infrastructure/BaseRepositories/LangDocumentSessionProvider.cs` | Infrastructure | Session provider |
| `Infrastructure/BaseRepositories/LangUnitOfWork.cs` | Infrastructure | UoW implementation |
| `Infrastructure/BaseRepositories/LangReadOnlyEventStoreRepository.cs` | Infrastructure | Read-only repo |
| `Infrastructure/Languages/LanguageProjection.cs` | Infrastructure | Marten projection |
| `Infrastructure/Languages/LanguageConfigurator.cs` | Infrastructure | Marten schema config |
| `Infrastructure/Translations/TranslationProjection.cs` | Infrastructure | Marten projection |
| `Infrastructure/Translations/TranslationConfigurator.cs` | Infrastructure | Marten schema config |
| `Infrastructure/Translations/TranslationFileGenerator.cs` | Infrastructure | JSON file generation |
| `Infrastructure/Consumers/TranslationSectionChangedConsumer.cs` | Infrastructure | MassTransit consumer |
| `Infrastructure/Consumers/TranslationSectionChangedConsumerDefinition.cs` | Infrastructure | Consumer config |
| `Infrastructure/MassTransitModulePrefixName.cs` | Infrastructure | Queue prefix |
| `Infrastructure/MassTransitRegistrator.cs` | Infrastructure | Consumer registration |

### Modify

| File | Change |
|------|--------|
| `Infrastructure/DependencyInjection.cs` | Replace EF Core with Marten, register new services |
| `Application/DependencyInjection.cs` | Add PostCommandBehavior, register publisher |
| `Presentation/DependencyInjection.cs` | Add static file middleware, remove EF seed |
| `Presentation/ApiEndpoints.cs` | Add section-slugs list route |
| `Presentation/Endpoints/EndpointsExtensions.cs` | Register section-slugs endpoint |
| `Contracts/Translations/CreateTranslationsRequest.cs` | Add `SectionSlug` |
| Application command/query handlers | Rewrite to use Marten repos + publish MassTransit events |
| `Infrastructure/Seeds/TranslationsSeed.cs` | Rewrite: unified reflection scan |
| `appsettings.*.json` | Add `Lang` connection string, add `LangJsonGenerationEnabled` feature flag |

### Delete

| File | Reason |
|------|--------|
| `Infrastructure/Common/Persistence/DataContext.cs` | Replaced by Marten |
| `Infrastructure/Common/Persistence/UnitOfWork.cs` | Replaced by `LangUnitOfWork` |
| `Infrastructure/Translations/Persistance/TranslationConfigurations.cs` | EF Core config |
| `Infrastructure/Languages/Persistance/LanguageConfigurations.cs` | EF Core config |
| `Infrastructure/Translations/Persistance/TranslationsRepository.cs` | Replaced by Marten repo |
| `Infrastructure/Languages/Persistance/LanguagesRepository.cs` | Replaced by Marten repo |
| `Infrastructure/Migrations/*` | EF Core migrations |
| `Application/Common/Interfaces/IUnitOfWork.cs` | Replaced by `ILangUnitOfWork` |
| `Application/Common/Interfaces/ITranslationsRepository.cs` | Replaced by generic Marten repo |
| `Application/Common/Interfaces/ILanguagesRepository.cs` | Replaced by generic Marten repo |
| `Application/Common/Helpers/StringHelper.cs` | EF migration constants |
| `Application/Languages/Configurations/GoogleApisSettings.cs` | Google Translate removed |
| `Auth/.../Seeds/TranslationsSeed.cs` | Replaced by unified seed |
| `Auth/.../Interfaces/ILangApiService.cs` | No more HTTP-based seeding |
| `Google.Cloud.Translation.V2` package reference | No longer needed |
| `Microsoft.EntityFrameworkCore.SqlServer` package reference | Replaced by Marten |
