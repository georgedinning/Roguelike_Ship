# Roguelike_Ship — Game Design Document

---

## 1. Overview

**Genre:** Top-down 2D ship roguelike

**Elevator Pitch:** Navigate a starship through hazardous space sectors. Progress along a distance bar, encounter asteroid fields and enemy ships, earn upgrades mid-run, and defeat a boss to advance. Manage your ship's power systems and use time controls to outmanoeuvre threats.

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

The distance bar (existing `DistanceBar.cs`) is the core progression axis.

- **Bar Structure:** A vertical (top-to-bottom) progress bar representing the length of the stage.
- **Encounter Points:** Fixed positions along the bar where encounters are placed (e.g. 30%, 60%, 90%, 100% = boss).
- **Warning System:** When the ship approaches an encounter point, a visual/audio warning is issued (e.g. "Asteroid field detected ahead", "Hostile signature incoming").
- **Stage Completion:** Reaching 100% triggers the boss encounter.
- **Boss Enemy:** A large, unique enemy ship at the end of each stage with distinct attack patterns.

---

## 4. Time Controls & Strategic Command

The player has command over time and ship systems, enabling tactical play rather than pure reaction.

### Time Controls
- **Pause:** Freezes all action. Player can assess the situation, toggle systems, and queue decisions.
- **Normal Speed:** Default 1× real-time.
- *(Future: Slow-mo button as a limited resource)*

### Ship Systems
The ship has several systems that can be toggled on or off. Each consumes from a limited **power budget**.

| System | Power | Effect When On |
|---|---|---|
| Weapons | 2 | Turrets can fire |
| Shields | 2 | Absorbs incoming damage, regenerates slowly |
| Engines | 1 | Ship can move |
| Sensors | 1 | Reveals encounter warning details, enemy info |

The player must make trade-offs: power down shields for more weapon uptime, divert power to engines to outrun a threat, etc.

*(System list is extensible — more systems can be added later.)*

---

## 5. Encounters

### Asteroid Fields (Environmental)
- Hazard zone on the distance bar.
- Floating asteroids that damage the ship on contact.
- Navigation matters — the player can try to dodge through or use time controls to pick a safe path.
- Asteroids can be shot and destroyed (costs ammo/cooldown time).

### Enemy Ships (Combat)
- Hostile ships that engage the player.
- Different enemy types with varying behaviours (e.g. chasers, turrets, bombers).
- Defeating an enemy ship rewards an upgrade choice.

### Boss Fights
- Unique boss at the end of each stage.
- Multiple phases, distinct attack patterns.
- Defeating the boss advances to the next stage.

---

## 6. Roguelike Progression (Mid-Run Upgrades)

There is no persistent currency or meta-progression. All upgrades are earned during the run.

- **Upgrade Sources:** Defeating an encounter (enemy ship or boss) rewards a choice of 2–3 upgrades.
- **Upgrade Categories:**
  - **Weapon Mods:** Fire rate, damage, bullet speed, spread, element effects
  - **Hull:** Max HP, armour, collision damage
  - **Systems:** Increased power budget, system efficiency
  - **Abilities:** Active abilities (e.g. dodge roll, EMP blast, repair drone)
- **Build Synergies:** Upgrades should interact so players can build toward a strategy (e.g. high shield regen + low HP, or glass cannon + speed).
- Upgrades are lost on death.

---

## 7. Controls

| Input | Action |
|---|---|
| Mouse move | Aim turret |
| Left mouse (hold) | Fire weapons |
| WASD / Arrow keys | Ship movement |
| Space | Pause / unpause time |
| 1–5 keys | Toggle ship systems on/off |
| *(Future)* | Active ability keybinds |

---

## 8. Visual Style & Feel

- **Art Style:** Lighter, cartoony 2D. Bright colours, clean sprites.
- **URP 2D Lighting:** Soft glow effects for thrusters, muzzle flash, explosions, shield hits.
- **UI:** Clean, readable. Distance bar is prominent. System status shown at a glance.
- **Screen Shake / Feedback:** Juice on hits, kills, and boss phases.
- **Sound:** (Future) Upbeat electronic soundtrack, punchy SFX for weapons and impacts.

---

## 9. Appendices / Notes

### Current Implementation State
- Player ship sprite, turret sprites exist.
- `ModGatling` turret aims at cursor and fires bullets (scene serialized).
- `Bullet` prefab with lifetime and Rigidbody2D.
- `DistanceBar` UI progress bar functional.
- `GameManager` drives stage timer.

### Known Gaps (from instructions.md)
- No enemy/AI system, spawner, collision/damage.
- No health system.
- `ShipModule.cs` is an empty stub.
- `rotationSpeed` in `ModGatling` unused.
- DistanceBar positioning may have off-by-one bugs.
- No Input Action Map integration (uses legacy input).
