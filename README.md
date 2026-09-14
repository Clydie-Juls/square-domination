# Square Domination

A local 2-player action game built in **Unity and C#** where players fight across multiple sides of a cube using directional movement, independently controlled weapons, power-ups, and environmental hazards.

The project was originally developed and published in 2020 and later released publicly on both **Newgrounds** and **itch.io**.

## Gameplay

Two players compete in the same local session and attempt to eliminate each other while navigating between different sides of the arena.

Core gameplay systems include:

- Local 2-player controls
- Independent player movement and weapon rotation
- Shooting and respawning
- Traversal between different sides of the cube
- Recharge timing when changing surfaces
- Randomly spawning power-ups
- Health restoration and stronger weapon pickups
- Conveyor-belt hazards that affect player movement
- Timed and Endless game modes
- Pause and game-state controls

## Gameplay Architecture

The project uses Unity's component-based `GameObject` model, with gameplay behavior split across player control, weapons, movement, power-ups, environmental interactions, and match-state logic.

Some implementation details include:

- Shared player behavior implemented through inheritance between player controller scripts
- Cube-face transitions calculated using vector math and quaternion rotations
- Movement and simple animations driven through direct vector manipulation
- Independent movement and weapon orientation for each player
- Separate systems for projectiles, health, respawning, power-ups, and game modes

The architecture is intentionally lightweight and reflects the scope of the project rather than using a larger framework or engine abstraction layer.

## Public Release

Square Domination was publicly released on Newgrounds and itch.io.

### Newgrounds

The Newgrounds release received:

- **680+ views**
- **67 user votes**
- **3.28 / 5 rating**

The game was published on December 22, 2020.

[Play on Newgrounds](https://www.newgrounds.com/portal/view/775309)

### itch.io

[Play on itch.io](https://shadowpeterx.itch.io/square-domination)

## Controls

### Player 1

| Action | Control |
|---|---|
| Move | W / A / S / D |
| Rotate weapon left | G |
| Rotate weapon right | H |
| Shoot | Space |

### Player 2

| Action | Control |
|---|---|
| Move | Arrow Keys |
| Rotate weapon left | Keypad 4 |
| Rotate weapon right | Keypad 6 |
| Shoot | Keypad 0 |

Player movement is independent from the direction the weapon is facing, allowing players to move and aim separately.

## Game Modes

### Timed

Players compete within a fixed match duration.

### Endless

Players can continue fighting without a fixed match timer.

Eliminated players respawn and continue playing.

## Technical Overview

Square Domination was built using:

- **Unity**
- **C#**
- Unity physics and collision systems
- Local multiplayer input handling
- Runtime game-state management
- Player health and respawn systems
- Weapon and projectile mechanics
- Randomized item spawning
- Environmental movement mechanics

The project provided hands-on experience building interconnected gameplay systems rather than isolated mechanics.

## Project Structure

```text
.
├── Assets/
│   ├── Scripts/
│   ├── Scenes/
│   ├── Prefabs/
│   ├── Materials/
│   └── ...
├── Packages/
├── ProjectSettings/
└── README.md
```


- It uses the **680+ views / 67 votes** as actual external validation instead of pretending they’re players. Newgrounds currently shows 680 views and 67 votes publicly. :contentReference[oaicite:1]{index=1}
- It describes the actual gameplay instead of the inaccurate “territory capture” language in your current README. :contentReference[oaicite:2]{index=2}
- It frames this correctly as an **older project that was actually shipped**, which is more valuable to a recruiter than trying to make a 2020 Unity project sound like modern distributed systems work.
- It gives your resume agent concrete facts it can safely extract: Unity/C#, local multiplayer, published release, 680+ views, 67 votes, game systems implemented.
