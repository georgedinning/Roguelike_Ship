# Roguelike_Ship — Game Design Document

---

## 1. Overview

**Genre:** Top-down 2D ship roguelike

**Elevator Pitch:** Pilot a stationary starship through hazardous space sectors. Progress along a distance bar, encounter asteroid fields and enemy ships, earn upgrades mid-run, and defeat a boss to advance. Manage your ship's power systems and use time controls to outmanoeuvre threats. The ship is a fixed target — survival depends on tactical power allocation and ability timing, not movement.

**Target Audience:** Players who enjoy roguelikes (FTL, Hades), top-down shooters, and games with tactical depth through resource management.

---

## 2. Gameplay Loop

```
Enter Stage → Progress Distance Bar → Warning → Encounter → Defeat → Choose Upgrade → ... → Boss → Defeat Boss → Next Stage
                                                         └── Death → Run Ends ──┘
```

- Each run is a fresh attempt with a base ship.
- The player progresses through a series of stages, each with its own distance bar.
- At fixed points along the bar, encounters trigger (events, enemies, hazards).
- Between encounters, the player can pause and adjust ship systems.
- Defeating a boss clears the stage and moves to the next.
- Death ends the run completely (permadeath — no persistent carry-over between runs).

---

## 3. Stage & Distance Bar

The distance bar (`DistanceBar.cs`) is the core progression axis.

- **Bar Structure:** A vertical (top-to-bottom) progress bar representing the length of the stage.
- **Encounter Points:** Fixed positions along the bar where encounters are placed (e.g. 30%, 60%, 90%, 100% = boss).
- **Encounter Markers:** Each encounter has a HUD icon positioned at its trigger point on the bar. Icons are white (pending), red (active), grey (completed).
- **Warning System:** *(Not yet implemented)* When the ship approaches an encounter point, a visual/audio warning is issued (e.g. "Asteroid field detected ahead", "Hostile signature incoming").
- **Stage Completion:** Reaching 100% triggers the boss encounter.
- **Boss Enemy:** A large, unique enemy ship at the end of each stage with distinct attack patterns.

---

## 4. Time Controls & Strategic Command

The player has command over time and ship systems, enabling tactical play rather than pure reaction.

### Time Controls *(Implemented)*
- **Pause:** Freezes all action (timeScale = 0). Player can assess the situation, toggle systems, and queue decisions.
- **Normal Speed:** Default 1× real-time (timeScale = 1).
- **Fast Forward:** 2× speed (timeScale = 2). Configurable in inspector.
- **Toggle:** Space key, or UI buttons (Pause / Play / Fast).
- **Affects:** All gameplay — physics, cooldowns, stage timer, bullet lifetime, turret rotation.
- *(Future: Slow-mo button as a limited resource)*

### Ship Systems *(Partly implemented)*
The ship has several systems that can be toggled on or off. Each consumes from a limited **power budget**.

**Implemented:**
- **Power Budget System** — `PowerBar` tracks capacity/usage, `PlayerShip` greedily powers on modules at start, shift+click toggles individual modules. Weapons (GatlingGunModule), Shields (ShieldModule), and Sensors (SensorModule) wired up.
- **Sensors** — toggleable on/off via power budget. Each powered sensor contributes `fogStartRadius`, `fogEndRadius`, `radarEndRadius` to the fog of war system. Multiple sensors stack additively.
- **Shields** — cost 2 power. Deployable energy shield that expands outward, absorbs one `"Hazard"` hit per cycle, deals 50 damage to the hazard, then collapses smoothly and recharges. Also blocks hits while expanding, not just when fully deployed. `Shield.cs` on the bubble child forwards trigger collisions to `ShieldModule` on a separate layer to avoid shift+click interference.

*(The ship has no propulsion — see rationale below.)*

### Why the Ship Doesn't Move

The player ship is a fixed platform. It can aim (change direction via mouse cursor) but cannot change its position relative to the world. This is deliberate:

- **Tactical depth comes from resource management, not movement skill.** Power allocation, time control timing, and aim precision are the three skill axes — not dodging or positioning.
- **The world moves, not the ship.** Asteroids drift in, the distance bar progresses, and future enemy ships will approach the player. The player reacts to what the world sends; they don't choose where to go.
- **Keeps the ship as a fixed target.** This enables the grid-based module system (modules placed in a fixed layout around the ship) and keeps the camera as a free-floating observation tool rather than a follow-cam.

---

## 5. Encounters

### Encounter System *(Implemented)*
- Abstract `Encounter` base class with `triggerPoint` (0–1) and `OnTrigger()`.
- Encounters are sorted by trigger point and fired sequentially as the bar progresses.
- `EncounterSpawner` instantiates encounters programmatically and registers them with `GameManager`.
- Each encounter has a HUD marker on the distance bar that turns red on trigger, grey on completion.

### Asteroid Fields *(Implemented)*
- `AsteroidFieldEncounter` spawns asteroids in a band above the player.
- Asteroids float downward, drift sideways, and spin.
- Asteroids can be shot and destroyed (via bullet collision).
- Asteroids self-destroy when they fall below the camera view.
- The encounter auto-completes (grey icon) when all spawned asteroids are destroyed or off-screen.

### Enemy Ships (Combat) *(Not yet implemented)*
- Hostile ships that engage the player.
- Different enemy types with varying behaviours (e.g. chasers, turrets, bombers).
- Defeating an enemy ship rewards an upgrade choice.

### Boss Fights *(Not yet implemented)*
- Unique boss at the end of each stage.
- Multiple phases, distinct attack patterns.
- Defeating the boss advances to the next stage.

---

## 6. Roguelike Progression (Mid-Run Upgrades) *(Not yet implemented)*

There is no persistent currency or meta-progression. All upgrades are earned during the run.

- **Upgrade Sources:** Defeating an encounter (enemy ship or boss) rewards a choice of 2–3 upgrades.
- **Upgrade Categories:**
  - **Weapon Mods:** Fire rate, damage, bullet speed, spread, element effects
  - **Hull:** Max HP, armour, collision damage
  - **Systems:** Increased power budget, system efficiency
  - **Abilities:** Active abilities (e.g. EMP blast, repair drone)
- **Build Synergies:** Upgrades should interact so players can build toward a strategy (e.g. high shield regen + low HP, or glass cannon + speed).
- Upgrades are lost on death.

---

## 7. Controls

| Input | Action |
|---|---|---|
| Mouse move | Aim turret |
| Left mouse (hold) | Fire weapons |
| Shift + Left click | Toggle module power under cursor |
| Space | Pause / unpause time |
| WASD | Pan camera |
| Middle mouse drag | Pan camera (screen-pixel drag) |
| Scroll wheel | Zoom in/out |
| O | Reset camera to ship |
| 1–5 keys | Toggle ship systems on/off (future) |
| *(Future)* | Active ability keybinds |

The ship is stationary and does not use WASD/arrow movement. All tactical depth comes from time controls, power management, and active abilities. Camera controls are free-form (not ship-following).

---

## 8. Visual Style & Feel

- **Art Style:** Lighter, cartoony 2D. Bright colours, clean sprites.
- **URP 2D Lighting:** Soft glow effects for thrusters, muzzle flash, explosions, shield hits.
- **UI:** Clean, readable. Distance bar is prominent. System status shown at a glance.
- **Screen Shake / Feedback:** *(Future)* Juice on hits, kills, and boss phases.
- **Sound:** *(Future)* Upbeat electronic soundtrack, punchy SFX for weapons and impacts.

---

## 9. Appendices / Notes

### Current Implementation State
- Player ship sprite, turret sprites exist.
- `ModGatling` turret aims at cursor and fires bullets, smooth rotation uses `rotationSpeed`.
- `Bullet` prefab with lifetime, damage, collision with asteroids.
- `TimeController` with pause/play/fast states, Space toggle, UI buttons.
- `DistanceBar` UI progress bar functional, encounter markers auto-positioned.
- `GameManager` drives stage timer with singleton, sorted encounter list.
- `Encounter` abstract class with `Fire()`, `MarkCompleted()`, HUD icon lifecycle.
- `AsteroidFieldEncounter` spawns asteroids, auto-completes when cleared.
- `EncounterSpawner` programmatically creates encounters with trigger points.
- `Asteroid` with physics, damage, off-screen cleanup.
- `Player` health, collision damage, invincibility frames, death.
- `ShipModule` base class with `powered`, `powerCost`, `SetPowered()`, `OnPowerChanged`, `OnPowerStateChanged()`.
- `PowerBar` with capacity/usage tracking, segment display, `HasAvailablePower`.
- `GatlingGunModule` with magazine/reload, shift-click toggling, power gating, visual tint.
- `SensorModule` with `fogStartRadius`/`fogEndRadius`/`radarEndRadius`, additively stacks via `FogManager`.
- `FogManager` with per-frame shader param push, radar ring + 8 spokes, entity dot tracking.
- `FogAffected` on entities auto-creates dot on FogDots layer, shows when `fogEnd < distance ≤ radarEnd`.
- `FogOverlayShader` — URP unlit shader with radial alpha from `_FogStart`/`_FogEnd`.
- `CameraController` — free camera with WASD/drag/zoom/O-reset, zoom-scaled speeds.
- Power budget system — modules start `powered=false`, `PlayerShip` greedily powers on in grid order, shift+click toggles.
- `ShieldModule` — deployable energy shield with Expanding/Deployed/Collapsing/Recharging state machine, absorbs one hazard per cycle, auto-cycles while powered.
- `Shield.cs` — forwards trigger collisions from ShieldBubble child to parent ShieldModule, enabling separate hitbox layers.

### Known Gaps (from instructions.md)
- CameraController uses screen-pixel drag (not world-space), feels inconsistent.
- No enemy/AI system (chasers, turrets, bombers).
- No boss fights.
- No warning system before encounter triggers.
- No roguelike upgrade system.
- No stage lifecycle (start/end events, multi-stage progression).
- No Input Action Map integration (uses legacy input).

### Future Feature Priority
| Priority | Feature | Notes |
|---|---|---|
| 1 | Ship systems & power budget | Weapons/shields/sensors toggleable, limited power, 1–5 keys |
| 2 | Warning system | Visual/audio cue before encounter triggers, ties into sensors |
| 3 | Enemy ships (AI combat) | Next encounter type after asteroid fields |
| 4 | Boss fights | Unique boss at 100%, multiple phases |
| 5 | Roguelike upgrades | Mid-run choices after defeating encounters |
| 6 | Stage lifecycle | Start/end events, multi-stage progression, permadeath |
| 7 | Camera world-space drag | Replace screen-pixel drag with world-space drag target |
