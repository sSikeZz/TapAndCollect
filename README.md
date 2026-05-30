# Tap And Collect

## Overview

Tap And Collect is a casual 2D Unity WebGL game where the player controls a basket to catch good items, avoid bad items, and survive until the timer reaches zero. The project was developed with a focus on clean architecture, reusable prefabs, responsive gameplay, and WebGL performance.

---

## Unity Version

Unity 6000.3.10f1

---

## Opening The Project

1. Clone or download the repository.
2. Open Unity Hub.
3. Click "Add Project" and select the project folder.
4. Open the project using Unity 6000.3.10f1.
5. Open the main scene located in `Assets/Scenes`.
6. Press Play in the Unity Editor or create a WebGL build using the Build Settings window.

---

## Project Structure

The project is divided into separate gameplay systems to keep responsibilities isolated and maintainable.

### Core Systems

* `GameManager`
  * Controls score, lives, timer, difficulty progression, bonuses, and game state.

* `FallingObjectSpawner`
  * Handles weighted spawning of collectibles, hazards, and bonus items.

* `FallingObject`
  * Controls falling behaviour, collisions, effects, and despawning.

* `Basket`
  * Handles player input, movement, collection feedback, and animations.

### Data

ScriptableObjects are used for item configuration, allowing gameplay values to be modified without changing code.

### Prefabs

The project uses reusable prefabs and prefab variants to avoid duplication and simplify content management.

---

## Design Patterns Used

### Singleton Pattern

Used in:

* `GameManager`

Reason:

The game requires a single central authority for score, lives, timer, difficulty progression, and overall game state. Using a Singleton provides easy access while preventing multiple instances.

### Observer Pattern

Used in:

* UI updates
* Score updates
* Life updates
* Timer updates
* Collection feedback

Reason:

Gameplay systems communicate through events rather than direct references. This reduces coupling between systems and makes the project easier to extend and maintain.

### Object Pooling Pattern

Used in:

* Collection particle effects

Reason:

Instead of repeatedly creating and destroying particle systems, pooled objects are reused to reduce allocations and improve runtime performance.

---

## Feel And Polish

Several gameplay feedback systems were added to improve responsiveness and player satisfaction:

* Basket tilt while moving
* Squash and stretch animations
* Falling item wobble motion
* Spawn pop animations
* Camera shake
* Screen flashes
* Particle effects
* Animated score and life UI
* Delayed game-over reveal

These additions were designed to create a more polished and satisfying gameplay experience without significantly increasing complexity.

---

## Trade-Offs Made

Due to the assignment scope and timeline, several decisions were made to prioritize code quality and polish over feature count.

* Focused on a single polished game mode instead of multiple modes.
* Kept gameplay rules simple to maintain clarity and accessibility.
* Used lightweight visual effects rather than more expensive graphical systems.
* Prioritized maintainable architecture and responsiveness over additional content.

These trade-offs allowed more time to be spent on gameplay feel, structure, and performance.

---

## What I Would Improve With More Time

If given additional development time, I would consider adding:

* Progressive difficulty levels
* Combo and streak systems
* Additional item behaviours
* More bonus power-ups
* Achievement and progression systems
* High score saving
* Additional visual effects and animations
* Mobile-specific polish and balancing

---

## AI Usage

AI was used as a supporting tool during development.

Usage included:

* Reviewing architecture ideas
* Discussing gameplay polish concepts
* Verifying implementation approaches
* Refining documentation

All gameplay systems, project setup, integration, and final testing were handled within Unity during development.
