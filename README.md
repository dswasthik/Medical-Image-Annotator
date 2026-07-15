# MedImage — Enhancer & Annotator

A WPF desktop app for enhancing and annotating medical images (X-ray/scan style).
Load an image, apply OpenCV enhancements (grayscale, contrast/brightness, window/level,
histogram equalization, sharpen, invert), draw pixel measurements, and save each result
as a **Study** in SQL Server against your user account.

Built as a one-day, portfolio-grade sample showing MVVM + SOLID + several design patterns.

## Tech stack
- .NET 10 (LTS), WPF (`net10.0-windows`)
- EF Core 10 + SQL Server (LocalDB by default)
- OpenCvSharp4 for image processing
- CommunityToolkit.Mvvm for MVVM
- Microsoft.Extensions.Hosting for DI

## Solution layout (clean architecture)
```
MedImage.Domain          entities, value objects, interfaces (no framework deps)
MedImage.Infrastructure  EF Core, repositories, auth, OpenCV image pipeline
MedImage.App             WPF views, view-models, DI bootstrap
```
Dependencies point inward: App -> Infrastructure -> Domain. Domain references nothing.

## Design patterns / SOLID
- **Strategy** — every enhancement is an `IImageFilter` (Grayscale, WindowLevel, Sharpen, ...).
- **Factory** — `IImageFilterFactory` turns an `ImageAdjustments` value object into an ordered pipeline.
- **Chain** — `ImageProcessingService` applies the filter pipeline in sequence.
- **Repository + Unit of Work** — `IUnitOfWork`, `IUserRepository`, `IStudyRepository`.
- **MVVM** — view-model-first navigation via `DataTemplate`s; no code-behind except the
  PasswordBox bridge and the mouse-drag measurement tool.
- **DIP everywhere** — the App depends only on Domain abstractions; imaging returns PNG
  bytes so the UI never references OpenCV.

## Prerequisites
- Windows + **Visual Studio 2026** (v18.0+) with the **.NET desktop development** workload.
  Visual Studio 2022 **cannot** target `net10.0` (only `net9.0` and below), so use
  VS 2026 — or the `dotnet` CLI / VS Code + C# Dev Kit.
- **.NET 10 SDK** (10.0.x)
- A SQL Server instance. The default connection string uses **LocalDB**
  (ships with Visual Studio). To use another server, edit
  `src/MedImage.App/appsettings.json` -> `ConnectionStrings:Default`.

## Run it
1. Open `MedImageAnnotator.sln` in Visual Studio.
2. Set **MedImage.App** as the startup project.
3. Restore NuGet packages (automatic on first build).
4. Press **F5**.

On first launch the app calls `EnsureCreated()` and builds the `MedImageDb` schema
automatically — no migration step needed. Register an account, sign in, open an image,
adjust, drag to measure, and **Save study**.

### First-run flow
Register -> Sign in -> Open image -> tweak sliders/checkboxes -> drag on the image to
measure -> enter a title -> Save study -> it appears under "My studies" (reopen any time).

## Notes / decisions
- **EnsureCreated vs migrations.** For speed the app uses `EnsureCreated()`. To switch to
  proper migrations later:
  ```
  dotnet tool install --global dotnet-ef
  cd src/MedImage.Infrastructure
  dotnet ef migrations add Init --startup-project ../MedImage.App
  dotnet ef database update --startup-project ../MedImage.App
  ```
  (A `DesignTimeDbContextFactory` is included so the EF tools work.) Drop the
  `EnsureCreated()` call in `App.xaml.cs` when you move to migrations.
- **Coordinates.** The image is shown 1:1 (Stretch=None) so screen drags map directly to
  native pixels. Measurements are stored in pixels; add a mm calibration later if needed.
- **Passwords** are hashed with PBKDF2 (SHA-256, 100k iterations) — BCL only.
- Passwords use a code-behind bridge because WPF `PasswordBox` is intentionally non-bindable.

## Suggested one-day build order (if rebuilding from scratch)
1. Domain entities + interfaces.
2. EF Core `AppDbContext` + repositories + Unit of Work; get login/signup working.
3. OpenCV filters (Strategy) + factory + `ImageProcessingService`; wire the live preview.
4. Editor view: sliders, open/reset/save.
5. Measurement overlay (mouse drag) + burn-in on export.
6. Study history (save/list/reopen). Polish.
