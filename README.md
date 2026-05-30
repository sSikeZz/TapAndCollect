# Tap And Collect

Tap And Collect is a small Unity 2D WebGL game where you move a basket, catch good items, avoid bad ones, and try to last until the timer runs out.

## Running The Game

- Unity version: `6000.3.10f1`

## How It Is Built

The game is split into small scripts so it is easy to follow and tune. `GameManager` handles score, lives, timer, state, bonuses, and the difficulty ramp. `FallingObjectSpawner` handles weighted item spawning, while `FallingObject` controls movement, collision, trails, and despawning. `Basket` handles player movement and catch feedback.

Prefabs are in `Assets/Prefabs`, and item/game tuning is done with ScriptableObjects in `Assets/ScriptableObjects`. This avoids duplicating GameObjects and makes it easier to rebalance the game from the Inspector.

## Feel And Performance

I added some extra juice to make the game feel more responsive: basket tilt and squash/stretch, item wobble and spawn pop, camera shake, screen flashes, particles, pitch-varied sounds, score/life UI pops, and a short delay before the game-over screen appears.

For performance, collection particles are pooled, the falling objects are simple 2D trigger objects, and most feedback is handled with lightweight per-frame values instead of creating lots of temporary objects.

## AI Usage

I used AI lightly as a support tool while working on the project. Mainly, it helped me review the code structure, think through a few game-feel ideas, and clean up parts of this README. The actual implementation stayed within the existing Unity setup and project structure.

Areas where AI was used for support:

- `Assets/Scripts/Gameplay/Basket.cs`
- `Assets/Scripts/Gameplay/FallingObject.cs`
- `Assets/Scripts/Gameplay/FallingObjectSpawner.cs`
- `Assets/Scripts/Gameplay/GameManager.cs`
- `Assets/Scripts/Data/GameSettings.cs`
- `Assets/Scripts/Feedback/GameFeedbackController.cs`
- `Assets/Scripts/UI/HudView.cs`


