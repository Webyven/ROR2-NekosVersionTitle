# NekosVersionTitle

Replaces the Risk of Rain 2 main menu logo with the Neko's Version logo, and shows the modpack version under the game's build number.

Client-side only.

## Configuration

Edit `BepInEx/config/Weby.NekosVersionTitle.cfg` (created after the first launch):

| Section | Option | Default | Description |
| --- | --- | --- | --- |
| Logo | ReplaceLogo | `true` | Replace the main menu logo |
| Logo | CustomLogoPath | *(empty)* | PNG to use instead of the built-in logo. Relative paths start from `BepInEx/config` |
| VersionText | ShowVersionText | `true` | Show the version text on the main menu |
| VersionText | Text | `Neko's Version` | Text to show, e.g. `Neko's Version v1.2` |
