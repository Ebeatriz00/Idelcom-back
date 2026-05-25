# Repository Guidelines

## Project Structure & Module Organization

This solution follows Clean Architecture. Main projects are `Application/`, `Core/`, `Infrastructure/`, `GlueMark/`, `DependencyInjection/`, `SharedKernel/`, and `Tests/`. Use `GlueMark/` for API controllers and middleware, `Application/` for DTOs, validators, services, and use cases, `Core/` for entities and interfaces, and `Infrastructure/` for repositories, persistence, notifications, and security.

Add new features as vertical slices: entity, interface, DTOs, validator, mapping profile, use case or service, repository, DI registration, and controller.

## Build, Test, and Development Commands

Run commands from the repository root:

- `dotnet restore Idelcom.WebApi.sln` restores NuGet packages.
- `dotnet build Idelcom.WebApi.sln` builds the full solution.
- `dotnet test Idelcom.WebApi.sln` runs the test project in `Tests/`.
- `dotnet run --project GlueMark/Idelcom.csproj` starts the API locally.

## Coding Style & Naming Conventions

Use 4-space indentation and standard C# formatting. Use `PascalCase` for classes, methods, DTOs, and profiles; use `camelCase` for locals and parameters; prefix interfaces with `I`.

Avoid generic `Model` names. Prefer purpose-specific suffixes such as `Dto`, `Result`, `Projection`, `Item`, `Context`, `Snapshot`, or `Row`. Stored procedures follow `SP_WS_*`. For Dapper persistence, prefer shared helpers such as `IDapperHelper`, `DapperParams`, and `DbParam` when applicable.

## Testing Guidelines

Tests live in `Tests/` and currently use the standard .NET test project structure. Add tests when changing validation rules, orchestration, repository mappings, or audit behavior. Prefer names that mirror the production class, for example `UpdateOperationsTeamSsomaTests`.

## Commit & Pull Request Guidelines

Recent history shows a Conventional Commit style such as `feat(operations): ...` and `docs: ...`. Use `feat`, `fix`, `refactor`, `docs`, `test`, or `hotfix`. Keep the subject short and imperative, and use a scope when the change targets a specific module, for example `feat(operationsWorkOrder): add status audit`.

For pull requests, include a clear summary, affected modules, validation performed, and sample request or response details when API behavior changes.

## Security & Configuration Tips

Do not commit secrets or local connection strings. Keep environment-specific settings in `appsettings.*`. Validate request DTOs before repository calls and preserve audit flows that rely on `[AuditField]`, `IAuditLogFactory`, and `IAuditService`.
