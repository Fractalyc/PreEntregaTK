using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem.LowLevel;
using UnityEngine.InputSystem.XR.Haptics;

public enum GameState
{
    MainMenu,
    Overworld,
    Dialogue,
    Minigames,
    BossGame,
    Paused,
    GameOver,
    Credits,
    Config
}

//On Click functions further down.

public class GameStateManager : MonoBehaviour
{
    public GameObject MainMenuUI;
    public GameObject OverworldUI;
    public GameObject DialogueUI;
    public GameObject MinigamesUI;
    public GameObject BossGameUI;
    public GameObject PausedUI;
    public GameObject GameOverUI;
    public GameObject CreditsUI;
    public GameObject ConfigUI;

    public static GameStateManager Instance;
    public int delay;
    public GameState currentState { get; private set; }
    public GameState previousState { get; private set; }

    private void Awake()
    {
        Instance = this;
    }
    private void Start()
    {
        if (OverworldState.returningFromRun)
        {
            OverworldState.returningFromRun = false;
            ChangeState(GameState.Overworld);
        }
        else
        {
            ChangeState(GameState.MainMenu);
        }
    }

    public void ChangeState(GameState newState)
    {
        if (currentState == newState) return;
        StartCoroutine(TransitionToState(newState));
    }

    private IEnumerator TransitionToState(GameState newState)
    {
        if (newState != GameState.MainMenu)
        {
            yield return new WaitForSecondsRealtime(delay);
        }

        
        if (newState == GameState.Paused && currentState != GameState.Paused)
        {
            previousState = currentState;
        }

        currentState = newState;
        HandleStateChange();
    }

    private void HandleStateChange()
    {
        HideAllMenus();

        Time.timeScale = 1.0f;

        switch (currentState)
        {
            case GameState.MainMenu:
                Time.timeScale = 0;
                MainMenuUI.SetActive(true);
                break;
            case GameState.Overworld:
                OverworldUI.SetActive(true);
                break;
            case GameState.Dialogue:
                DialogueUI.SetActive(true);
                break;
            case GameState.Minigames:
                MinigamesUI.SetActive(true);
                break;
            case GameState.BossGame:
                BossGameUI.SetActive(true);
                break;
            case GameState.Paused:
                PausedUI.SetActive(true);
                Time.timeScale = 0;
                break;
            case GameState.GameOver:
                GameOverUI.SetActive(true);
                Time.timeScale = 0;
                break;
            case GameState.Credits:
                CreditsUI.SetActive(true);
                break;
            case GameState.Config:
                ConfigUI.SetActive(true);
                Time.timeScale = 0;
                break;
        }
    }

    private void HideAllMenus()
    {
        MainMenuUI.SetActive(false);
        OverworldUI.SetActive(false);
        DialogueUI.SetActive(false);
        MinigamesUI.SetActive(false);
        BossGameUI.SetActive(false);
        PausedUI.SetActive(false);
        GameOverUI.SetActive(false);
        CreditsUI.SetActive(false);
        ConfigUI.SetActive(false);
    }

    public void TogglePause()
    {
        if (currentState == GameState.Paused)
        {
            ChangeState(previousState);
        }
        else if (currentState == GameState.Overworld || currentState == GameState.Minigames || currentState == GameState.BossGame || currentState == GameState.Dialogue || currentState == GameState.MainMenu || currentState == GameState.Config)
        {
            ChangeState(GameState.Paused);
        }
    }

    public void ChangeToMainMenu()
    {
        ChangeState(GameState.MainMenu);
    }
    public void ChangeToOverWorld()
    {
        ChangeState(GameState.Overworld);
    }
    public void ChangeToDialogue()
    {
        ChangeState(GameState.Dialogue);
    }
    public void ChangeToMinigames()
    {
        ChangeState(GameState.Minigames);
    }
    public void ChangeToBossGame()
    {
        ChangeState(GameState.BossGame);
    }
    public void ChangeToPaused()
    {
        TogglePause();
    }
    public void ChangeToGameOver()
    {
        ChangeState(GameState.GameOver);
    }
    public void ChangeToCredits()
    {
        ChangeState(GameState.Credits);
    }
    public void ChangeToConfig()
    {
        ChangeState(GameState.Config);
    }

    public void SwitchUI()
    {
        ConfigUI.SetActive(true);
        PausedUI.SetActive(false);
    }
    public void SwitchUIoff()
    {
        ConfigUI.SetActive(false);
        PausedUI.SetActive(true);
    }
    public void SwitchUIoffMain()
    {
        ConfigUI.SetActive(false);
        MainMenuUI.SetActive(true);
    }


    public void QuitGame()
    {
        Application.Quit();

#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#endif
    }
}
