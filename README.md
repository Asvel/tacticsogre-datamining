# Tactics Ogre Reborn Datamining

Datamining related things for Tactics Ogre: Reborn.


## [imhex-patterns](./imhex-patterns)

Hex patterns for using with the [ImHex Hex Editor](https://imhex.werwolv.net/).

To use it, add the imhex-patterns folder to `ImHex -> Help -> Settings -> Folders`.

Note: due to a bug, pattern auto-loading won't work on Windows before ImHex v1.25.0.

Most useful data in Reborn editon is obfuscated, deobfuscation is needed before using these patterns.


## [extracts](./extracts)

* [Deobfuscate](./extracts/Deobfuscate): a basic game data deobfuscator.
* [ExtractText/GameTextEncoding](./extracts/ExtractText/GameTextEncoding.cs): a game text decoder (for human).
* [ExtractScreenplay#L24](./extracts/ExtractScreenplay/ExtractScreenplay.cs#L24): inferred name of some global flags.
* [ExtractScreenplay#L196](./extracts/ExtractScreenplay/ExtractScreenplay.cs#L196): screenplay invk/task instruction parameters.
