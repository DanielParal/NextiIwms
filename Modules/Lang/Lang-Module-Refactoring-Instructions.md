# Lang Module Refactoring Instructions

## Goals

1. Introduce a **Section** entity — translations belong to a section, JSON files are generated per section
2. Two complete CRUD systems: **Language** and **Translation** (with Section management)
3. **Remove Google Translate API** dependency entirely
4. **Reflection-based seed** that scans all assemblies for `IErrorComponentSlugProvider` and `ITranslatable` implementations
5. **Static JSON file generation** — when a translation within a section changes, regenerate that section's JSON file
6. **Serve JSON files** to the FE via a dedicated anonymous endpoint

---

## Step 1: Add Section Entity

### Domain

**New file**: `Nexticz.Module.Lang.Domain/Sections/Section.cs`

```csharp
namespace Nexticz.Module.Lang.Domain.Sections;

public class Section
{
    public Guid Id { get; set; }
    public required string Name { get; set; }       // e.g. "auth", "lang", "shared", "mmo-settings"
    public required string Slug { get; set; }       // unique key, e.g. "auth", "lang", "shared"
    public DateTime Created { get; set; } = DateTime.UtcNow;
    public DateTime? Updated { get; set; }
    public ICollection<Translation>? Translations { get; set; }
}
```

**New file**: `Nexticz.Module.Lang.Domain/Sections/SectionErrors.cs`

```csharp
public abstract class SectionErrors : IErrorComponentSlugProvider
{
    public static string ComponentSlug { get; } = "lang-api-sectionService-";

    public static Error SectionWithIdDoesNotExist => Error.Validation(
        code: ComponentSlug + "sectionWithIdDoesNotExist",
        description: "Tato sekce neexistuje");

    public static Error SectionWithSlugAlreadyExists => Error.Validation(
        code: ComponentSlug + "sectionWithSlugAlreadyExists",
        description: "Sekce s tímto klíčem již existuje");

    public static Error CreateSectionError => Error.Validation(
        code: ComponentSlug + "createSectionError",
        description: "Sekci se nepodařilo vytvořit");
}
```

### Modify Translation Entity

Add a FK to Section. The existing `Module` field on Translation can be mapped to a Section during migration.

```csharp
// Translation.cs — add these properties
public Guid SectionId { get; set; }
public Section? Section { get; set; }
```

### EF Configuration

**New file**: `Nexticz.Module.Lang.Infrastructure/Sections/Persistance/SectionConfigurations.cs`

```csharp
public class SectionConfigurations : IEntityTypeConfiguration<Section>
{
    public void Configure(EntityTypeBuilder<Section> builder)
    {
        builder.ToTable("Sections");
        builder.Property(x => x.Name).HasMaxLength(100);
        builder.Property(x => x.Slug).HasMaxLength(100);
        builder.HasIndex(x => x.Slug).IsUnique();
        builder
            .HasMany(x => x.Translations)
            .WithOne(x => x.Section)
            .HasForeignKey(x => x.SectionId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
```

**Modify**: `TranslationConfigurations.cs` — add `SectionId` column config.

**Modify**: `DataContext.cs` — add `DbSet<Section> Sections { get; set; }`.

### Migration

Create a new EF Core migration that:
1. Creates the `Sections` table
2. Seeds initial sections from distinct `Module` values already in the `Translations` table
3. Adds `SectionId` FK to `Translations`
4. Populates `SectionId` from matching `Module` values

---

## Step 2: Section CRUD

Follow the existing pattern (Language CRUD as reference).

### Contracts

**New file**: `Nexticz.Module.Lang.Contracts/Sections/SectionResponse.cs`

```csharp
public class SectionResponse
{
    public required Guid Id { get; set; }
    public required string Name { get; set; }
    public required string Slug { get; set; }
    public DateTime? Created { get; set; }
    public DateTime? Updated { get; set; }
}
```

**New file**: `Nexticz.Module.Lang.Contracts/Sections/CreateSectionRequest.cs`

```csharp
public class CreateSectionRequest
{
    public required string Name { get; set; }
    public required string Slug { get; set; }
}
```

**New file**: `Nexticz.Module.Lang.Contracts/Sections/UpdateSectionRequest.cs`

```csharp
public class UpdateSectionRequest
{
    public required string Name { get; set; }
}
```

### Application (CQRS)

Create these handlers following the existing pattern:

| Type    | Name                    | Description                       |
|---------|-------------------------|-----------------------------------|
| Command | `CreateSectionCommand`  | Creates a new section             |
| Command | `UpdateSectionCommand`  | Updates section name              |
| Command | `RemoveSectionCommand`  | Deletes section (if no translations) |
| Query   | `GetSectionByIdQuery`   | Get single section                |
| Query   | `ListSectionsQuery`     | List sections with DevExtreme filtering |

### Repository

**New file**: `ISectionsRepository.cs` in Application/Common/Interfaces

```csharp
public interface ISectionsRepository
{
    Task<Section?> GetSectionByIdAsync(Guid id, CancellationToken cancellationToken);
    Task<Section?> GetSectionBySlugAsync(string slug, CancellationToken cancellationToken);
    Task<FilteredResult> ListFilteredSectionsAsync(SectionsFilteringParams filteringParams, CancellationToken cancellationToken);
}
```

Add `ISectionsRepository SectionRepository` to `IUnitOfWork`.

### Endpoints

**New route group** in `ApiEndpoints.cs`:

```csharp
public static class Sections
{
    private const string Base = $"{ApiBase}/sections";

    public const string GetSections = $"{Base}";
    public const string GetSectionById = $"{Base}/{{id}}";
    public const string CreateSection = $"{Base}";
    public const string UpdateSection = $"{Base}/{{id}}";
    public const string RemoveSection = $"{Base}/{{id}}";
}
```

Create endpoint files in `Endpoints/Sections/` following the same pattern as Languages/Translations.

Register in `EndpointsExtensions.cs` alongside Languages and Translations.

### Final API surface for Sections

| Method   | Route                        | Auth       | Description        |
|----------|------------------------------|------------|--------------------|
| `GET`    | `/api/lang/sections`         | Authorized | List sections      |
| `GET`    | `/api/lang/sections/{id}`    | Authorized | Get section by ID  |
| `POST`   | `/api/lang/sections`         | Authorized | Create section     |
| `PUT`    | `/api/lang/sections/{id}`    | Authorized | Update section     |
| `DELETE` | `/api/lang/sections/{id}`    | Authorized | Remove section     |

---

## Step 3: Modify Translation CRUD

### Assign translation to section

**Modify** `CreateTranslationsRequest` — add `SectionSlug`:

```csharp
public class CreateTranslationsRequest
{
    public required List<CreateTranslationsItem> Items { get; set; }
    public required EnumHelper.LanguageShortcutEnum LanguageShortcut { get; set; }
    public required string SectionSlug { get; set; }  // NEW
}
```

**Modify** `CreateTranslationsCommandHandler`:
1. Resolve `Section` by `SectionSlug` at the start
2. Set `SectionId` on each new `Translation` entity
3. After saving, trigger JSON file regeneration for this section + language

**Modify** `UpdateTranslationCommandHandler`:
1. After updating the value, get the translation's `SectionId`
2. Trigger JSON file regeneration for that section + the translation's `LanguageShortcut`

**Modify** `RemoveTranslationCommandHandler`:
1. Before removing, capture `SectionId` and `LanguageShortcut`
2. After removing, trigger JSON file regeneration for that section + language

### Keep existing Translation fields

The `Module`, `Feature`, `Component`, `Name`, `Slug` fields stay — they are still useful for error slug parsing. The `SectionId` is the new grouping mechanism for JSON file generation.

---

## Step 4: Remove Google Translate API

### Files to modify

1. **Delete** `Nexticz.Module.Lang.Application/Languages/Configurations/GoogleApisSettings.cs`

2. **Modify** `UpdateLanguageCommandHandler.cs` — remove all Google Translate logic:

```csharp
public class UpdateLanguageCommandHandler(IUnitOfWork unitOfWork)
    : IRequestHandler<UpdateLanguageCommand, ErrorOr<Updated>>
{
    public async Task<ErrorOr<Updated>> Handle(UpdateLanguageCommand command, CancellationToken cancellationToken)
    {
        var language = await unitOfWork.LanguageRepository
            .GetLanguageByShortcutAsync(command.Shortcut, cancellationToken);

        if (language is null)
            return LanguageErrors.LanguageWithShortcutDoesnotExist;

        language.IsActive = command.UpdateLanguageRequest.IsActive;
        unitOfWork.Update(language);
        var result = await unitOfWork.CompleteAsync(cancellationToken);

        if (!result)
            return LanguageErrors.UpdateLanguageError;

        return Result.Updated;
    }
}
```

Remove `IConfiguration`, `ILogger`, `Google.Cloud.Translation.V2` imports. The handler now only toggles `IsActive` — no auto-translation.

3. **Remove** `Google.Cloud.Translation.V2` package reference from `Nexticz.Lib.Shared.csproj`

4. **Remove** `GoogleApisSettings` section from `appsettings.*.json`

### Translation for other languages

Without Google Translate, translations for non-Czech languages must be entered manually through the Translation CRUD endpoints. The admin UI should allow selecting a language when creating/editing a translation.

---

## Step 5: Reflection-Based Seed (Unified)

### Goal

Replace the per-module `TranslationsSeed` classes with a single unified seed in the Lang module that scans **all loaded assemblies** for both `IErrorComponentSlugProvider` and `ITranslatable` implementations.

### New file: `Nexticz.Module.Lang.Infrastructure/Common/Persistence/Initialization/Seeds/TranslationsSeed.cs` (rewrite)

```csharp
public class TranslationsSeed
{
    private readonly IServiceProvider _serviceProvider;

    public TranslationsSeed(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }

    public async Task RunSeed()
    {
        var mediatr = _serviceProvider.GetRequiredService<ISender>();
        var logger = _serviceProvider.GetRequiredService<ILogger<TranslationsSeed>>();

        var itemsToSeed = new List<CreateTranslationsItem>();

        // 1. Scan ALL loaded assemblies for IErrorComponentSlugProvider
        var errorTypes = AppDomain.CurrentDomain.GetAssemblies()
            .SelectMany(a => a.GetTypes())
            .Where(t => typeof(IErrorComponentSlugProvider).IsAssignableFrom(t) && !t.IsInterface);

        foreach (var errorType in errorTypes)
        {
            itemsToSeed.AddRange(GetErrorsFromType(errorType));
        }

        // 2. Scan ALL loaded assemblies for static ITranslatable properties
        var translatableTypes = AppDomain.CurrentDomain.GetAssemblies()
            .SelectMany(a => a.GetTypes())
            .Where(t => t.GetProperties(BindingFlags.Public | BindingFlags.Static)
                .Any(p => typeof(ITranslatable).IsAssignableFrom(p.PropertyType)));

        foreach (var type in translatableTypes)
        {
            itemsToSeed.AddRange(GetTranslatablesFromType(type));
        }

        // 3. Group by section (module part of slug) and seed
        var groupedByModule = itemsToSeed.GroupBy(x => x.Slug.Split('-')[0]);

        foreach (var group in groupedByModule)
        {
            var request = new CreateTranslationsRequest
            {
                Items = group.ToList(),
                LanguageShortcut = EnumHelper.LanguageShortcutEnum.Cs,
                SectionSlug = group.Key  // section = module part of slug
            };

            var command = new CreateTranslationsCommand { CreateTranslationsRequest = request };
            var result = await mediatr.Send(command);

            if (result.IsError)
                logger.LogError("Seed error for section {Section}: {Error}", group.Key, result.FirstError.Description);
        }
    }

    private List<CreateTranslationsItem> GetErrorsFromType(Type errorType)
    {
        return errorType.GetProperties(BindingFlags.Public | BindingFlags.Static)
            .Where(p => p.PropertyType == typeof(Error))
            .Select(p => (Error)p.GetValue(null)!)
            .Select(e => new CreateTranslationsItem { Slug = e.Code, Value = e.Description })
            .ToList();
    }

    private List<CreateTranslationsItem> GetTranslatablesFromType(Type type)
    {
        return type.GetProperties(BindingFlags.Public | BindingFlags.Static)
            .Where(p => typeof(ITranslatable).IsAssignableFrom(p.PropertyType))
            .Select(p => (ITranslatable)p.GetValue(null)!)
            .Select(t => new CreateTranslationsItem { Slug = t.TranslationKey, Value = t.TranslationValue })
            .ToList();
    }
}
```

### Section auto-creation during seed

The `CreateTranslationsCommandHandler` (or a separate seed step) should auto-create sections when they don't exist yet. During seed, if a section with the given slug doesn't exist, create it with `Name = Slug` (can be renamed later via admin UI).

### Remove per-module seeds

After the unified seed is in place:

1. **Remove** `Modules/Auth/Nexticz.Module.Auth.Infrastructure/Common/Persistence/Initialization/Seeds/TranslationsSeed.cs`
2. **Remove** translation seed calls from Auth's `DatabaseInitializer`
3. **Remove** any other module-specific `TranslationsSeed` files
4. **Remove** `ILangApiService` and its HTTP-based translation seeding from Auth module
5. The Lang module's `DatabaseInitializer` now handles all translation seeding centrally

### Seed order in `DatabaseInitializer`

```csharp
public async Task RunSeed()
{
    await new LanguagesSeed(_serviceProvider).RunSeed();       // 1. Languages first
    await new SectionsSeed(_serviceProvider).RunSeed();        // 2. Sections (optional explicit seed)
    await new TranslationsSeed(_serviceProvider).RunSeed();    // 3. Translations (reflection-based)
}
```

---

## Step 6: JSON File Generation

### New service: `ITranslationFileGenerator`

**New file**: `Nexticz.Module.Lang.Application/Common/Interfaces/ITranslationFileGenerator.cs`

```csharp
public interface ITranslationFileGenerator
{
    /// Regenerate JSON file for a specific section and language
    Task RegenerateAsync(string sectionSlug, EnumHelper.LanguageShortcutEnum language, CancellationToken cancellationToken);

    /// Regenerate all JSON files for all sections and active languages
    Task RegenerateAllAsync(CancellationToken cancellationToken);
}
```

**New file**: `Nexticz.Module.Lang.Infrastructure/Translations/TranslationFileGenerator.cs`

```csharp
public class TranslationFileGenerator : ITranslationFileGenerator
{
    private readonly DataContext _context;
    private readonly string _outputPath;

    public TranslationFileGenerator(DataContext context, IWebHostEnvironment env)
    {
        _context = context;
        _outputPath = Path.Combine(env.ContentRootPath, "wwwroot", "translations");
    }

    public async Task RegenerateAsync(string sectionSlug, EnumHelper.LanguageShortcutEnum language,
        CancellationToken cancellationToken)
    {
        // 1. Query translations for this section + language
        var translations = await _context.Translations
            .Where(t => t.Section!.Slug == sectionSlug)
            .Where(t => t.LanguageShortcut == language)
            .Select(t => new { t.Slug, t.Value })
            .ToListAsync(cancellationToken);

        // 2. Build flat slug→value dictionary
        var dict = translations.ToDictionary(t => t.Slug, t => t.Value);

        // 3. Write JSON file
        var langDir = Path.Combine(_outputPath, language.ToString().ToLower());
        Directory.CreateDirectory(langDir);

        var filePath = Path.Combine(langDir, $"{sectionSlug}.json");
        var json = JsonSerializer.Serialize(dict, new JsonSerializerOptions { WriteIndented = true });
        await File.WriteAllTextAsync(filePath, json, cancellationToken);
    }

    public async Task RegenerateAllAsync(CancellationToken cancellationToken)
    {
        var activeLanguages = await _context.Languages
            .Where(l => l.IsActive)
            .Select(l => l.Shortcut)
            .ToListAsync(cancellationToken);

        var sections = await _context.Sections
            .Select(s => s.Slug)
            .ToListAsync(cancellationToken);

        foreach (var language in activeLanguages)
        {
            foreach (var section in sections)
            {
                await RegenerateAsync(section, language, cancellationToken);
            }
        }
    }
}
```

### JSON file format

Each file is a flat `slug → value` dictionary:

```json
// wwwroot/translations/cs/auth.json
{
    "auth-api-authService-invalidPassword": "Neplatné heslo",
    "auth-api-authService-userIsCurentlyLogged": "Neočekávaná chyba, byli jste odhlášeni",
    "auth-api-authService-validationTokenError": "JWT token není validní"
}
```

Using full slugs as keys makes FE lookup straightforward — the error response `slug` field maps directly to a key in the JSON file.

### Register in DI

In `Infrastructure/DependencyInjection.cs`:

```csharp
services.AddScoped<ITranslationFileGenerator, TranslationFileGenerator>();
```

### Trigger regeneration

Call `ITranslationFileGenerator.RegenerateAsync(sectionSlug, language)` in:

| Handler                            | When                                           |
|------------------------------------|-------------------------------------------------|
| `CreateTranslationsCommandHandler` | After saving new translations                   |
| `UpdateTranslationCommandHandler`  | After updating translation value                |
| `RemoveTranslationCommandHandler`  | After deleting a translation                    |
| `TranslationsSeed.RunSeed()`       | After all seed translations are saved (call `RegenerateAllAsync`) |

---

## Step 7: Serve JSON Files to FE

### Option A: Static file middleware (recommended)

Add static file serving in `UseLangModule`:

```csharp
public static void UseLangModule(this WebApplication app)
{
    // Serve translation JSON files
    app.UseStaticFiles(new StaticFileOptions
    {
        FileProvider = new PhysicalFileProvider(
            Path.Combine(app.Environment.ContentRootPath, "wwwroot", "translations")),
        RequestPath = "/translations",
        OnPrepareResponse = ctx =>
        {
            // Cache for 1 hour, revalidate with server
            ctx.Context.Response.Headers.CacheControl = "public, max-age=3600, must-revalidate";
        }
    });

    app.MapLangApiEndpoints();
    _ = app.SeedLangDatabaseAsync(app);
}
```

FE fetches: `GET /translations/cs/auth.json`

No auth required — translation files are public, read-only, and contain no sensitive data.

### Option B: Dedicated API endpoint

If static files are not preferred, add a dedicated endpoint:

**New file**: `Endpoints/Translations/GetTranslationFileEndpoint.cs`

```csharp
public static class GetTranslationFileEndpoint
{
    public static IEndpointRouteBuilder MapGetTranslationFile(this IEndpointRouteBuilder builder)
    {
        builder.MapGet("/api/lang/translations/{language}/{section}.json",
                async (string language, string section, ITranslationFileGenerator generator,
                    CancellationToken cancellationToken) =>
                {
                    var filePath = Path.Combine("wwwroot", "translations", language, $"{section}.json");

                    if (!File.Exists(filePath))
                        return Results.NotFound();

                    var json = await File.ReadAllTextAsync(filePath, cancellationToken);
                    return Results.Content(json, "application/json");
                })
            .HasApiVersion(1.0)
            .AllowAnonymous()
            .WithName("GetTranslationFile");

        return builder;
    }
}
```

### FE integration pattern

```
1. App init → GET /translations/cs/auth.json (or whatever sections the app needs)
2. Store in memory / state manager
3. On API error → error.slug → lookup in loaded translations → show translated message
4. If slug not found → fall back to error.message (Czech hardcoded text)
```

---

## Step 8: Update Language CRUD Endpoints

### Current endpoints (keep)

| Method | Route                            | Description                    |
|--------|----------------------------------|--------------------------------|
| `GET`  | `/api/lang/languages`            | List languages (Anonymous)     |
| `GET`  | `/api/lang/languages/{shortcut}` | Get language by shortcut       |
| `PUT`  | `/api/lang/languages/{shortcut}` | Update language (toggle active)|

### Changes

- `UpdateLanguageCommandHandler` — already simplified (Step 4), just toggles `IsActive`
- No more Google Translate call
- Optionally: when a language is activated, call `RegenerateAllAsync` to generate JSON files for the newly active language (if translations already exist for it)

### Optional new endpoint

| Method | Route                             | Description                     |
|--------|-----------------------------------|---------------------------------|
| `POST` | `/api/lang/languages`             | Create a new language           |
| `DELETE`| `/api/lang/languages/{shortcut}` | Remove a language               |

These are optional — the current seed-only approach for creating languages may be sufficient.

---

## Summary: Files to Create / Modify / Delete

### Create

| File | Layer | Purpose |
|------|-------|---------|
| `Domain/Sections/Section.cs` | Domain | Section entity |
| `Domain/Sections/SectionErrors.cs` | Domain | Section error definitions |
| `Contracts/Sections/SectionResponse.cs` | Contracts | Section GET DTO |
| `Contracts/Sections/CreateSectionRequest.cs` | Contracts | Section POST DTO |
| `Contracts/Sections/UpdateSectionRequest.cs` | Contracts | Section PUT DTO |
| `Application/Common/Interfaces/ISectionsRepository.cs` | Application | Section repository contract |
| `Application/Common/Interfaces/ITranslationFileGenerator.cs` | Application | JSON generation contract |
| `Application/Sections/Commands/CreateSection/*` | Application | Create section handler |
| `Application/Sections/Commands/UpdateSection/*` | Application | Update section handler |
| `Application/Sections/Commands/RemoveSection/*` | Application | Remove section handler |
| `Application/Sections/Queries/GetSectionById/*` | Application | Get section handler |
| `Application/Sections/Queries/ListSections/*` | Application | List sections handler |
| `Application/Sections/Common/Models/SectionsFilteringParams.cs` | Application | Section filtering |
| `Infrastructure/Sections/Persistance/SectionConfigurations.cs` | Infrastructure | EF config for Section |
| `Infrastructure/Sections/Persistance/SectionsRepository.cs` | Infrastructure | Section data access |
| `Infrastructure/Translations/TranslationFileGenerator.cs` | Infrastructure | JSON file generation |
| `Presentation/Endpoints/Sections/*.cs` | Presentation | Section API endpoints |
| New EF Core migration | Infrastructure | Section table + FK |

### Modify

| File | Change |
|------|--------|
| `Domain/Translations/Translation.cs` | Add `SectionId`, `Section` navigation |
| `Infrastructure/Translations/Persistance/TranslationConfigurations.cs` | Add `SectionId` FK config |
| `Infrastructure/Common/Persistence/DataContext.cs` | Add `DbSet<Section>` |
| `Application/Common/Interfaces/IUnitOfWork.cs` | Add `ISectionsRepository` |
| `Infrastructure/Common/Persistence/UnitOfWork.cs` | Add `SectionsRepository` |
| `Contracts/Translations/CreateTranslationsRequest.cs` | Add `SectionSlug` |
| `Application/Translations/Commands/CreateTranslations/CreateTranslationsCommandHandler.cs` | Resolve section, trigger JSON regen |
| `Application/Translations/Commands/UpdateTranslation/UpdateTranslationCommandHandler.cs` | Trigger JSON regen |
| `Application/Translations/Commands/RemoveTranslation/RemoveTranslationCommandHandler.cs` | Trigger JSON regen |
| `Application/Languages/Commands/UpdateLanguage/UpdateLanguageCommandHandler.cs` | Remove Google Translate |
| `Infrastructure/Common/Persistence/Initialization/Seeds/TranslationsSeed.cs` | Rewrite: unified reflection scan |
| `Infrastructure/Common/Persistence/Initialization/DatabaseInitializer.cs` | Add sections seed step |
| `Infrastructure/DependencyInjection.cs` | Register `ITranslationFileGenerator` |
| `Presentation/DependencyInjection.cs` | Add static file middleware |
| `Presentation/ApiEndpoints.cs` | Add Sections routes |
| `Presentation/Endpoints/EndpointsExtensions.cs` | Register section endpoints |

### Delete

| File | Reason |
|------|--------|
| `Application/Languages/Configurations/GoogleApisSettings.cs` | Google Translate removed |
| `Auth/.../Seeds/TranslationsSeed.cs` | Replaced by unified seed |
| `Auth/.../Interfaces/ILangApiService.cs` | No more HTTP-based seeding |
| Google.Cloud.Translation.V2 package reference in `Lib.Shared.csproj` | No longer needed |
| `GoogleApisSettings` in `appsettings.*.json` | No longer needed |
