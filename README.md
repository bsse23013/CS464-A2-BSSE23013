# CS464 Assignment 02 · Muhammad Sarmad Ahmad · BSSE23013

- **Video:** https://drive.google.com/file/d/1vgxCzGGIaaYDg7PZmaMagFUtj6dvMZ4A/view?usp=vids_web
- **Input:** Legacy Input with Active Input Handling set to Both
- **Ground check:** Contact normal (OnCollisionStay, normal.y > 0.5), with a gizmo ray in OnDrawGizmosSelected
- **Camera:** SmoothDamp with look-ahead
- **Juice:** Squash on landing, Ease.OutBack, started on a landing after more than 0.1 s in the air

## Tuning

| Setting | Value | Why this number |
|---|---|---|
| Speed | 5 m/s | It crosses a 3 m gap in 0.6 s, less than the 1.02 s of air time, so the jump carries over the gap. |
| Jump height | 1.8 m | Clears the 1.2 m ledges with 0.6 m to spare. |
| Time to apex | 0.61 s | Follows from the jump height: v / g = 5.94 / 9.81. |
| Fall multiplier | 2.2× | Same apex, but the fall takes 0.41 s instead of 0.61 s, so the jump feels snappier. |
| Coyote time | 0.1 s | At 5 m/s that is 0.5 m past the edge, enough for a late press at a run-off jump. |
| Jump buffer | 0.12 s | At 5 m/s that is 0.6 m before landing, forgiving but not sloppy. |
| Camera smoothing | 0.3 s (smoothTime) | SmoothDamp alone trails the player by about 1.5 m at 5 m/s, which keeps the motion soft. |
| Look-ahead or dead zone | 0.35 s | 5 m/s × 0.35 s = 1.75 m ahead, which cancels the 1.5 m trail so the camera leads slightly. |

## Four answers

1. **The jump, in numbers.** The launch speed is v = √(2gh) = √(2 × 9.81 × 1.8) ≈ 5.94 m/s, so jumpForce on a 1 kg body is 5.94. The time to apex is v / g ≈ 0.61 s. The fall takes 0.61 / √2.2 ≈ 0.41 s, so the whole jump lasts about 1.02 s. At 5 m/s one jump carries me about 5 × 1.02 ≈ 5.1 m, which clears my widest gap of 3 m.
2. **Update and FixedUpdate.** In Update I read the keys, build the camera-relative move vector, stamp the coyote and buffer times, queue the jump and start the squash. In FixedUpdate I set rb.linearVelocity from move (keeping y), apply the queued jump once and add the extra fall gravity. If I swapped them, GetKeyDown is true for only one frame and at 144 fps about two frames in three have no FixedUpdate, so presses would be lost. Setting physics from Update would also make movement depend on the frame rate.
3. **Vector3.Dot.** In OnCollisionStay, `Vector3.Dot(contact.normal, Vector3.up) > 0.5f`. The sign shows which way the surface faces: positive means it faces up, so it counts as ground, near 0 is a wall, and negative is a ceiling.
4. **The juice.** The squash uses Ease.OutBack over 0.25 s, which is 15 frames at 60 fps. OutBack overshoots a little and settles, so the landing feels springy, and 0.25 s is short enough not to get in the way of the next jump.

## Levels

| Level | What the controller must do there | What I changed for this jump |
|---|---|---|
| Level01 | Walk to the goal and jump through a gap of around 1.6 meters.  | Nothing: the 1.8 m jump clears 1.6 m gap |
| Level02 | Cross a narrow 1.5 meter wide pathway and 1.2 m jump for coin  | Nothing: the 1 meter wide can pass through this |
| Level03 | Jump a gap, then climb two 1.2 m ledges | Nothing: both are within the jump |
| Level04 | Go up a 45 degree walkway , jump a gap and go upstairs | Nothing: the 0.1 s window is 0.5 m past the edge |
| Level05 | Jump down and forward up , then go upstairs and jump over a gap | Nothing: all jumps fit the 1.8 m height and 5.1 m reach |
