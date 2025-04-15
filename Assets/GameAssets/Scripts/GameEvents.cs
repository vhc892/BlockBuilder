using System;

public static class GameEvents
{
    public static event Action onCheckWin;
    public static void CheckWin()
    {
        onCheckWin?.Invoke();
    }

    public static event Action onLevelComplete;
    public static void LevelComplete()
    {
        onLevelComplete?.Invoke();
    }


    public static event Action onLevelStart;
    public static void LevelStart()
    {
        onLevelStart?.Invoke();
    }

    public static event Action onLevelRestart;
    public static void LevelRestart()
    {
        onLevelRestart?.Invoke();
    }

    public static event Action onLevelLose;
    public static void LevelLose()
    {
        onLevelLose?.Invoke();
    }

    public static event Action onShapeTouch;
    public static void ShapeTouch()
    {
        onShapeTouch?.Invoke();
    }
}
