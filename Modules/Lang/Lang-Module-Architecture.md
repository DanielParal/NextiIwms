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

## How Errors Are Returned to the Frontend (Current State)

When an API handler returns an `ErrorOr<T>` error, the endpoint calls `ResultsHelper.Problem(errors)` which maps it to an `ApiErrorResponse`:

```json
{
  "correlationId": "abc-123",
  "errors": [
    {
      "slug": "auth-api-authService-invalidPassword",
      "message": "Neplatné heslo"
    }
  ]
}
```

The **slug** is the `Error.Code` (built from `ComponentSlug + name`) and the **message** is the `Error.Description` (hardcoded Czech text). The FE receives the slug and can look up the translated text from its loaded translations.

### The two translation source interfaces

#### 1. `IErrorComponentSlugProvider` — Error messages (101 implementations)

Every error class implements this interface with a static `ComponentSlug` property. Each `Error` static property encodes the slug as its `Code` and Czech text as `Description`:

```csharp
public abstract class AuthenticationErrors : IErrorComponentSlugProvider
{
    public static string ComponentSlug { get; } = "auth-api-authService-";

    public static Error InvalidPassword => Error.Validation(
        code: ComponentSlug + "invalidPassword",           // ← slug
        description: "Neplatné heslo");                    // ← Czech default
}
```

At startup, `TranslationsSeed` uses **reflection** to extract all `Error` properties and seeds them as Czech translations.

#### 2. `ITranslatable` — General UI translations (not yet used)

```csharp
public interface ITranslatable
{
    string TranslationKey { get; }    // slug
    string TranslationValue { get; }  // Czech default
}
```

Record implementation exists (`Translation` record in `Lib.Shared`) but **no classes currently implement it in any module**. This is the intended interface for non-error translations (labels, button texts, tooltips, etc.).

### Current seed process

Each module has its own `TranslationsSeed` that runs at startup:

| Module | How it seeds |
|--------|-------------|
| **Lang** | Directly uses MediatR to call `CreateTranslationsCommand` — seeds `TranslationErrors`, `LanguageErrors`, `Errors.Common` |
| **Auth** | Waits for API health check, then calls Lang API over HTTP (`ILangApiService.CreateTranslations`) — seeds `AuthenticationErrors`, `Errors.Common` |
| **Other modules** | Similar pattern — each module reflects its own error classes |

---

## Proposal: Static JSON Translation Files

### Motivation

Replace the current `GET /api/lang/translations` API call with **pre-generated static JSON files** served directly from the server. The FE fetches a single file per module+language instead of querying the database on every page load.

### Target file structure

```
wwwroot/translations/
  ├── cs/
  │   ├── shared.json        ← Errors.Common, FileHandlingErrors, ImportErrors
  │   ├── lang.json          ← LanguageErrors, TranslationErrors
  │   ├── auth.json          ← AuthenticationErrors
  │   ├── mmo.json
  │   ├── vh.json
  │   ├── sign.json
  │   └── ...
  ├── en/
  │   ├── shared.json
  │   ├── lang.json
  │   ├── auth.json
  │   └── ...
  ├── de/
  │   └── ...
  └── manifest.json          ← version/hash per file for cache busting
```

### JSON file format

Flat key-value (slug suffix → translated text), grouped by module:

```json
// wwwroot/translations/cs/auth.json
{
  "authService": {
    "invalidPassword": "Neplatné heslo",
    "userIsCurentlyLogged": "Neočekávaná chyba, byli jste odhlášeni",
    "validationTokenError": "JWT token není validní",
    "registrationError": "Chyba při registraci"
  }
}
```

The FE loads only what it needs: `GET /translations/cs/auth.json`

### Manifest file for cache busting

```json
// wwwroot/translations/manifest.json
{
  "version": "2026-02-28T12:00:00Z",
  "files": {
    "cs/auth.json": "a1b2c3d4",
    "cs/lang.json": "e5f6g7h8",
    "en/auth.json": "i9j0k1l2"
  }
}
```

FE fetches manifest first, then loads files with `?v=a1b2c3d4` query param for cache busting.

### When to regenerate

| Trigger | What to regenerate |
|---------|-------------------|
| Translation created/updated/deleted | Only the affected `{lang}/{module}.json` file |
| Language activated (Google Translate) | All `{lang}/*.json` files for the new language |
| Application startup (seed) | All files for all active languages |

A `TranslationFileGenerator` service handles regeneration. It queries translations from DB grouped by module + language and writes JSON files to `wwwroot/translations/`.

### How errors flow to the FE (proposed)

1. **Startup**: Seed runs → errors reflected from `IErrorComponentSlugProvider` + translations from `ITranslatable` → saved to DB → JSON files generated
2. **Language activation**: Google Translate fills missing translations → JSON files regenerated for new language
3. **API error response** stays the same — returns `{ slug, message }` — but the FE uses the **slug** to look up the translated message from its loaded JSON file instead of showing the hardcoded Czech `message`
4. **FE initialization**: Loads `manifest.json` → loads `/{lang}/{module}.json` for each needed module → caches in memory

```
API Error Response                    FE Translation Lookup
┌─────────────────────┐              ┌────────────────────────────┐
│ {                   │              │ auth.json (loaded at init) │
│   "slug":           │──lookup──→   │ {                          │
│     "auth-api-      │              │   "authService": {         │
│      authService-   │              │     "invalidPassword":     │
│      invalidPassword│              │       "Invalid password"   │
│   "message":        │              │   }                        │
│     "Neplatné heslo"│              │ }                          │
│ }                   │              └────────────────────────────┘
└─────────────────────┘
    ↑ fallback if FE                        ↑ preferred
    translation missing                     (user's language)
```

### How to collect all translatable content via reflection

Both interfaces can be discovered at startup using assembly scanning:

```
┌─────────────────────────────────┐     ┌──────────────────────────────┐
│  IErrorComponentSlugProvider    │     │  ITranslatable               │
│  (101 implementations)         │     │  (future UI translations)    │
│                                 │     │                              │
│  Reflect → static Error props   │     │  Reflect → TranslationKey +  │
│  Error.Code = slug              │     │            TranslationValue   │
│  Error.Description = Czech text │     │                              │
└───────────────┬─────────────────┘     └──────────────┬───────────────┘
                │                                      │
                └──────────────┬───────────────────────┘
                               ▼
                 ┌─────────────────────────┐
                 │  TranslationsSeed       │
                 │  (unified, per module)  │
                 │                         │
                 │  1. Scan all assemblies  │
                 │  2. Collect slugs+values │
                 │  3. Upsert to DB        │
                 │  4. Generate JSON files  │
                 └─────────────────────────┘
```

### Suggested implementation for `ITranslatable` usage

Modules can define UI translation classes that implement `ITranslatable`:

```csharp
// In any module, e.g. Auth
public class AuthUiTranslations
{
    public static ITranslatable LoginButton => new Translation(
        "auth-ui-login-loginButton", "Přihlásit se");
    public static ITranslatable LogoutButton => new Translation(
        "auth-ui-login-logoutButton", "Odhlásit se");
    public static ITranslatable ForgotPassword => new Translation(
        "auth-ui-login-forgotPassword", "Zapomenuté heslo");
}
```

The seed process scans for all `static ITranslatable` properties the same way it scans for `static Error` properties — by reflection. Both feed into the same translation DB and JSON generation pipeline.

### What changes vs. current approach

| Aspect | Current | Proposed |
|--------|---------|----------|
| FE fetches translations | `GET /api/lang/translations` (DB query each time) | `GET /translations/{lang}/{module}.json` (static file) |
| Error seeding | Each module seeds independently, some via HTTP | Unified seed scans all assemblies at startup |
| Translation sources | Only `IErrorComponentSlugProvider` | Both `IErrorComponentSlugProvider` + `ITranslatable` |
| Google Translate output | Stored only in DB | Stored in DB + regenerated JSON files |
| Caching | None (DevExtreme paging) | Browser/CDN cache + manifest hash busting |
| `GET /api/lang/translations` | Public, anonymous | Kept as admin-only for translation management UI |

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
