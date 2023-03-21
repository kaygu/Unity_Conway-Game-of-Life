using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public static class Manager
{
    public enum GameStateEnum
    {
        Initializing,
        EditMode,
        Running,
        Invalid
    }
    public static GameStateEnum GameState;

    
    public static bool Init()
    {
        // Check if textures / prefabs are not missing
        return true;
    }
}
