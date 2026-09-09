# AGENTS.md — SiloSync

Guidance for coding agents working on this repository.

## Solution Overview

SiloSync moves media files from a legacy SQL Server "Gallery" table into a
central File Storage API, tracking progress locally so nothing is sent twice.

```
SiloSync.slnx
├─ SiloSync.Api      (ASP.NET Core Web API — file storage host + read-only report endpoints)
├─ SiloSync.Service  (Windows Service — polls SQL Server, uploads files)
├─ SiloSync.Shared   (shared DTOs/contracts referenced by all three)
└─ SiloSync.UI       (Blazor Server — MudBlazor, RTL, browses/filters report data)
```

- **Target framework:** .NET 9 for all projects.
- **Package versions are centralized.** This repo uses **Central Package
  Management (CPM)** — `Directory.Packages.props` at the repo root. Never add
  a `Version="..."` attribute directly in a `.csproj` `PackageReference`; add/
  update the version in `Directory.Packages.props` instead.
- `Directory.build.props` (repo root) applies shared build settings across
  all projects — check it before adding per-project overrides.

## SiloSync.Service

A `Microsoft.NET.Sdk.Web` project hosted as a Windows Service.

- `Program.cs` — `WebApplication.CreateBuilder` + `UseWindowsService()` +
  Serilog (`UseSerilog`).
- `Program.Services.cs` → `AddSiloSyncServices` — the **single place** to
  register DI: options, both `DbContext`s, the typed `HttpClient`, and the
  worker/reader/store/uploader services. Add new service registrations here,
  not in `Program.cs`.
- `Serivces\` (note: **this folder name is intentionally misspelled** —
  keep the existing spelling for consistency; do not "fix" it in isolation
  without also renaming all references and usings).
  - `Worker.cs` — `IHostedService`/`BackgroundService` main loop using a
    `PeriodicTimer` (`PollIntervalMinutes`). Each cycle: read watermark →
    fetch new Gallery rows → resolve file path → skip/log missing file or
    null `fld_GalleryUsageId` → upload → record `SentFile` → advance
    watermark. The whole cycle is wrapped in try/catch so one failure never
    kills the service.
  - `IGalleryReader` / `GalleryReader` — reads `dbo.tbl_Gallery` rows where
    `fld_GalleryId > lastId`, ascending, capped by `BatchSize`.
  - `ISyncStateStore` / `SyncStateStore` — reads/updates the SQLite
    watermark and idempotently checks/inserts `SentFile` rows.
  - `IFileUploader` / `ApiFileUploader` — typed `HttpClient` (via
    `AddHttpClient`) posting `multipart/form-data` to the API.
- `Options\SiloSyncOptions.cs` — strongly-typed options bound from the
  `SiloSync` config section via `AddOptions<T>().Bind(...)
  .ValidateDataAnnotations()`. Add new settings here + `appsettings.json`,
  never read `IConfiguration` directly in service code.
- `Data\Gallery\` — `GalleryEntity` + `GalleryDbContext` (SQL Server,
  **read-only**, no-tracking by default). This context must never write to
  the Gallery database.
- `Data\Local\` — `SyncDbContext` (SQLite) with:
  - `SyncState` — single-row watermark (`LastGalleryId`, `UpdatedAtUtc`).
  - `SentFile` — safety net against duplicate sends (`GalleryId` unique,
    `UsageId`, `MediaName`, `FilePath`, `SentAtUtc`, `RemoteUrl`).
  - The SQLite DB is migrated/created at startup (`Database.Migrate()` /
    `EnsureCreated`); the directory is created if missing. The DB file must
    live under a writable path (`AppContext.BaseDirectory` or ProgramData)
    because the service runs under a service account.

### Config (`appsettings.json` → `SiloSync` section)
`PollIntervalMinutes`, `SqlServer:ConnectionString`,
`SqlServer:MediaRootPath`, `Sqlite:DatabasePath`, `Api:BaseUrl`,
`Api:UploadEndpoint`, `BatchSize`.

### Known data quirks (must stay respected in new code)
- `fld_GalleryUsageId` is **nullable** — rows without a usage id are skipped
  and logged, but the watermark still advances past them.
- `fld_GalleryMediaPath` may be **relative** — resolve against the
  configurable `MediaRootPath`. If the resolved file is missing, log and
  skip it (do not advance the watermark incorrectly / do not throw and
  crash the cycle).

## SiloSync.Api

Bare ASP.NET Core Controllers host with OpenAPI.

- References `SiloSync.Shared` for DTOs — keep API and Service in sync
  through shared contracts rather than duplicating request/response shapes.
- `Services\IFileStorageService` / `FileStorageService` — saves uploads to
  `{Root}/{sanitized UsageId}/{sanitized fileName}`, creates directories,
  handles filename collisions, enumerates files per usage id, and builds
  download URLs.
- `Controllers\FilesController.cs`:
  - `POST /api/files` — multipart upload, returns stored URL.
  - `GET /api/files/{usageId}` — list of download URLs.
  - `GET /api/files/{usageId}/{fileName}` — streams file with content-type
    detection.
- Config (`appsettings.json` → `FileStorage` section): `RootPath`,
  `PublicBaseUrl`.
- **Security requirement:** `usageId` (and file names) are used to build
  folder/file paths — always sanitize them to prevent path traversal. Any
  new endpoint touching the filesystem must go through
  `IFileStorageService`, not raw `Path.Combine` with user input.
- `Program.cs` configures larger multipart/Kestrel request body size limits
  for uploads and ensures the storage root directory exists at startup.

### VehicleTagReport (read-only reporting)

A second, **independent SQL Server connection** used only for reporting —
never written to, and never assume it's the same database/server as
anything `SiloSync.Service` talks to.

- `Options\VehicleTagReportOptions.cs` — `VehicleTagReport:ConnectionString`
  config section.
- `Data\VehicleTagReport\` — `VehicleTagReportEntity` +
  `VehicleTagReportDbContext`, mapped via Fluent API (`OnModelCreating`) to
  `[dbo].[VehicleTagReport]`. The table's columns are **Persian
  identifiers** (e.g. `سریال`, `عنوان صف خودرو`, `مرکز پذیرش`); the entity
  exposes English property names and each `HasColumnName(...)` call maps to
  the real column. Always query with `AsNoTracking()`.
- `Services\IVehicleTagReportService` / `VehicleTagReportService` — filters
  by partial `SerialNumber`/`QueueTitle` (`Contains`) and exact
  `ReceptionCenter`, pages server-side, and exposes distinct
  `ReceptionCenter` values for filter dropdowns. Materialize entities with
  `ToListAsync()` before mapping to `VehicleTagReportDto` — EF Core cannot
  translate a static mapping method inside `.Select(...)`.
- `Controllers\VehicleTagReportController.cs`:
  - `GET /api/vehicletagreport?serialNumber=&queueTitle=&receptionCenter=&pageNumber=&pageSize=`
    → `PagedResult<VehicleTagReportDto>`.
  - `GET /api/vehicletagreport/reception-centers` → `string[]`.
- `سریال` doubles as the file-storage `usageId` — the UI fetches a record's
  images by calling the existing `GET /api/files/{usageId}` with
  `SerialNumber`, not through this controller.

## SiloSync.Shared

Currently holds the shared contracts used by both API and Service:
`UploadFileRequest`, `UploadFileResponse`, `FileLinkDto` (usageId, media
name, usage type, extension type, upload datetime, additional data,
returned URLs); and the reporting contracts `VehicleTagReportDto`,
`VehicleTagReportQuery`, `PagedResult<T>` (used by `SiloSync.Api` and
`SiloSync.UI`). Put any new cross-project DTO here rather than duplicating
it in `Api`, `Service`, or `UI`.

## SiloSync.UI

Blazor Server app (`InteractiveServer` render mode) — the operator-facing
front end. RTL/Persian by design; do not add LTR-only assumptions.

- References `SiloSync.Shared` (for the DTOs) and calls `SiloSync.Api` over
  HTTP via **typed `HttpClient`s** registered in `Program.cs` — never call
  `IConfiguration`/raw `HttpClient` from a component. Because this is
  Blazor **Server**, all `IVehicleTagReportApiClient`/`IFilesApiClient`
  calls happen on the server; no CORS configuration is needed on the Api
  for them. `<img>` tags pointing at `GET /api/files/{usageId}/{fileName}`
  *do* hit the Api directly from the browser — keep `FileStorage:PublicBaseUrl`
  on the Api reachable from wherever the UI is opened.
- `Options\ApiOptions.cs` — `Api:BaseUrl` config section (base address of
  `SiloSync.Api`).
- `ApiClients\` — `IVehicleTagReportApiClient`/`VehicleTagReportApiClient`
  (search + reception-center lookup) and `IFilesApiClient`/`FilesApiClient`
  (per-usageId file links, reusing the Api's existing `FilesController`).
  Both fail soft (log + return an empty result) rather than throwing, so a
  down Api degrades the grid instead of crashing the page.
- `Components\Pages\Home.razor` — the main report page: three filters
  (سریال text, عنوان صف خودرو text, مرکز پذیرش dropdown sourced from the
  Api) above a `MudDataGrid` using `ServerData` for server-side paging.
  **Keep `PagerContent` with a `MudDataGridPager` present** — without it,
  `MudDataGrid` silently re-applies its own client-side sort/filter on top
  of the server-paged data.
- `Components\Shared\VehicleDetailPanel.razor` /
  `VehicleImageGallery.razor` — the grid's `HierarchyColumn` row-expansion
  content: the remaining (non-column) fields, plus a lazily-loaded image
  gallery keyed off `SerialNumber` as the `usageId`. Load images on expand,
  not eagerly per row, to avoid an N+1 call per page load.
- MudBlazor is wired globally: `Program.cs` → `AddMudServices()`;
  `App.razor` loads `_content/MudBlazor/MudBlazor.min.css`/`.min.js` plus
  the Vazirmatn Persian web font and sets `<html lang="fa" dir="rtl">`;
  `MainLayout.razor` wraps everything in `<MudRTLProvider RightToLeft="true">`
  (with `MudThemeProvider`/`MudPopoverProvider`/`MudDialogProvider`/
  `MudSnackbarProvider` as its children, per MudBlazor's RTL requirement).
- No authentication/authorization on this project by design (internal tool).
- Config (`appsettings.json` → `Api` section): `Api:BaseUrl`.

## Conventions for Future Changes

1. **CPM first**: add/bump package versions in `Directory.Packages.props`
   before referencing them in any `.csproj`.
2. **DI registration** goes through `AddSiloSyncServices` (Service) and the
   equivalent composition root in `SiloSync.Api\Program.cs` — don't scatter
   registrations.
3. **Options pattern** for all configuration; validate with data
   annotations.
4. **Idempotency matters**: the sync worker must be safe to re-run without
   re-uploading files — always check `SentFile` before upload and advance
   the watermark only after successful processing of a row.
5. **Don't rename the `Serivces` folder** casually — it's a known,
   intentional misspelling kept for consistency with existing code/imports.
6. **Read-only Gallery access**: never add write operations to
   `GalleryDbContext`.
7. **Read-only VehicleTagReport access**: same rule for
   `VehicleTagReportDbContext` in `SiloSync.Api` — it's a reporting
   connection only, never write to it, and don't assume it shares a
   database/server with the Gallery connection used by `SiloSync.Service`.
8. Keep the plan file
   (`C:\Users\Software-Lead\.copilot\plans\plan-silosync-gallery-file-sync-service-file-storage-api.md`)
   in mind for context on *why* things are structured this way; the
   remaining open item is end-to-end verification (build, run API, run
   Service against a real SQL Server table, confirm files land under
   `{Root}/{UsageId}/` and aren't re-sent).
