using UnityEngine;
using System.Collections.Generic;
using System;
using UnityEngine.Assertions;
using static UIButton;

// The difference between GameplayManager and GameStateManager is a little muddy, but essentially Gameplay Manager is directly responsibile for all events that occur inside while the game is actively being played, whereas gamestate manager is responsible for the overall global state of the game, even when it's not being played.
// This functionality could definitely be merged and moved over to the GameStateManager.
// I definitely feel a lot of confusion otherwise.
[Obsolete]
public class GameplayManager : MonoBehaviour
{
}
