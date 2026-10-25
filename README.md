# NekosVersionTitle

A Risk of Rain 2 mod that replaces the main menu logo with the Neko's Version logo.

Client-side only, no configuration needed.

## Installation

Install it with a mod manager such as [r2modman](https://thunderstore.io/package/ebkr/r2modman/), or manually:

1. Install [BepInExPack](https://thunderstore.io/c/riskofrain2/p/bbepis/BepInExPack/) and [HookGenPatcher](https://thunderstore.io/c/riskofrain2/p/RiskofThunder/HookGenPatcher/).
2. Copy `NekosVersionTitle.dll` into `BepInEx/plugins`.

## Building

Requires the .NET SDK.

```sh
dotnet build -c Release
```

The output is written to `NekosVersionTitle/bin/Release/netstandard2.1/NekosVersionTitle.dll`.
The logo is embedded in the assembly from [`NekosVersionTitle/Resources/Logo.png`](NekosVersionTitle/Resources/Logo.png).

## Packaging for Thunderstore

The [`Thunderstore`](Thunderstore) folder contains the package files (`manifest.json`, `icon.png`, `README.md`, `CHANGELOG.md`).
Copy the built DLL into `Thunderstore/plugins/` and zip the contents of the folder to create the package.

## License

The code is licensed under the [MIT License](LICENSE).

The logo (`NekosVersionTitle/Resources/Logo.png`) is not covered by the MIT License.
It is a modified version of the Risk of Rain 2 logo, which belongs to Gearbox Software and Hopoo Games.
This is an unofficial fan project and is not affiliated with or endorsed by them.
