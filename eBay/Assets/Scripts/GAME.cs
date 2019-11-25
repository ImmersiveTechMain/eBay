using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Timer = SkeetoTools.Timer;

public static class GAME
{
    public delegate void CALLBACK();
    public static CALLBACK OnGameStarted = delegate () { };

    public static string UDP_GameCompleted = "GAME_COMPLETED";
    public static string UDP_GameLost = "GAME_LOST";
    public static string UDP_GameStart = "GAME_START";
    public static string UDP_GameReset = "GAME_RESET";
    public static string UDP_ReduceMusicVolume = "SET_MUSIC_VOLUME_LOW";
    public static string UDP_IncreaseMusicVolume = "SET_MUSIC_VOLUME_HIGH";
    public static string UDP_SetGameTimer = "SET_GAME_TIMER_"; // SET_GAME_TIMER_X  where X is seconds

    public static Timer timer;
    public static float duration = 600;
    public static bool gameHasStarted = false;
    public static bool gameHasEnded = false;

    public static void SetGameDuration(float duration)
    {
        if (!gameHasStarted || gameHasEnded)
        {
            GAME.duration = duration;
        }
        else
        {
            Debug.Log("Can't set the game duration while it is running");
        }
    }

    public static void StartGame()
    {
        if (!gameHasStarted)
        {
            if (timer != null) { timer.OnTimerEnds = () => { }; timer.Stop(); }
            timer = new Timer(duration);
            timer.Run();
            gameHasEnded = false;
            gameHasStarted = true;
            OnGameStarted?.Invoke(); // same as: if (OnGameStarted != null) { OnGameStarted(); }
        }
    }

    public static void ResetGame()
    {
        OnGameStarted = delegate () { };
        gameHasStarted = false;
        gameHasEnded = false;
        if (timer != null) { timer.OnTimerEnds = () => { }; timer.OnTick = delegate () { }; timer.Stop(); timer = null; }
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}