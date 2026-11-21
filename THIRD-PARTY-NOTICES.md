# Third-party notices

**CheckYourNumbers** is © 2026 Per Hejndorf and is licensed under the [MIT License](LICENSE), except where stated below. This file lists the third-party components and assets it uses, each under its own licence. Licence texts and copyright notices are kept by the upstream projects at the *Source* links; where a licence requires its notice to travel with binaries, the upstream package already carries it.

_Last reviewed: 2026-11-21. Versions are the ones resolved in this repository at that date; Dependabot keeps them current, so check the project files for the exact version of a given release._

## Libraries included in the application

| Library | Version | Licence | Source | Modified |
|---|---|---|---|---|
| .NET runtime and Blazor WebAssembly (shipped in `_framework/`) | 10.0 | MIT | [dotnet/runtime](https://github.com/dotnet/runtime), [dotnet/aspnetcore](https://github.com/dotnet/aspnetcore) | No |
| Microsoft.AspNetCore.Components.WebAssembly | 10.0.14 | MIT | [NuGet](https://www.nuget.org/packages/Microsoft.AspNetCore.Components.WebAssembly/10.0.14) | No |

<details>
<summary>Indirect (transitive) dependencies — 25 packages</summary>

These packages are pulled in by the libraries above and are shipped with the application. All are used unmodified.

| Library | Version | Licence | Source | Modified |
|---|---|---|---|---|
| Microsoft.AspNetCore.Authorization | 10.0.14 | MIT | [NuGet](https://www.nuget.org/packages/Microsoft.AspNetCore.Authorization/10.0.14) | No |
| Microsoft.AspNetCore.Components | 10.0.14 | MIT | [NuGet](https://www.nuget.org/packages/Microsoft.AspNetCore.Components/10.0.14) | No |
| Microsoft.AspNetCore.Components.Forms | 10.0.14 | MIT | [NuGet](https://www.nuget.org/packages/Microsoft.AspNetCore.Components.Forms/10.0.14) | No |
| Microsoft.AspNetCore.Components.Web | 10.0.14 | MIT | [NuGet](https://www.nuget.org/packages/Microsoft.AspNetCore.Components.Web/10.0.14) | No |
| Microsoft.AspNetCore.Metadata | 10.0.14 | MIT | [NuGet](https://www.nuget.org/packages/Microsoft.AspNetCore.Metadata/10.0.14) | No |
| Microsoft.Extensions.Configuration | 10.0.14 | MIT | [NuGet](https://www.nuget.org/packages/Microsoft.Extensions.Configuration/10.0.14) | No |
| Microsoft.Extensions.Configuration.Abstractions | 10.0.14 | MIT | [NuGet](https://www.nuget.org/packages/Microsoft.Extensions.Configuration.Abstractions/10.0.14) | No |
| Microsoft.Extensions.Configuration.Binder | 10.0.14 | MIT | [NuGet](https://www.nuget.org/packages/Microsoft.Extensions.Configuration.Binder/10.0.14) | No |
| Microsoft.Extensions.Configuration.FileExtensions | 10.0.14 | MIT | [NuGet](https://www.nuget.org/packages/Microsoft.Extensions.Configuration.FileExtensions/10.0.14) | No |
| Microsoft.Extensions.Configuration.Json | 10.0.14 | MIT | [NuGet](https://www.nuget.org/packages/Microsoft.Extensions.Configuration.Json/10.0.14) | No |
| Microsoft.Extensions.DependencyInjection | 10.0.14 | MIT | [NuGet](https://www.nuget.org/packages/Microsoft.Extensions.DependencyInjection/10.0.14) | No |
| Microsoft.Extensions.DependencyInjection.Abstractions | 10.0.14 | MIT | [NuGet](https://www.nuget.org/packages/Microsoft.Extensions.DependencyInjection.Abstractions/10.0.14) | No |
| Microsoft.Extensions.Diagnostics | 10.0.14 | MIT | [NuGet](https://www.nuget.org/packages/Microsoft.Extensions.Diagnostics/10.0.14) | No |
| Microsoft.Extensions.Diagnostics.Abstractions | 10.0.14 | MIT | [NuGet](https://www.nuget.org/packages/Microsoft.Extensions.Diagnostics.Abstractions/10.0.14) | No |
| Microsoft.Extensions.FileProviders.Abstractions | 10.0.14 | MIT | [NuGet](https://www.nuget.org/packages/Microsoft.Extensions.FileProviders.Abstractions/10.0.14) | No |
| Microsoft.Extensions.FileProviders.Physical | 10.0.14 | MIT | [NuGet](https://www.nuget.org/packages/Microsoft.Extensions.FileProviders.Physical/10.0.14) | No |
| Microsoft.Extensions.FileSystemGlobbing | 10.0.14 | MIT | [NuGet](https://www.nuget.org/packages/Microsoft.Extensions.FileSystemGlobbing/10.0.14) | No |
| Microsoft.Extensions.Logging | 10.0.14 | MIT | [NuGet](https://www.nuget.org/packages/Microsoft.Extensions.Logging/10.0.14) | No |
| Microsoft.Extensions.Logging.Abstractions | 10.0.14 | MIT | [NuGet](https://www.nuget.org/packages/Microsoft.Extensions.Logging.Abstractions/10.0.14) | No |
| Microsoft.Extensions.Options | 10.0.14 | MIT | [NuGet](https://www.nuget.org/packages/Microsoft.Extensions.Options/10.0.14) | No |
| Microsoft.Extensions.Options.ConfigurationExtensions | 10.0.14 | MIT | [NuGet](https://www.nuget.org/packages/Microsoft.Extensions.Options.ConfigurationExtensions/10.0.14) | No |
| Microsoft.Extensions.Primitives | 10.0.14 | MIT | [NuGet](https://www.nuget.org/packages/Microsoft.Extensions.Primitives/10.0.14) | No |
| Microsoft.Extensions.Validation | 10.0.14 | MIT | [NuGet](https://www.nuget.org/packages/Microsoft.Extensions.Validation/10.0.14) | No |
| Microsoft.JSInterop | 10.0.14 | MIT | [NuGet](https://www.nuget.org/packages/Microsoft.JSInterop/10.0.14) | No |
| Microsoft.JSInterop.WebAssembly | 10.0.14 | MIT | [NuGet](https://www.nuget.org/packages/Microsoft.JSInterop.WebAssembly/10.0.14) | No |

</details>

## Build, test and tooling (not distributed)

Used to build, test or render promotional material only; not part of the shipped application.

| Library | Version | Licence | Source | Modified |
|---|---|---|---|---|
| Microsoft.AspNetCore.Components.WebAssembly.DevServer | 10.0.14 | MIT | [NuGet](https://www.nuget.org/packages/Microsoft.AspNetCore.Components.WebAssembly.DevServer/10.0.14) | No |
| edge-tts (Python package, used by `tools/tts/WordsToMp3.py` to make the German, Swiss German, French and Spanish recordings) | 7.3.1 | LGPL-3.0 | [rany2/edge-tts](https://github.com/rany2/edge-tts) | No |

## Fonts, data and online services

These are used at run time but are not bundled in this repository. Their own terms apply.

| Service | Used for | Terms | Source |
|---|---|---|---|
| Google Fonts | Web fonts (Fraunces, Inter, JetBrains Mono), loaded at run time from Google's CDN; font files are not included in this repository | SIL Open Font License 1.1 | [fonts.google.com](https://fonts.google.com) |

## Icons, images, audio and video

| Asset(s) | Origin | Licence |
|---|---|---|
| `CheckYourNumbers/wwwroot/favicon.ico`, `CheckYourNumbers/wwwroot/icons/` (8 files: .png) | Drawn by the author's script `tools/make-icons.ps1` (System.Drawing), not made with AI | MIT (as part of this project) |
| `images/banner.png` | Rendered by the author's script `tools/make-banner.ps1` from `tools/banner.html` (the app's own markup, icon and flags), not made with AI | MIT (as part of this project) |
| `CheckYourNumbers/wwwroot/flags/` (8 files: .svg) | National flags drawn as SVG code for this project (Spain simplified, without the coat of arms), not made with AI. National flags are generally not protected by copyright | MIT (as part of this project) |
| `CheckYourNumbers/wwwroot/audio/da/numbers/` (100 files: .mp3) | Synthetic speech generated by the author with Microsoft Azure AI Speech (neural voice `da-DK-ChristelNeural`) | Excluded from MIT; used under Microsoft's terms for Azure AI Speech, which require disclosing that the voice is synthetic (shown in the app's footer) |
| `CheckYourNumbers/wwwroot/audio/en/numbers/` (100 files: .mp3) | Synthetic speech generated by the author with Microsoft Azure AI Speech (an `en-GB` neural voice) | Excluded from MIT; as above |
| `CheckYourNumbers/wwwroot/audio/it/numbers/` (100 files: .wav) | Synthetic speech generated by the author with Microsoft Azure AI Speech (an Italian voice), in 2022 | Excluded from MIT; as above |
| `CheckYourNumbers/wwwroot/audio/nl/numbers/` (100 files: .mp3) | Synthetic speech generated by the author with Microsoft Azure AI Speech (neural voice `nl-NL-FennaNeural`) | Excluded from MIT; as above |
| `CheckYourNumbers/wwwroot/audio/de/numbers/`, `audio/ch/numbers/`, `audio/fr/numbers/`, `audio/es/numbers/` (100 files each: .mp3; copies in `tools/tts/audio/`) | Synthetic speech generated by the author with edge-tts (Microsoft's online text-to-speech) and the neural voices `de-DE-AmalaNeural`, `de-CH-LeniNeural`, `fr-FR-DeniseNeural` and `es-ES-ElviraNeural` | Excluded from MIT; used under Microsoft's terms, which require disclosing that the voices are synthetic (shown in the app's footer) |
| `tools/tts/numbers.txt`, `tools/tts/words-*.txt`, `tools/tts/WordsToMp3.py`, `tools/tts/WordsToMp3.bat` | Written by the author, or generated from the app's own code, as input for speech synthesis | MIT (as part of this project) |

The recordings are not part of the MIT licence and must not be extracted and redistributed as an audio dataset or used to train speech models.

## Trademarks

Microsoft, Windows, .NET, Blazor and other Microsoft product names are trademarks of the Microsoft group of companies. All other product names, logos and brands mentioned in this repository are the property of their respective owners. They are used only to identify the services and tools this project works with, and their use does not imply endorsement.
