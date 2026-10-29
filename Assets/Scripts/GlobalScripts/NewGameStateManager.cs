/*
using System.Collections;
using UnityEngine;

public enum GameState { MainMenu, Overworld, Dialogue, Minigames, BossGame, Paused, GameOver, Credits }

public class NewGameStateManager : MonoBehaviour
{
    public static NewGameStateManager Instance { get; private set; }

    // References are now private and set dynamically per scene
    private GameObject MainMenuUI;
    private GameObject OverworldUI;
    private GameObject DialogueUI;
    private GameObject MinigamesUI;
    private GameObject BossGameUI;
    private GameObject PausedUI;
    private GameObject GameOverUI;
    private GameObject CreditsUI;

    [Header("Settings")]
    public float delay = 0.5f;
    public GameState currentState { get; private set; }

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        ChangeState(GameState.MainMenu);
    }

    // Call this from your local scene UI container as soon as a scene loads
    public void AssignSceneUI(GameObject main, GameObject overworld, GameObject dialogue,
                              GameObject mini, GameObject boss, GameObject pause,
                              GameObject over, GameObject credits)
    {
        MainMenuUI = main;
        OverworldUI = overworld;
        DialogueUI = dialogue;
        MinigamesUI = mini;
        BossGameUI = boss;
        PausedUI = pause;
        GameOverUI = over;
        CreditsUI = credits;

        // Refresh the current menu view for the new scene
        HandleStateChange();
    }

    public void ChangeState(GameState newState)
    {
        StartCoroutine(TransitionToState(newState));
    }

    private IEnumerator TransitionToState(GameState newState)
    {
        if (newState != GameState.MainMenu)
        {
            yield return new WaitForSecondsRealtime(delay);
        }

        currentState = newState;
        HandleStateChange();
    }

    private void HandleStateChange()
    {
        HideAllMenus();
        Time.timeScale = 1f;

        switch (currentState)
        {
            case GameState.MainMenu: 
                Time.timeScale = 0f; 
                if (MainMenuUI) MainMenuUI.SetActive(true); 
                break;
            case GameState.Overworld: 
                if (OverworldUI) 
                    OverworldUI.SetActive(true); 
                break;
            case GameState.Dialogue: 
                if (DialogueUI) 
                    DialogueUI.SetActive(true); 
                break;
            case GameState.Minigames: 
                if (MinigamesUI) 
                    MinigamesUI.SetActive(true); 
                break;
            case GameState.BossGame: 
                if (BossGameUI) 
                    BossGameUI.SetActive(true); 
                break;
            case GameState.Paused: 
                Time.timeScale = 0f; 
                if (PausedUI) 
                    PausedUI.SetActive(true); 
                break;
            case GameState.GameOver: 
                Time.timeScale = 0f; 
                if (GameOverUI)
                    GameOverUI.SetActive(true); 
                break;
            case GameState.Credits: 
                if (CreditsUI) 
                    CreditsUI.SetActive(true); 
                break;
        }
    }

    private void HideAllMenus()
    {
        if (MainMenuUI) MainMenuUI.SetActive(false);
        if (OverworldUI) OverworldUI.SetActive(false);
        if (DialogueUI) DialogueUI.SetActive(false);
        if (MinigamesUI) MinigamesUI.SetActive(false);
        if (BossGameUI) BossGameUI.SetActive(false);
        if (PausedUI) PausedUI.SetActive(false);
        if (GameOverUI) GameOverUI.SetActive(false);
        if (CreditsUI) CreditsUI.SetActive(false);
    }

    public void ChangeToMainMenu() => ChangeState(GameState.MainMenu);
    public void ChangeToOverWorld() => ChangeState(GameState.Overworld);
    public void ChangeToDialogue() => ChangeState(GameState.Dialogue);
    public void ChangeToMinigames() => ChangeState(GameState.Minigames);
    public void ChangeToBossGame() => ChangeState(GameState.BossGame);
    public void ChangeToPaused() => ChangeState(GameState.Paused);
    public void ChangeToGameOver() => ChangeState(GameState.GameOver);
    public void ChangeToCredits() => ChangeState(GameState.Credits);

}
*/
