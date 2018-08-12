# FFXIVRichPresence
A FFXIV Rich Presence mod via memory.

Since, due to discord's limits(512 assets), it's not possible to have cover pictures for every single zone, I decided to group pictures by Area(e.g. Kugane, La Noscea, Dravania) and all jobs.

Components:
* FFXIVRichPresenceRunner: Reads game memory, fetches needed data via xivapi and updates discord presence via [DiscordRPC](https://github.com/Lachee/discord-rpc-csharp)
* dump64: Gets ran by the game if in the game folder and launches FFXIVRichPresenceRunner.
