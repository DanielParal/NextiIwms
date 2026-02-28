# Lang Module Architecture

## Overview

The Lang module is a **localization and translation management system** within the Nexticz modular monolith. It provides CRUD operations for languages and translations, with automatic translation via Google Cloud Translation API when activating a new language.

---

## Module Structure

```
Modules/Lang/
├── Nexticz.Module.Lang.Contracts/          # Public API contracts (DTOs)
│   ├── Languages/
│   │   ├── LanguageResponse.cs             # GET response DTO
│   │   └── UpdateLanguageRequest.cs        # PUT request DTO
│   └── Translations/
│       ├── TranslationResponse.cs          # GET response DTO
│       ├── CreateTranslationsRequest.cs    # POST request DTO (batch)
│       ├── CreateTranslationsResponse.cs   # POST response DTO
│       └── UpdateTranslationRequest.cs     # PUT request DTO
│
├── Nexticz.Module.Lang.Domain/             # Core business entities & errors
│   ├── Languages/
│   │   ├── Language.cs                     # Language entity
│   │   └── LanguageErrors.cs              # Language-specific error definitions
│   └── Translations/
│       ├── Translation.cs                  # Translation entity
│       └── TranslationErrors.cs           # Translation-specific error definitions
│
├── Nexticz.Module.Lang.Application/        # Use cases (CQRS handlers)
│   ├── DependencyInjection.cs             # MediatR + FluentValidation registration
│   ├── Common/
│   │   ├── Behavior/
│   │   │   └── ValidationBehavior.cs      # MediatR pipeline (FluentValidation)
│   │   ├── Helpers/
│   │   │   ├── LangHelper.cs              # Slug parsing helper
│   │   │   └── StringHelper.cs            # DB schema/migration constants
│   │   └── Interfaces/
│   │       ├── ILanguagesRepository.cs    # Language repository contract
│   │       ├── ITranslationsRepository.cs # Translation repository contract
│   │       └── IUnitOfWork.cs             # Unit of Work contract
│   ├── Languages/
│   │   ├── Commands/
│   │   │   ├── CreateLanguage/            # Create a language (used by seed)
│   │   │   └── UpdateLanguage/            # Activate/deactivate + auto-translate
│   │   ├── Queries/
│   │   │   ├── GetLanguageByShortcut/     # Get single language by enum shortcut
│   │   │   └── ListLanguages/             # List with DevExtreme filtering
│   │   ├── Common/Models/
│   │   │   └── LanguagesFilteringParams.cs
│   │   └── Configurations/
│   │       └── GoogleApisSettings.cs      # Google Translate API key config
│   └── Translations/
│       ├── Commands/
│       │   ├── CreateTranslations/        # Batch create translations
│       │   ├── UpdateTranslation/         # Update single translation value
│       │   └── RemoveTranslation/         # Delete single translation
│       ├── Queries/
│       │   ├── GetTranslationById/        # Get single translation
│       │   └── ListTranslations/          # List with DevExtreme filtering
│       └── Common/Models/
│           └── TranslationsFilteringParams.cs
│
├── Nexticz.Module.Lang.Infrastructure/     # Data access & persistence
│   ├── DependencyInjection.cs             # EF Core + UoW registration
│   ├── Common/Persistence/
│   │   ├── DataContext.cs                 # EF Core DbContext
│   │   ├── UnitOfWork.cs                  # UoW implementation
│   │   ├── Extensions/
│   │   │   └── SeedDatabaseExtensions.cs  # DB migration + seed on startup
│   │   └── Initialization/
│   │       ├── DatabaseInitializer.cs     # Seed orchestrator
│   │       └── Seeds/
│   │           ├── LanguagesSeed.cs       # Seeds 12 languages (inactive)
│   │           └── TranslationsSeed.cs    # Seeds error message translations
│   ├── Languages/Persistance/
│   │   ├── LanguageConfigurations.cs      # EF Core entity configuration
│   │   └── LanguagesRepository.cs         # Language data access
│   ├── Translations/Persistance/
│   │   ├── TranslationConfigurations.cs   # EF Core entity configuration
│   │   └── TranslationsRepository.cs     # Translation data access
│   └── Migrations/
│       ├── 20240130153434_InitialMigration.cs
│       └── DataContextModelSnapshot.cs
│
└── Nexticz.Module.Lang.Presentation/       # API endpoints (Minimal API)
    ├── DependencyInjection.cs             # Module composition root
    ├── ApiEndpoints.cs                    # Route constants
    └── Endpoints/
        ├── EndpointsExtensions.cs         # Route group registration + auth
        ├── Languages/
        │   ├── LanguagesExtensions.cs     # Language endpoint mapping
        │   ├── GetLanguagesEndpoint.cs    # GET /api/lang/languages [Anonymous]
        │   ├── GetLanguageByShortcutEndpoint.cs  # GET /api/lang/languages/{shortcut}
        │   └── UpdateLanguageEndpoint.cs  # PUT /api/lang/languages/{shortcut}
        └── Translations/
            ├── TranstalatesExtensions.cs  # Translation endpoint mapping
            ├── GetTranslationsEndpoint.cs # GET /api/lang/translations [Anonymous]
            ├── GetTranslationByIdEndpoint.cs  # GET /api/lang/translations/{id}
            ├── CreateTranslationsEndpoint.cs  # POST /api/lang/translations [Anonymous]
            ├── UpdateTranslationEndpoint.cs   # PUT /api/lang/translations/{id}
            └── RemoveTranslationEndpoint.cs   # DELETE /api/lang/translations/{id}
```

---

## Domain Model

### Language Entity
| Property     | Type                          | Description                    |
|-------------|-------------------------------|--------------------------------|
| Id          | `Guid`                        | Primary key                    |
| Name        | `string` (max 10)             | Display name (e.g. "Czech")    |
| Shortcut    | `LanguageShortcutEnum`        | Enum key (Cs, En, De, ...)     |
| IsActive    | `bool`                        | Whether the language is active |
| Translations| `ICollection<Translation>?`   | Navigation property            |

### Translation Entity
| Property          | Type                          | Description                          |
|------------------|-------------------------------|--------------------------------------|
| Id               | `Guid`                        | Primary key                          |
| Module           | `string` (max 50)             | Source module (e.g. "lang")          |
| Feature          | `string` (max 50)             | Feature area (e.g. "api")           |
| Component        | `string` (max 50)             | Component (e.g. "translationService")|
| Name             | `string` (max 50)             | Translation key name                 |
| Slug             | `string` (max 255)            | Full key: `module-feature-component-name` |
| Value            | `string` (max 255)            | Translated text                      |
| LanguageShortcut | `LanguageShortcutEnum`        | FK to Language.Shortcut              |
| Language         | `Language?`                   | Navigation property                  |
| Created          | `DateTime`                    | UTC creation timestamp               |
| Updated          | `DateTime?`                   | Last update timestamp                |
| UpdatedWith      | `string?` (max 255)           | Audit info for updates               |

### Relationship
```
Language (1) ──── Shortcut ──→ (N) Translation.LanguageShortcut
              (Restrict on delete)
```

### Supported Languages (LanguageShortcutEnum)
`Cs` (Czech), `En` (English), `De` (German), `Es` (Spanish), `Fr` (French), `Hu` (Hungarian), `It` (Italian), `Lt` (Lithuanian), `Pl` (Polish), `Ro` (Romanian), `Ru` (Russian), `Sl` (Slovenian)

---

## API Endpoints

| Method   | Route                                | Auth       | Description                        |
|----------|--------------------------------------|------------|------------------------------------|
| `GET`    | `/api/lang/languages`                | Anonymous  | List languages (DevExtreme filter) |
| `GET`    | `/api/lang/languages/{shortcut}`     | Authorized | Get language by shortcut enum      |
| `PUT`    | `/api/lang/languages/{shortcut}`     | Authorized | Activate/deactivate language       |
| `GET`    | `/api/lang/translations`             | Anonymous  | List translations (DevExtreme)     |
| `GET`    | `/api/lang/translations/{id}`        | Authorized | Get translation by GUID            |
| `POST`   | `/api/lang/translations`             | Anonymous  | Batch create translations          |
| `PUT`    | `/api/lang/translations/{id}`        | Authorized | Update translation value           |
| `DELETE` | `/api/lang/translations/{id}`        | Authorized | Remove translation                 |

**Authorization**: Requires `Developer` or `SysAdmin` role with `Any` permission (except Anonymous endpoints).

---

## Key Flows

### 1. Application Startup (Seed)
1. `SeedLangDatabaseAsync` runs EF Core migrations
2. `LanguagesSeed` creates 12 languages (all `IsActive = false`)
3. `TranslationsSeed` reflects over `IErrorComponentSlugProvider` classes → extracts all `Error` properties → creates Czech translations from error codes/descriptions

### 2. Activate Language (PUT /api/lang/languages/{shortcut})
1. Load language by shortcut
2. Set `IsActive` to request value
3. Save language change
4. Load ALL translations (no filters)
5. Group by slug, find slugs missing the target language
6. For each missing: call Google Translate (Czech → target language)
7. Insert new Translation records

### 3. Create Translations (POST /api/lang/translations)
1. For each item in batch: parse slug → extract module/feature/component/name
2. Check if translation already exists for slug + language
3. If not exists → add new Translation
4. Save all
5. Re-query and return Czech translations for the slug as `{name: value}` dictionary

### 4. Slug Convention
Format: `module-feature-component-name`
Example: `lang-api-translationService-translationWithIdDoesnotExist`
Parsed by splitting on `-` (exactly 4 segments expected for translations, 3 for component slugs).

---

## Dependencies

### Internal
- `Nexticz.Lib.Shared` — EnumHelper, DevExtreme filtering, ErrorOr, DataAccess base, AuthorizationHelper

### External NuGet
- `MediatR` — CQRS command/query dispatching
- `FluentValidation` — Request validation pipeline
- `ErrorOr` — Discriminated union error handling
- `Microsoft.EntityFrameworkCore.SqlServer` — SQL Server persistence
- `Google.Cloud.Translation.V2` — Auto-translation via Google API
- `DevExtreme.AspNet.Data` — Server-side filtering/sorting/paging

### Database
- **Provider**: SQL Server
- **Schema**: `Lang`
- **Tables**: `Languages`, `Translations`
- **Migration history**: `__MigrationHistory` in `Lang` schema
