# Lang Module Refactoring Instructions

## Goals

1. **Replace EF Core + SQL Server with Marten + PostgreSQL** — follow the Portal module pattern (event sourcing, projections, document store)
2. Two complete CRUD systems: **Language** and **Translation** — Translation has `Name`, `Scope`, `Key` (computed: `Scope-Name`), `Value`, `Language`
3. **Remove Google Translate API** dependency entirely
4. **Reflection-based seed** that scans all assemblies for `IErrorComponentSlugProvider` and `ITranslatable` implementations
5. **MassTransit event-driven multi-language creation** — when a translation is created, publish a MassTransit event; a consumer creates translations for all other enabled languages for the tenant
6. **API endpoint for translations by scope** — FE fetches `GET /api/lang/translations/scope/{scope}` with language from request header
7. **FE auto-creates missing translations** — if the FE needs a translation that doesn't exist in the response, it sends a fire-and-forget `POST` to create it and displays the default value immediately

### Why Scope (not a separate entity)?

The `Scope` is a categorization string — e.g. `"nexti-pageNotFound"`, `"auth-api-authService"`, `"lang-settings"`. It has no meaningful metadata beyond the string itself. The FE determines the scope when creating translations. Storing it as a plain `string` on `Translation` avoids unnecessary CRUD, FK constraints, and complexity. If the FE needs a list of available scopes, a `SELECT DISTINCT Scope FROM Translations` query provides it without a dedicated table.

#### Scope maps to `IErrorComponentSlugProvider.ComponentSlug`

For error translations, the scope is the `ComponentSlug` (minus trailing dash):

| `IErrorComponentSlugProvider` | Scope | Name | Key |
|-------------------------------|-------|------|-----|
| `ComponentSlug = "lang-api-languageService-"` | `lang-api-languageService` | `doesNotExist` | `lang-api-languageService-doesNotExist` |
| `ComponentSlug = "auth-api-authService-"` | `auth-api-authService` | `invalidPassword` | `auth-api-authService-invalidPassword` |

For UI translations, the scope is the module/feature prefix:

| Scope | Name | Key |
|-------|------|-----|
| `nexti-pageNotFound` | `title` | `nexti-pageNotFound-title` |
| `nexti-dashboard` | `welcomeMessage` | `nexti-dashboard-welcomeMessage` |

---

## Migration Strategy — Feature Flag (Keep Old Module)

The old Lang module (`Nexticz.Module.Lang.Presentation`, `.Application`, `.Infrastructure`, `.Domain`, `.Contracts`) **stays untouched**. A brand-new `Nexticz.Module.Lang` project is created alongside it. A feature flag in `Program.cs` toggles between old and new — only one is active at a time.

This follows the same pattern as **Auth** (new `Nexticz.Module.Auth` project exists alongside old 5-project module) and **Cuzk** (feature flag gating in `ServiceCollectionExtensions.cs`).

### Project structure (side-by-side)

```
Modules/Lang/
├── Nexticz.Module.Lang/                          ← NEW (single project, Marten-based)
│   ├── Nexticz.Module.Lang.csproj
│   ├── ServiceCollectionExtensions.cs            ← AddLangModuleV2() / UseLangModuleV2Async()
│   ├── ModuleName.cs
│   ├── ModuleNameProvider.cs
│   ├── Application/
│   │   ├── ApplicationServiceCollectionExtensions.cs
│   │   ├── Interfaces/
│   │   ├── PipelineBehaviors/
│   │   ├── MassTransitPublishers/
│   │   ├── Languages/ (Commands, Queries)
│   │   └── Translations/ (Commands, Queries)
│   ├── Domain/
│   │   ├── AggregateRoot.cs
│   │   ├── Languages/ (Language.cs, Events/, LanguageErrors.cs)
│   │   └── Translations/ (Translation.cs, Events/, TranslationErrors.cs)
│   └── Infrastructure/
│       ├── InfrastructureServiceCollectionExtensions.cs
│       ├── ILangDocumentStore.cs
│       ├── BaseRepositories/
│       ├── Languages/ (Projection, Configurator)
│       ├── Translations/ (Projection, Configurator)
│       ├── Consumers/
│       ├── MassTransitModulePrefixName.cs
│       ├── MassTransitRegistrator.cs
│       └── Dbs/Seeds/
│
├── Nexticz.Module.Lang.Presentation/             ← OLD (untouched)
├── Nexticz.Module.Lang.Application/              ← OLD (untouched)
├── Nexticz.Module.Lang.Infrastructure/           ← OLD (untouched)
├── Nexticz.Module.Lang.Domain/                   ← OLD (untouched)
└── Nexticz.Module.Lang.Contracts/                ← SHARED (used by both old and new)
```

### New project references

`Nexticz.Module.Lang.csproj`:

```xml
<Project Sdk="Microsoft.NET.Sdk">
    <PropertyGroup>
        <TargetFramework>net10.0</TargetFramework>
        <ImplicitUsings>enable</ImplicitUsings>
        <Nullable>enable</Nullable>
    </PropertyGroup>
    <ItemGroup>
        <ProjectReference Include="..\..\..\Libs\Nexticz.Lib.Shared\Nexticz.Lib.Shared.csproj" />
        <ProjectReference Include="..\Nexticz.Module.Lang.Contracts\Nexticz.Module.Lang.Contracts.csproj" />
    </ItemGroup>
</Project>
```

### Feature flag — `LangV2IsEnabled`

**New file**: `Nexticz.Module.Lang/ServiceCollectionExtensions.cs`

```csharp
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.FeatureManagement;
using Nexticz.Module.Lang.Application;
using Nexticz.Module.Lang.Infrastructure;

namespace Nexticz.Module.Lang;

public static class ServiceCollectionExtensions
{
    private const string LangV2IsEnabled = nameof(LangV2IsEnabled);

    public static IServiceCollection AddLangModuleV2(this IServiceCollection services,
        IConfiguration configuration)
    {
        var isEnabled = configuration.GetValue<bool>($"FeatureManagement:{LangV2IsEnabled}");

        if (!isEnabled)
            return services;

        services.AddApplicationLayer();
        services.AddInfrastructureLayer(configuration);

        return services;
    }

    public static async Task UseLangModuleV2Async(this WebApplication app)
    {
        var featureManager = app.Services.GetRequiredService<IFeatureManager>();
        if (!await featureManager.IsEnabledAsync(LangV2IsEnabled))
            return;

        await app.UseInfrastructureLayerAsync();
        app.MapLangV2ApiEndpoints();
    }
}
```

### Old module also gets a feature flag guard

Update `Nexticz.Module.Lang.Presentation/DependencyInjection.cs` — add the inverse check so old module is **disabled** when V2 is enabled:

```csharp
public static IServiceCollection AddLangModule(this IServiceCollection services, IConfiguration configuration)
{
    var v2Enabled = configuration.GetValue<bool>("FeatureManagement:LangV2IsEnabled");
    if (v2Enabled)
        return services;  // V2 is active, skip old module

    return services
        .AddLangPresentation()
        .AddLangApplication()
        .AddLangInfrastructure(configuration);
}

public static void UseLangModule(this WebApplication app)
{
    var v2Enabled = app.Configuration.GetValue<bool>("FeatureManagement:LangV2IsEnabled");
    if (v2Enabled)
        return;  // V2 is active, skip old module

    app.MapLangApiEndpoints();
    _ = app.SeedLangDatabaseAsync(app);
}
```

### Program.cs — register both, flag decides

```csharp
// In Program.cs — both are called, but only one activates based on LangV2IsEnabled
builder.Services
    .AddLangModule(builder.Configuration)      // old (active when LangV2IsEnabled = false)
    .AddLangModuleV2(builder.Configuration);   // new (active when LangV2IsEnabled = true)

// ...

app.UseLangModule();                           // old (skips if LangV2IsEnabled = true)
await app.UseLangModuleV2Async();              // new (skips if LangV2IsEnabled = false)
```

### Jipocar API project reference

Add to `Nexticz.Nexty.Jipocar.Api.csproj`:

```xml
<!-- Existing -->
<ProjectReference Include="..\..\..\..\..\Modules\Lang\Nexticz.Module.Lang.Presentation\Nexticz.Module.Lang.Presentation.csproj" />
<!-- New -->
<ProjectReference Include="..\..\..\..\..\Modules\Lang\Nexticz.Module.Lang\Nexticz.Module.Lang.csproj" />
```

### Feature management configuration

In `appsettings.Development.json`:

```json
"FeatureManagement": {
    "LangV2IsEnabled": false
}
```

Set to `true` to activate the new Marten-based module. Set to `false` (or omit) to keep using the old EF Core module.

### Contracts are shared

`Nexticz.Module.Lang.Contracts` is referenced by **both** old and new modules. New request/response types (e.g. `CreateTranslationRequest` with `{ Scope, Name, Value }`) are added to this shared Contracts project. Old request types remain for the old module.

---

> **All steps below describe code that goes into the NEW `Nexticz.Module.Lang` project.** The old module's files are not modified (except the feature flag guard in `Presentation/DependencyInjection.cs`).

---

## Step 1: Marten Infrastructure (Portal Pattern)

The new Lang module uses Marten (PostgreSQL event store + document projections), matching the existing Portal module infrastructure. All code below lives in the `Nexticz.Module.Lang` project.

### New shared infrastructure wiring

Follow Portal's pattern exactly:

**New file**: `Infrastructure/ILangDocumentStore.cs`

```csharp
using Marten;

namespace Nexticz.Module.Lang.Infrastructure;

public interface ILangDocumentStore : IDocumentStore;
```

**New file**: `Application/Interfaces/ILangDocumentSessionProvider.cs`

```csharp
using Nexticz.Lib.Shared.DataAccess.Marten;

namespace Nexticz.Module.Lang.Application.Interfaces;

internal interface ILangDocumentSessionProvider : IMartenDocumentSessionProvider;
```

**New file**: `Application/Interfaces/ILangUnitOfWork.cs`

```csharp
using Nexticz.Lib.Shared.DataAccess.Marten;

namespace Nexticz.Module.Lang.Application.Interfaces;

internal interface ILangUnitOfWork : IMartenUnitOfWork;
```

**New file**: `Application/Interfaces/ILangReadOnlyEventStoreRepository.cs`

```csharp
using Nexticz.Lib.Shared.DataAccess.Marten;

namespace Nexticz.Module.Lang.Application.Interfaces;

internal interface ILangReadOnlyEventStoreRepository : IMartenReadOnlyEventStoreRepository;
```

**New file**: `Application/ILangCommand.cs`

```csharp
using MediatR;

namespace Nexticz.Module.Lang.Application;

internal interface ILangCommand<out TResponse> : IRequest<TResponse>;
```

### Infrastructure implementations (same as Portal)

**New file**: `Infrastructure/BaseRepositories/LangDocumentSessionProvider.cs`

```csharp
using Nexticz.Lib.Shared.DataAccess.Marten;
using Nexticz.Module.Lang.Application.Interfaces;

namespace Nexticz.Module.Lang.Infrastructure.BaseRepositories;

internal class LangDocumentSessionProvider(ILangDocumentStore store)
    : MartenDocumentSessionProvider(store), ILangDocumentSessionProvider;
```

**New file**: `Infrastructure/BaseRepositories/LangUnitOfWork.cs`

```csharp
using Nexticz.Lib.Shared.DataAccess.Marten;
using Nexticz.Lib.Shared.UserProviders;
using Nexticz.Module.Lang.Application.Interfaces;

namespace Nexticz.Module.Lang.Infrastructure.BaseRepositories;

internal class LangUnitOfWork(ILangDocumentSessionProvider documentSessionProvider, ICurrentUserProvider currentUserProvider)
    : MartenUnitOfWork(documentSessionProvider, currentUserProvider), ILangUnitOfWork;
```

**New file**: `Infrastructure/BaseRepositories/LangReadOnlyEventStoreRepository.cs`

```csharp
using Nexticz.Lib.Shared.DataAccess.Marten;
using Nexticz.Module.Lang.Application.Interfaces;

namespace Nexticz.Module.Lang.Infrastructure.BaseRepositories;

internal class LangReadOnlyEventStoreRepository(ILangDocumentSessionProvider documentSessionProvider)
    : MartenReadOnlyEventStoreRepository(documentSessionProvider), ILangReadOnlyEventStoreRepository;
```

### PostCommandBehavior (auto-save after commands)

**New file**: `Application/PipelineBehaviors/LangPostCommandBehavior.cs`

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

**New file**: `Infrastructure/InfrastructureServiceCollectionExtensions.cs`

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

### EF Core artifacts NOT carried over to new module

The old module's EF Core infrastructure stays untouched in the old projects. The new `Nexticz.Module.Lang` project does **not** include these — it uses Marten instead:

| Old module file (stays in place) | New module equivalent |
|----------------------------------|----------------------|
| `Nexticz.Module.Lang.Infrastructure/Common/Persistence/DataContext.cs` | Marten document store (`ILangDocumentStore`) |
| `Nexticz.Module.Lang.Infrastructure/Common/Persistence/UnitOfWork.cs` | `LangUnitOfWork` |
| `Nexticz.Module.Lang.Infrastructure/Translations/Persistance/TranslationConfigurations.cs` | Marten `TranslationConfigurator` |
| `Nexticz.Module.Lang.Infrastructure/Languages/Persistance/LanguageConfigurations.cs` | Marten `LanguageConfigurator` |
| `Nexticz.Module.Lang.Infrastructure/Translations/Persistance/TranslationsRepository.cs` | `ILangReadOnlyEventStoreRepository` |
| `Nexticz.Module.Lang.Infrastructure/Languages/Persistance/LanguagesRepository.cs` | `ILangReadOnlyEventStoreRepository` |
| `Nexticz.Module.Lang.Infrastructure/Migrations/*` | Marten auto-creates schema |
| `Nexticz.Module.Lang.Application/Common/Interfaces/IUnitOfWork.cs` | `ILangUnitOfWork` |
| `Nexticz.Module.Lang.Application/Common/Interfaces/ITranslationsRepository.cs` | Generic Marten repo |
| `Nexticz.Module.Lang.Application/Common/Interfaces/ILanguagesRepository.cs` | Generic Marten repo |
| `Nexticz.Module.Lang.Application/Common/Helpers/StringHelper.cs` | Not needed (no EF migration constants) |

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

**New file**: `Domain/Languages/Language.cs`

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

### Translation data model

| Field | Type | Example | Description |
|-------|------|---------|-------------|
| `Name` | string | `pageNotFound` | Translation name within a scope |
| `Scope` | string | `nexti-pageNotFound` | Grouping key (module-feature or ComponentSlug) |
| `Key` | string | `nexti-pageNotFound-pageNotFound` | Computed: `Scope-Name` — unique per language |
| `Value` | string | `Stranka nenalezena` | Translated text |
| `Language` | string | `cz`, `en` | Language code |

### Aggregate

**Rewrite**: `Domain/Translations/Translation.cs`

```csharp
using ErrorOr;
using Nexticz.Module.Lang.Domain.Translations.Events;

namespace Nexticz.Module.Lang.Domain.Translations;

public class Translation : AggregateRoot
{
    public string Name { get; private set; }        // "pageNotFound"
    public string Scope { get; private set; }       // "nexti-pageNotFound"
    public string Key { get; private set; }         // "nexti-pageNotFound-pageNotFound" (Scope-Name)
    public string Value { get; private set; }       // "Stranka nenalezena"
    public string Language { get; private set; }    // "cz", "en"
    public DateTimeOffset CreatedAt { get; private set; }
    public DateTimeOffset? UpdatedAt { get; private set; }

    private Translation() { }

    private Translation(
        string name, string scope, string value, string language,
        DateTimeOffset createdAt, Guid? id = null) : base(id ?? Guid.NewGuid())
    {
        Name = name;
        Scope = scope;
        Key = $"{scope}-{name}";
        Value = value;
        Language = language;
        CreatedAt = createdAt;
    }

    public static ErrorOr<Translation> CreateFrom(
        string name, string scope, string value, string language,
        DateTimeOffset createdAt)
    {
        if (string.IsNullOrWhiteSpace(name))
            return TranslationErrors.CreateTranslationError;

        if (string.IsNullOrWhiteSpace(scope))
            return TranslationErrors.CreateTranslationError;

        return new Translation(name, scope, value, language, createdAt);
    }

    public ErrorOr<Success> Update(string value)
    {
        Value = value;
        return Result.Success;
    }

    public void Apply(TranslationCreatedEvent e)
    {
        Name = e.Name;
        Scope = e.Scope;
        Key = e.Key;
        Value = e.Value;
        Language = e.Language;
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
    Guid Id, string Name, string Scope, string Key,
    string Value, string Language, DateTimeOffset CreatedAt) : IMartenEvent;
```

**New file**: `Domain/Translations/Events/TranslationUpdatedEvent.cs`

```csharp
using Nexticz.Lib.Shared.DataAccess.Marten.Configuration;

namespace Nexticz.Module.Lang.Domain.Translations.Events;

public record TranslationUpdatedEvent(
    Guid Id, string Value, DateTimeOffset UpdatedAt) : IMartenEvent;
```

**New file**: `Domain/Translations/Events/TranslationDeletedEvent.cs`

```csharp
using Nexticz.Lib.Shared.DataAccess.Marten.Configuration;

namespace Nexticz.Module.Lang.Domain.Translations.Events;

public record TranslationDeletedEvent(Guid Id, DateTimeOffset DeletedAt) : IMartenEvent;
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

        options.Schema.For<Domain.Translations.Translation>().UniqueIndex(x => new { x.Key, x.Language });
        options.Schema.For<Domain.Translations.Translation>().Index(x => x.Scope);
        options.Schema.For<Domain.Translations.Translation>().Index(x => x.Language);
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

### Scopes (no CRUD — derived from translations)

There is **no Scope entity or CRUD**. The `Scope` is a plain string on `Translation`. If the FE needs a list of available scopes, use:

```csharp
// Query: ListScopesQuery
var scopes = await session
    .Query<Translation>()
    .Select(t => t.Scope)
    .Distinct()
    .ToListAsync(cancellationToken);
```

This returns e.g. `["nexti-pageNotFound", "auth-api-authService", "lang-settings"]` — no dedicated table needed.

### Get translations by scope (main FE query)

**GetTranslationsByScopeQueryHandler** — returns all translations for a scope + language:

```csharp
internal class GetTranslationsByScopeQueryHandler(
    ILangReadOnlyEventStoreRepository readOnlyRepository)
    : IRequestHandler<GetTranslationsByScopeQuery, ErrorOr<GetTranslationsByScopeResponse>>
{
    public async Task<ErrorOr<GetTranslationsByScopeResponse>> Handle(
        GetTranslationsByScopeQuery query, CancellationToken cancellationToken)
    {
        var translations = await readOnlyRepository.GetFilteredAsync<Translation>(
            t => t.Scope == query.Scope && t.Language == query.Language,
            cancellationToken);

        var items = translations.Select(t => new TranslationItem(t.Name, t.Value)).ToList();
        return new GetTranslationsByScopeResponse(items);
    }
}
```

**Response:**

```json
// GET /api/lang/translations/scope/nexti-pageNotFound  (language "cz" from header)
[
    { "name": "pageNotFound", "value": "Stranka nenalezena" },
    { "name": "dalsiPriklad", "value": "Dalsi priklad" }
]
```

### Translation CRUD

**CreateTranslationCommandHandler** — creates a translation and publishes MassTransit event for other languages:

```csharp
internal class CreateTranslationCommandHandler(
    ILangReadOnlyEventStoreRepository readOnlyRepository,
    ILangUnitOfWork unitOfWork,
    ILangPublisher langPublisher,
    IClock clock) : IRequestHandler<CreateTranslationCommand, ErrorOr<Success>>
{
    public async Task<ErrorOr<Success>> Handle(
        CreateTranslationCommand command, CancellationToken cancellationToken)
    {
        var key = $"{command.Request.Scope}-{command.Request.Name}";

        var existing = await readOnlyRepository.GetFirstByConditionAsync<Translation>(
            t => t.Key == key && t.Language == command.Language,
            cancellationToken);

        if (existing is not null)
            return Result.Success;  // idempotent — already exists

        var translation = Translation.CreateFrom(
            command.Request.Name, command.Request.Scope,
            command.Request.Value, command.Language, clock.UtcNowOffset);

        if (translation.IsError) return translation.Errors;

        var createdEvent = new TranslationCreatedEvent(
            translation.Value.Id, translation.Value.Name, translation.Value.Scope,
            translation.Value.Key, translation.Value.Value, translation.Value.Language,
            clock.UtcNowOffset);

        unitOfWork.StartStream<TranslationCreatedEvent, Translation>(translation.Value.Id, createdEvent);

        // Publish MassTransit event — consumer will create for other languages
        await langPublisher.PublishTranslationCreated(
            translation.Value.Scope, translation.Value.Name,
            translation.Value.Value, translation.Value.Language, cancellationToken);

        return Result.Success;
    }
}
```

**Request (from FE):**

```json
// POST /api/lang/translations  (language "cz" from header)
{
    "scope": "nexti-pageNotFound",
    "name": "pageNotFound",
    "value": "Stranka nenalezena"
}
```

**UpdateTranslationCommandHandler** — updates value only (no MassTransit, per-language manual update):

```csharp
// After appending TranslationUpdatedEvent — no cross-language propagation
// Translators update each language individually via admin panel
```

**RemoveTranslationCommandHandler** — deletes a single translation by ID.

---

## Step 5: MassTransit Event-Driven Multi-Language Creation

When FE (or seed) creates a translation for one language, the consumer automatically creates translations for **all other enabled languages** for the tenant. This ensures every scope has translations in every active language.

### MassTransit message contract

**New file**: `Nexticz.Module.Lang.Contracts/Translations/TranslationCreated.cs`

```csharp
namespace Nexticz.Module.Lang.Contracts.Translations;

public record TranslationCreated(string Scope, string Name, string Value, string Language);
```

Published when a translation is created via `CreateTranslationCommandHandler`.

### Publisher

**New file**: `Application/MassTransitPublishers/ILangPublisher.cs`

```csharp
using Nexticz.Lib.Shared.MessagePublishers;

namespace Nexticz.Module.Lang.Application.MassTransitPublishers;

internal interface ILangPublisher : IBaseMessagePublisher
{
    Task PublishTranslationCreated(string scope, string name, string value, string language,
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
    public async Task PublishTranslationCreated(string scope, string name, string value, string language,
        CancellationToken cancellationToken)
    {
        await PublishAsync(new TranslationCreated(scope, name, value, language), cancellationToken);
    }
}
```

Register in DI: `services.AddScoped<ILangPublisher, LangPublisher>();`

### Consumer — creates translations for other enabled languages

**New file**: `Infrastructure/Consumers/TranslationCreatedConsumer.cs`

```csharp
using MassTransit;
using Microsoft.Extensions.Logging;
using Nexticz.Module.Lang.Contracts.Translations;

namespace Nexticz.Module.Lang.Infrastructure.Consumers;

internal class TranslationCreatedConsumer(
    ILogger<TranslationCreatedConsumer> logger,
    ILangDocumentStore documentStore,
    ILangUnitOfWork unitOfWork,
    ILangReadOnlyEventStoreRepository readOnlyRepository) : IConsumer<TranslationCreated>
{
    public async Task Consume(ConsumeContext<TranslationCreated> context)
    {
        var msg = context.Message;

        logger.LogInformation(
            "TranslationCreated received: Scope={Scope}, Name={Name}, Language={Language}",
            msg.Scope, msg.Name, msg.Language);

        // 1. Get all enabled languages for the tenant
        await using var session = documentStore.QuerySession();
        var enabledLanguages = await session
            .Query<Domain.Languages.Language>()
            .Where(l => l.IsActive)
            .Select(l => l.Shortcut)
            .ToListAsync(context.CancellationToken);

        // 2. For each language OTHER than the one that was just created
        foreach (var language in enabledLanguages.Where(l => l != msg.Language))
        {
            // 3. Check if translation already exists (idempotent)
            var key = $"{msg.Scope}-{msg.Name}";
            var existing = await session
                .Query<Domain.Translations.Translation>()
                .FirstOrDefaultAsync(t => t.Key == key && t.Language == language,
                    context.CancellationToken);

            if (existing is not null)
            {
                logger.LogTrace("Translation already exists: Key={Key}, Language={Language}", key, language);
                continue;
            }

            // 4. Create translation for this language with the same value (to be translated later)
            var translation = Translation.Create(msg.Name, msg.Scope, msg.Value, language,
                DateTimeOffset.UtcNow);

            unitOfWork.AppendEvents(translation);
            await unitOfWork.SaveChangesAsync(context.CancellationToken);

            logger.LogInformation("Created translation: Key={Key}, Language={Language}", key, language);
        }
    }
}
```

> **Note**: The consumer creates translations with the **same value** (e.g. Czech text) for all other languages. Translators then update each language individually via the admin panel.

**New file**: `Infrastructure/Consumers/TranslationCreatedConsumerDefinition.cs`

```csharp
using MassTransit;

namespace Nexticz.Module.Lang.Infrastructure.Consumers;

internal class TranslationCreatedConsumerDefinition : ConsumerDefinition<TranslationCreatedConsumer>
{
    private static string QueuePrefix => MassTransitModulePrefixName.ModulePrefixName;

    public TranslationCreatedConsumerDefinition()
    {
        EndpointName = $"{QueuePrefix}-{KebabCaseEndpointNameFormatter.Instance.Consumer<TranslationCreatedConsumer>()}";
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
        cfg.AddConsumer<TranslationCreatedConsumer, TranslationCreatedConsumerDefinition>();
    }
}
```

### Flow diagram

```
CreateTranslationCommandHandler     RabbitMQ                    TranslationCreatedConsumer
┌──────────────────┐     publish    ┌──────────────┐    consume   ┌─────────────────────────────┐
│ FE POST or Seed  │ ──────────→    │ Translation  │ ──────────→  │ TranslationCreatedConsumer  │
│                  │                │ Created      │              │                             │
│ 1. Create transl │                │ {Scope,      │              │ 1. Get enabled languages    │
│    for CZ        │                │  Name,       │              │ 2. For each OTHER language: │
│ 2. Append event  │                │  Value,      │              │    a. Check if Key+Lang     │
│ 3. Publish msg   │                │  Language}   │              │       already exists (skip) │
└──────────────────┘                └──────────────┘              │    b. Create translation    │
                                                                  │       with same Value       │
                                                                  │    c. Append + save events  │
                                                                  └─────────────────────────────┘
```

---

## Step 6: FE Integration — API Endpoint + Auto-Create

There are **no static JSON files**. FE fetches translations directly from the API by scope and auto-creates any missing translations via POST.

### API error response format (ErrorOr → ApiErrorResponse)

This is the end-to-end flow from a domain error to a translated message displayed to the user. Understanding this is critical — the `slug` in the API response is the **same Key** used to look up translations by scope.

#### 1. Domain: Define errors with `IErrorComponentSlugProvider`

Each module defines error classes that implement `IErrorComponentSlugProvider`. The `ComponentSlug` follows the pattern `{module}-api-{service}-`:

```csharp
public abstract class LanguageErrors : IErrorComponentSlugProvider
{
    public static string ComponentSlug { get; } = "lang-api-languageService-";

    public static Error LanguageWithShortcutDoesnotExist => Error.Validation(
        code: ComponentSlug + "languageWithShortcutDoesnotExist",
        description: "Tento jazyk neexistuje");   // Czech hardcoded fallback
}
```

- `code` = full Key, e.g. `"lang-api-languageService-languageWithShortcutDoesnotExist"`
- `description` = Czech default text (always present, used as fallback)

How this maps to Translation fields:

| Error property | Translation field | Example |
|---|---|---|
| `ComponentSlug` (minus trailing `-`) | `Scope` | `lang-api-languageService` |
| Error property name | `Name` | `languageWithShortcutDoesnotExist` |
| `Error.Code` | `Key` (= Scope-Name) | `lang-api-languageService-languageWithShortcutDoesnotExist` |
| `Error.Description` | `Value` (Czech default) | `Tento jazyk neexistuje` |

#### 2. Application: Handler returns `ErrorOr<T>`

```csharp
var language = await readOnlyRepository.GetFirstByConditionAsync<Language>(...);
if (language is null)
    return LanguageErrors.LanguageWithShortcutDoesnotExist;  // returns Error
```

#### 3. Presentation: Endpoint maps to HTTP response

```csharp
return result.Match(
    Results.Ok,          // success → 200
    ResultsHelper.Problem);  // error → 422/404/401
```

`ResultsHelper.Problem` maps `ErrorType` to HTTP status:

| ErrorType | HTTP Status |
|-----------|-------------|
| `Validation` | 422 Unprocessable Entity |
| `Failure` | 422 Unprocessable Entity |
| `NotFound` | 404 Not Found |
| `Unauthorized` | 401 Unauthorized |

#### 4. Error mapping: `ErrorOrExtension.MapToErrorResponse()`

The `Error` is transformed into `ApiErrorResponse` via the shared extension:

```csharp
public static ApiErrorResponse MapToErrorResponse(this Error error)
{
    return new ApiErrorResponse
    {
        CorrelationId = CorrelationIdProvider.Instance.GetInternalId(),
        Errors = [new ApiError { Message = error.Description, Slug = error.Code }]
    };
}
```

#### 5. API response received by FE

```json
{
    "correlationId": "aB3xK9m",
    "errors": [
        {
            "slug": "lang-api-languageService-languageWithShortcutDoesnotExist",
            "message": "Tento jazyk neexistuje"
        }
    ]
}
```

| Field | Source | Purpose |
|-------|--------|---------|
| `slug` | `Error.Code` (= Key = Scope-Name) | **Key** for translation lookup |
| `message` | `Error.Description` (Czech hardcoded) | **Fallback** if not found in loaded translations |
| `correlationId` | `CorrelationIdProvider` | Request tracing / debugging |

#### End-to-end error flow diagram

```
Domain                    Application              Endpoint                 FE
┌────────────────┐       ┌──────────────┐        ┌──────────────┐        ┌─────────────────────┐
│ LanguageErrors │       │ Handler      │        │ result.Match │        │ handleApiError()    │
│                │       │              │        │              │        │                     │
│ ComponentSlug  │──→    │ return       │──→     │ ResultsHelper│──→     │ error.slug →        │
│ + error name   │       │ Error        │        │ .Problem()   │        │ lookup by scope     │
│                │       │              │        │              │        │                     │
│ code: "lang-   │       │ ErrorOr<T>   │        │ 422 + JSON:  │        │ found? → translated │
│  api-langSvc-  │       │              │        │ { slug,      │        │ not found? →        │
│  doesNotExist" │       │              │        │   message,   │        │  1. show fallback   │
│                │       │              │        │   corrId }   │        │  2. POST to create  │
│ desc: "Tento   │       │              │        │              │        │                     │
│  jazyk..."     │       │              │        │              │        │                     │
└────────────────┘       └──────────────┘        └──────────────┘        └─────────────────────┘
```

### FE flow — fetch translations by scope

```
1. FE needs translations for a page/feature
   → GET /api/lang/translations/scope/{scope}
   → Language is resolved from request header (Accept-Language or custom header)
   → Response: [{ "name": "pageNotFound", "value": "Stránka nenalezena" }, ...]

2. FE stores translations in memory / state manager keyed by scope

3. FE loops through all translation keys it needs for the current view
   → If found in loaded translations → use it
   → If NOT found → show default value AND fire-and-forget POST to auto-create
```

### FE flow — auto-create missing translations

When FE encounters a translation key that doesn't exist in the loaded scope, it auto-creates it so it appears for translators in the admin panel:

```typescript
// pseudocode
function resolveTranslation(scope: string, name: string, defaultValue: string): string {
    const translations = getLoadedTranslations(scope);  // from GET /scope/{scope}

    const found = translations.find(t => t.name === name);
    if (found) {
        return found.value;  // ✓ translated message
    }

    // Translation missing — show default immediately, create in background
    createMissingTranslation(scope, name, defaultValue);  // fire-and-forget
    return defaultValue;  // show default while translation is being created
}

async function createMissingTranslation(scope: string, name: string, defaultValue: string): Promise<void> {
    // Language is resolved from request header — FE doesn't send it in body
    await fetch('/api/lang/translations', {
        method: 'POST',
        body: JSON.stringify({ scope, name, value: defaultValue })
    });
    // POST creates translation for current language → publishes MassTransit event
    // → consumer creates translations for all other enabled languages
}
```

### FE flow — handle API error responses

```typescript
function handleApiError(response: ApiErrorResponse): string[] {
    return response.errors.map(error => {
        // Derive scope from slug: "lang-api-languageService-doesNotExist"
        // → scope = "lang-api-languageService" (ComponentSlug minus trailing dash)
        // → name = "doesNotExist"
        const lastDash = error.slug.lastIndexOf('-');
        const scope = error.slug.substring(0, lastDash);
        const name = error.slug.substring(lastDash + 1);

        const translations = getLoadedTranslations(scope);
        const found = translations.find(t => t.name === name);

        if (found) {
            return found.value;  // ✓ translated error message
        }

        // Slug not in loaded translations — show Czech fallback, auto-create in background
        createMissingTranslation(scope, name, error.message);
        return error.message;  // Czech fallback from ErrorOr description
    });
}
```

**Important considerations:**

- The `POST` endpoint is **idempotent** — if Key+Language already exists, the handler skips it
- The FE should **debounce** creation requests — batch multiple missing names per scope
- The `value` sent is the default text — translators update it to correct translations via the admin panel
- Language comes from the **request header** — FE does not send language in the body
- The FE does **NOT wait** for the POST response — it shows the default value immediately

### Flow diagram — FE translation lifecycle

```
FE Page Load                         Backend
┌───────────────────┐
│ 1. GET /api/lang/ │ ───────────→   Query Marten: WHERE Scope = X AND Language = header-lang
│    translations/  │ ←───────────   [{ name, value }, ...]
│    scope/{scope}  │
│ 2. Store in memory│
│ 3. Loop through   │
│    needed keys    │
│                   │
│ ALL FOUND → done  │
│                   │
│ MISSING KEY:      │
│  1. Show default  │
│  2. POST /api/    │ ───────────→   Creates translation → publishes TranslationCreated
│     lang/         │                   → consumer creates for other languages
│     translations  │
│  3. Next page     │ ───────────→   GET scope → now includes the new translation
│     load picks    │
│     up new value  │
└───────────────────┘

FE API Error Flow                    Backend
┌───────────────────┐
│ API call fails    │ ←──────────    { slug: "auth-...-invalidPw", message: "Neplatné heslo" }
│ Derive scope+name │
│ from error.slug   │
│                   │
│ FOUND → show msg  │
│                   │
│ NOT FOUND:        │
│  1. Show fallback │
│     (error.msg)   │
│  2. POST /api/    │ ───────────→   Creates translation → publishes TranslationCreated
│     lang/         │                   → consumer creates for other languages
│     translations  │
└───────────────────┘
```

### Key design principle

The **Key is the bridge** between the backend error system and the translation system:

1. Backend defines errors with Keys via `IErrorComponentSlugProvider` (`Key` = `ComponentSlug` + error name)
2. Seed scans all `IErrorComponentSlugProvider` implementations → creates translations in Marten with those Keys
3. FE fetches translations by scope, receives API errors with slug = Key, looks them up
4. If a Key is missing (new error added by developer, not yet seeded), the FE auto-creates it
5. MassTransit consumer creates translations for all other enabled languages

---

## Step 7: Remove Google Translate API

1. **Delete** `Application/Languages/Configurations/GoogleApisSettings.cs`
2. **Remove** Google Translate logic from `UpdateLanguageCommandHandler` — handler now only toggles `IsActive`
3. **Remove** `Google.Cloud.Translation.V2` package reference from csproj
4. **Remove** `GoogleApisSettings` section from `appsettings.*.json`

Translations for non-Czech languages are entered manually via the Translation CRUD endpoints.

---

## Step 8: Reflection-Based Seed (Unified)

Single `TranslationsSeed` in the Lang module scans **all loaded assemblies** for `IErrorComponentSlugProvider` and `ITranslatable`:

```csharp
public class TranslationsSeed
{
    public async Task RunSeed(IServiceProvider serviceProvider)
    {
        var mediatr = serviceProvider.GetRequiredService<ISender>();
        var logger = serviceProvider.GetRequiredService<ILogger<TranslationsSeed>>();

        // 1. Scan for IErrorComponentSlugProvider
        var errorTypes = AppDomain.CurrentDomain.GetAssemblies()
            .SelectMany(a => a.GetTypes())
            .Where(t => typeof(IErrorComponentSlugProvider).IsAssignableFrom(t) && !t.IsInterface);

        foreach (var errorType in errorTypes)
        {
            // ComponentSlug = "lang-api-languageService-"
            // Scope = ComponentSlug minus trailing dash = "lang-api-languageService"
            var slug = (string)errorType.GetProperty("ComponentSlug")!.GetValue(null)!;
            var scope = slug.TrimEnd('-');

            var errorProps = errorType.GetProperties(BindingFlags.Public | BindingFlags.Static)
                .Where(p => p.PropertyType == typeof(Error));

            foreach (var prop in errorProps)
            {
                var error = (Error)prop.GetValue(null)!;
                // Name = error property name (e.g. "languageWithShortcutDoesnotExist")
                // Value = error.Description (Czech default)
                await mediatr.Send(new CreateTranslationCommand(
                    Scope: scope,
                    Name: prop.Name,       // camelCase property name
                    Value: error.Description));
            }
        }

        // 2. Scan for static ITranslatable properties
        var translatableTypes = AppDomain.CurrentDomain.GetAssemblies()
            .SelectMany(a => a.GetTypes())
            .Where(t => t.GetProperties(BindingFlags.Public | BindingFlags.Static)
                .Any(p => typeof(ITranslatable).IsAssignableFrom(p.PropertyType)));

        foreach (var type in translatableTypes)
        {
            // Extract scope and name from ITranslatable, create via MediatR
            // ...
        }
    }
}
```

Each `CreateTranslationCommand` is **idempotent** (skips if Key+Language exists) and publishes `TranslationCreated` → the MassTransit consumer creates translations for all other enabled languages.

### Remove per-module seeds

1. **Remove** `Auth/.../Seeds/TranslationsSeed.cs`
2. **Remove** `Auth/.../Interfaces/ILangApiService.cs` and its HTTP-based seeding
3. **Remove** translation seed calls from Auth's `DatabaseInitializer`
4. All seeding is now centralized in the Lang module

---

## Step 9: API Endpoints

### Language endpoints

| Method   | Route                            | Auth       | Description          |
|----------|----------------------------------|------------|----------------------|
| `GET`    | `/api/lang/languages`            | Anonymous  | List languages       |
| `GET`    | `/api/lang/languages/{shortcut}` | Authorized | Get by shortcut      |
| `PUT`    | `/api/lang/languages/{shortcut}` | Authorized | Toggle active        |

### Scopes endpoint (derived from translations, no CRUD)

| Method | Route                    | Auth       | Description                              |
|--------|--------------------------|------------|------------------------------------------|
| `GET`  | `/api/lang/scopes`       | Authorized | List distinct scopes from translations   |

### Translation endpoints

| Method   | Route                                    | Auth       | Description                                |
|----------|------------------------------------------|------------|--------------------------------------------|
| `GET`    | `/api/lang/translations`                 | Authorized | List all translations (admin panel)        |
| `GET`    | `/api/lang/translations/{id}`            | Authorized | Get by ID (admin panel)                    |
| `GET`    | `/api/lang/translations/scope/{scope}`   | Anonymous  | Get translations by scope (FE primary endpoint, language from header) |
| `POST`   | `/api/lang/translations`                 | Anonymous  | Create single translation (FE auto-create, language from header) |
| `PUT`    | `/api/lang/translations/{id}`            | Authorized | Update value (admin panel)                 |
| `DELETE` | `/api/lang/translations/{id}`            | Authorized | Remove translation (admin panel)           |

---

## Summary: Files to Create / Modify / Delete

> All paths under **Create** are relative to the new `Modules/Lang/Nexticz.Module.Lang/` project.
> The old 5-project module (`Nexticz.Module.Lang.Presentation`, `.Application`, `.Infrastructure`, `.Domain`) stays **untouched**.

### Create (new `Nexticz.Module.Lang` project)

| File | Layer | Purpose |
|------|-------|---------|
| `Nexticz.Module.Lang.csproj` | Project | Single project; refs `Nexticz.Lib.Shared` + `Nexticz.Module.Lang.Contracts` |
| `ServiceCollectionExtensions.cs` | Root | `AddLangModuleV2()` / `UseLangModuleV2Async()` with `LangV2IsEnabled` flag |
| `ModuleName.cs` | Root | Module name enum value |
| `ModuleNameProvider.cs` | Root | Module name constant |
| `Domain/AggregateRoot.cs` | Domain | Module-level base class |
| `Domain/Languages/Language.cs` | Domain | Event-sourced aggregate |
| `Domain/Languages/Events/*.cs` | Domain | Created, Updated, Deleted events |
| `Domain/Languages/LanguageErrors.cs` | Domain | ErrorOr error definitions |
| `Domain/Translations/Translation.cs` | Domain | Event-sourced aggregate (Name, Scope, Key, Value, Language) |
| `Domain/Translations/Events/*.cs` | Domain | Created, Updated, Deleted events |
| `Domain/Translations/TranslationErrors.cs` | Domain | ErrorOr error definitions |
| `Application/ApplicationServiceCollectionExtensions.cs` | Application | `AddApplicationLayer()` |
| `Application/ILangCommand.cs` | Application | Command marker interface |
| `Application/Interfaces/ILangDocumentSessionProvider.cs` | Application | Marten session provider |
| `Application/Interfaces/ILangUnitOfWork.cs` | Application | Marten UoW |
| `Application/Interfaces/ILangReadOnlyEventStoreRepository.cs` | Application | Marten read-only repo |
| `Application/PipelineBehaviors/LangPostCommandBehavior.cs` | Application | Auto-save behavior |
| `Application/MassTransitPublishers/ILangPublisher.cs` | Application | Publisher interface |
| `Application/MassTransitPublishers/LangPublisher.cs` | Application | Publisher implementation |
| `Application/Languages/Commands/*.cs` | Application | Create, Update, Delete language handlers |
| `Application/Languages/Queries/*.cs` | Application | Get, GetAll language handlers |
| `Application/Translations/Commands/*.cs` | Application | Create, Update, Delete translation handlers |
| `Application/Translations/Queries/GetTranslationsByScopeQuery.cs` | Application | FE primary query |
| `Infrastructure/InfrastructureServiceCollectionExtensions.cs` | Infrastructure | `AddInfrastructureLayer()` / `UseInfrastructureLayerAsync()` |
| `Infrastructure/ILangDocumentStore.cs` | Infrastructure | Marten store marker |
| `Infrastructure/BaseRepositories/LangDocumentSessionProvider.cs` | Infrastructure | Session provider |
| `Infrastructure/BaseRepositories/LangUnitOfWork.cs` | Infrastructure | UoW implementation |
| `Infrastructure/BaseRepositories/LangReadOnlyEventStoreRepository.cs` | Infrastructure | Read-only repo |
| `Infrastructure/Languages/LanguageProjection.cs` | Infrastructure | Marten projection |
| `Infrastructure/Languages/LanguageConfigurator.cs` | Infrastructure | Marten schema config |
| `Infrastructure/Translations/TranslationProjection.cs` | Infrastructure | Marten projection |
| `Infrastructure/Translations/TranslationConfigurator.cs` | Infrastructure | Marten schema config |
| `Infrastructure/Consumers/TranslationCreatedConsumer.cs` | Infrastructure | MassTransit consumer — creates for other languages |
| `Infrastructure/Consumers/TranslationCreatedConsumerDefinition.cs` | Infrastructure | Consumer config |
| `Infrastructure/MassTransitModulePrefixName.cs` | Infrastructure | Queue prefix |
| `Infrastructure/MassTransitRegistrator.cs` | Infrastructure | Consumer registration |
| `Infrastructure/Dbs/Seeds/TranslationsSeed.cs` | Infrastructure | Reflection-based seed |

### Create (shared Contracts — `Nexticz.Module.Lang.Contracts`)

| File | Layer | Purpose |
|------|-------|---------|
| `Translations/TranslationCreated.cs` | Contracts | MassTransit message (used by new module, shared project) |
| `Translations/CreateTranslationRequest.cs` | Contracts | API request: `{ Scope, Name, Value }` (language from header) |

### Modify (existing files outside the new project)

| File | Change |
|------|--------|
| `Nexticz.Module.Lang.Presentation/DependencyInjection.cs` | Add inverse feature flag guard — skip when `LangV2IsEnabled = true` |
| `Nexticz.Nexty.Jipocar.Api/Program.cs` | Add `AddLangModuleV2()` + `await UseLangModuleV2Async()` calls |
| `Nexticz.Nexty.Jipocar.Api/Nexticz.Nexty.Jipocar.Api.csproj` | Add project reference to `Nexticz.Module.Lang.csproj` |
| `appsettings.*.json` | Add `LangV2IsEnabled` feature flag + `Lang` PostgreSQL connection string |

### Delete (deferred — when `LangV2IsEnabled` is proven stable)

Nothing is deleted during initial rollout. Once the V2 module is stable and the feature flag is permanently enabled:

| What to remove | Reason |
|----------------|--------|
| `Nexticz.Module.Lang.Presentation/` project | Replaced by new module endpoints |
| `Nexticz.Module.Lang.Application/` project | Replaced by new module application layer |
| `Nexticz.Module.Lang.Infrastructure/` project | Replaced by new module Marten infrastructure |
| `Nexticz.Module.Lang.Domain/` project | Replaced by new module domain |
| `LangV2IsEnabled` feature flag | No longer needed — rename `AddLangModuleV2` → `AddLangModule` |
| Inverse guard in old `DependencyInjection.cs` | Old module removed entirely |
| Old project reference in `Jipocar.Api.csproj` | Only new module reference kept |
| `Google.Cloud.Translation.V2` package reference | Google Translate dependency removed |
| `Microsoft.EntityFrameworkCore.SqlServer` package (if Lang was the last user) | EF Core no longer needed for Lang |
