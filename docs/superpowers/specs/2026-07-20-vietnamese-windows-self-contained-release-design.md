# Vietnamese Windows Self-Contained Release Design

## Goal

Publish a Windows x64 ZIP from the `tintutien` branch that users can extract and run without installing .NET 10.

## Trigger

A GitHub Actions workflow runs when a tag matching `vi-v*` is pushed, for example:

```text
vi-v5.1.0-rc3.1
```

The tag determines the exact source commit and the GitHub Release tag.

## Build

The workflow uses a Windows GitHub-hosted runner and .NET SDK 10 to publish `src/ui/UI.csproj` with:

- Release configuration
- `win-x64` runtime identifier
- self-contained deployment
- single-file publishing
- debug symbols disabled

The build environment requires .NET 10, but the resulting application bundles the runtime and does not require .NET 10 on the user's computer.

## Runtime dependency

The workflow downloads the repository's pinned Windows x64 `libmpv` package and places `libmpv-2.dll` beside `SubtitleEdit.exe`. Keeping this DLL outside the single-file executable preserves video playback support.

## Artifact

The workflow creates one file:

```text
SubtitleEdit-Vietnamese-Windows-x64.zip
```

The archive contains the published application, including at minimum:

```text
SubtitleEdit.exe
libmpv-2.dll
```

No installer or additional release guide is generated.

## Release

The workflow creates a GitHub Release for the pushed `vi-v*` tag and attaches only the Windows x64 ZIP artifact.

## Vietnamese default

The workflow does not generate a separate settings file. The application uses the existing Windows first-run behavior on the `tintutien` branch, which defaults new Windows profiles to Vietnamese. Existing user settings remain respected.

## Verification

Before creating the release, the workflow must:

1. Restore dependencies.
2. Build the UI project in Release mode.
3. Run the UI test project in Release mode.
4. Publish the self-contained `win-x64` application.
5. Verify that `SubtitleEdit.exe` and `libmpv-2.dll` exist in the publish directory.
6. Create the ZIP and verify that it exists and is non-empty.

A failure in any step prevents GitHub Release creation.

## User flow

The user downloads `SubtitleEdit-Vietnamese-Windows-x64.zip`, extracts it, and runs `SubtitleEdit.exe`. No separate .NET 10 installation is required.
