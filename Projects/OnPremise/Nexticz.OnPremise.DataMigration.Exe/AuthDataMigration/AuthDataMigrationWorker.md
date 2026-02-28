# Auth Data Migration Worker (MS SQL ➜ Postgres)

## Goal
Implement a background worker that migrates Identity/auth data (e.g., `AppUser`, roles, user-role mappings, and other Identity tables if present) from an **MS SQL** database into a **Postgres** database.

## Given / Constraints
- Connection strings exist in configuration:
    - `ConnectionStrings:AuthMsSql`
    - `ConnectionStrings:AuthPostgres`
- Instead of reading configuration directly in the worker, you may inject a singleton **`AuthDataMigrationSettings`** that provides these connection strings.
- The worker **`AuthDataMigrationWorker`** is already registered.
- Execution must be gated by Feature Management:
    - Only run migration if the feature flag **`EnableAuthDataMigration`** is enabled.
- The solution already contains EF Core DbContexts you can use:
    - **`AuthMsSqlContext`** (SQL Server source)
    - **`AuthPostgresContext`** (Postgres target)

## Feature flag gating (required)
- Inject `IFeatureManager`.
- Before doing any DB work, check:
    - `await featureManager.IsEnabledAsync("EnableAuthDataMigration")`
- If disabled:
    - Log that migration is disabled
    - Exit early / do nothing (no DB connections, no queries)

## Data scope (minimum)
Migrate at least these sets (names may differ depending on schema):
- Users (`AppUser` / users table)
- Roles (`AppUserRoles` / roles table)
- User↔Role mapping (user-role join table)

If present in the schema, also migrate common Identity tables:
- User claims
- Role claims
- External logins
- Tokens

## Data access approach
Use the simplest approach that fits the repository:
- EF Core with two DbContexts (one for SQL Server, one for Postgres).
- Prefer using the existing **`AuthMsSqlContext`** and **`AuthPostgresContext`** rather than creating new contexts.

Avoid hardcoding table/column names where possible; inspect schema or use mapped entities.

## Mapping rules (critical)
- Preserve auth-critical fields exactly to keep logins valid:
    - `PasswordHash`, `SecurityStamp`, `ConcurrencyStamp`, lockout fields, normalized username/email, etc.
- Handle common type conversions correctly:
    - MSSQL `uniqueidentifier` ➜ Postgres `uuid`
    - `nvarchar(max)` ➜ `text`
    - `bit` ➜ `boolean`

## Ordering / referential integrity
Migrate in FK-safe order (typical):
1. Roles
2. Users
3. UserRoles (join)
4. Claims / Logins / Tokens (if applicable)

## Idempotency & conflict strategy (required)
The migration must be safe to run multiple times.

Choose a consistent conflict strategy and apply it everywhere:
- Recommended: **Upsert** (insert or update) in Postgres keyed by `Id`.
    - Postgres: `INSERT ... ON CONFLICT (Id) DO UPDATE ...`
- Alternative: `DO NOTHING` if you explicitly want to avoid overwriting.

Document which strategy is used.

## Logging / observability
Log progress without leaking secrets:
- Feature flag state
- Start/end of migration
- Per-table migrated counts: read/inserted/updated/skipped
- Duration per stage

Do **not** log sensitive values (password hashes, tokens, secrets).

## Run strategy
Default behavior should be **run-once**:
- When enabled: migrate, log success, then stop (or sleep long).
- Do not continuously re-run unless explicitly configured.

## Acceptance checklist
- When `EnableAuthDataMigration` is `false`: worker performs **no** migration work.
- When enabled: migrates Users, Roles, UserRoles (minimum), preserving auth fields.
- Can be rerun safely (idempotent; no duplicates; predictable upsert behavior).
- Uses `AuthDataMigrationSettings` for connection strings (no hardcoded strings).
- Uses batching + respects cancellation.