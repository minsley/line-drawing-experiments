# Line Drawing Experiments

## Dependencies
1. git & git-lfs
2. Unity (tested with 2021.2.10f1 [Apple Silicon])
3. [Shapes](https://assetstore.unity.com/packages/tools/particles-effects/shapes-173167) 
   1. Note: has some [extra install steps](#shapesSetupNote)

## Notes
1. <a id="shapesSetupNote"/>This project relies on [Freya Holmer's](https://github.com/FreyaHolmer) excellent vector graphics library [Shapes](https://assetstore.unity.com/packages/tools/particles-effects/shapes-173167). I moved it into a local package so I could make this repo public without rehosting the library. If you want to run this project, you'll need to buy a copy for yourself, and then do **either**:
   * Install Shapes to `Assets` (as per Freya's recommendation) then remove the local path dependency from `Packages/manifest.json`, or
   * Convert it into a package, by renaming `Shapes/` to `com.acegikmo.shapes/` (all the package bits actually seem to be in there already) and point `Packages/manifest.json` to its new location. By default I keep it in a sibling directory to this project called `_lib`.
