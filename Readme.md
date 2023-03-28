# Stacklands BugFixes Mod

Fixes various bugs in the game:

- Fixes Well Fed buff only getting applied when not already present
- Fixes Crane arrows sometimes pointing to the wrong target (i.e. not where the card will actually end up)
- Prevents Cranes from attempting to put cards on stacks with ongoing crafting (which wouldn't succeed)
- Fixes Crane movement when the top card on the output stack moves away during movement
- Fixes Sawmills and Brickyards detaching from Magic Glue after crafting is complete
- Fixes Stable Portals detaching from Magic Glue after placing villagers on it
- Fixes the display resolution sometimes not loading correctly (e.g. when the resolution is set to 1080p on a 4k monitor, it sometimes reverts to 4k)

Arguably not a bug:

- Allow the Demon sword to drop Demon Swords even if one is already on the board. The devs clearly intentional made it this way so it's definitely not a bug but you can already avoid this anyway by shipping the other swords
  to the island before the fight so imo it's a rather dumb restrictions.

## Manual Installation

This mod requires BepInEx to work. BepInEx is a modding framework which allows multiple mods to be loaded.

1. Download and install BepInEx from the [Thunderstore](https://stacklands.thunderstore.io/package/BepInEx/BepInExPack_Stacklands/).
2. Download this mod and extract it into `BepInEx/plugins/`
3. Launch the game

## Development

1. Install BepInEx
2. This mod uses publicized game DLLs to get private members without reflection
   - Use https://github.com/CabbageCrow/AssemblyPublicizer for example to publicize `Stacklands/Stacklands_Data/Managed/GameScripts.dll` (just drag the DLL onto the publicizer exe)
   - This outputs to `Stacklands_Data\Managed\publicized_assemblies\GameScripts_publicized.dll` (if you use another publicizer, place the result there)
3. Compile the project. This copies the resulting DLL into `<GAME_PATH>/BepInEx/plugins/`.
   - Your `GAME_PATH` should automatically be detected. If it isn't, you can manually set it in the `.csproj` file.
   - If you're using VSCode, the `.vscode/tasks.json` file should make it so that you can just do `Run Build`/`Ctrl+Shift+B` to build.

## Links

- Github: https://github.com/benediktwerner/Stacklands-BugFixes-Mod
- Thunderstore: https://stacklands.thunderstore.io/package/benediktwerner/BugFixes

## Changelog

- v1.2.2: Fix resolution fix in windowed mode
- v1.2.1: Update Readme
- v1.2.0:
  - Fixes Well Fed buff only getting applied when not already present
  - Fixes Crane arrows sometimes pointing to the wrong target (i.e. not where the card will actually end up)
  - Prevents Cranes from attempting to put cards on stacks with ongoing crafting (which wouldn't succeed)
  - Fixes Crane movement when the top card on the output stack moves away during movement
  - Fixes Sawmills and Brickyards detaching from Magic Glue after crafting is complete
  - Fixes Stable Portals detaching from Magic Glue after placing villagers on it
- v1.1.4: Remove all the now unnecessary bug fixes and the mob drop fix (since it reduces Magic Dust gain a lot and probably isn't that useful otherwise)
- v1.1.3:
  - Remove now unneded "heal lowest" fix (got fixed in the game)
  - Allow the Demon to drop a Demon Sword while another one is on the board (you can already do this anyway by shipping them to the island before the fight)
- v1.1.2: Update Readme (clarify mob-specific drops adjustment)
- v1.1.1: Fix drag into combat
- v1.1
  - When dragging a stack onto a fight, add all cards to it
  - Allow getting the combat intro pack (with `Rumor: Combat`) on any pack after the 10th instead of only exactly the 10th (for saves created before the Witch Forest update or when you buy multiple packs before opening them in which case there won't be one counted as exactly the 10th)
  - Don't limit mob-specific drops to one per "attack" and block them while there are still enemies on the board
- v1.0: Initial release
