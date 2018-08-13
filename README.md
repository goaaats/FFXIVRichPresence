# FFXIVRichPresence
A FFXIV Rich Presence mod via memory.

![demo](https://i.imgur.com/lJeizBv.png)

Since, due to discord's limits(512 assets), it's not possible to have cover pictures for every single zone, I decided to group pictures by Area(e.g. Kugane, La Noscea, Dravania) and all jobs.

Currently, only ffxiv_dx11 is supported.

Displays:
* Character Name + FC Tag
* Character Job + Level
* Character Server
* Current Zone's name and region preview picture

Ideas:
* Read player state flags and update the state: e.g. crafting, in party, in content, in battle, etc.

Components:
* FFXIVRichPresenceRunner: Reads game memory via [memory.dll](https://github.com/erfg12/memory.dll), fetches needed data via xivapi and updates discord presence via [DiscordRPC](https://github.com/Lachee/discord-rpc-csharp)
* dump64: Gets ran by the game if in the game folder and launches FFXIVRichPresenceRunner.

Maintenance:
* Fix pointers and offsets in Definitions.cs and definitions.json for live updates
* Add new cover images for areas: zone_{zone name with all symbols and spaces removed}
