# FPSController
The goal of this controller was to mimic how Valves character controller feels in TF2.

Notable features:

## PlayerController
- Smooth crouching
- Can walk up and down hills smoothly
- If a slope is too steep the controller will slide down it
- Double jumping can be increased to any number when enabled
- Can be affected by forces, Ex: Explosion force, Jump Pads, etc.
- Can move up and down triggers tagged as Ladder
- Can swim in triggers tagged as Water, will drift slowly down if no input detected

## Player Camera
- Aim punch * Will be revised soon *
- Camera wobble * Will be revised soon *

### Future Goals
- Network when Unity releases their new network API
