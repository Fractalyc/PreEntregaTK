/*
using UnityEngine;

public class SceneUIHolder : MonoBehaviour
{
    [Header("Scene Specific UI Elements")]
    public GameObject MainMenuUI;
    public GameObject OverworldUI;
    public GameObject DialogueUI;
    public GameObject MinigamesUI;
    public GameObject BossGameUI;
    public GameObject PausedUI;
    public GameObject GameOverUI;
    public GameObject CreditsUI;

    private void Start()
    {
        // Feed this scene's unique UI references into the persistent state manager
        if (NewGameStateManager.Instance != null)
        {
            NewGameStateManager.Instance.AssignSceneUI
            (
                MainMenuUI, OverworldUI, DialogueUI,
                MinigamesUI, BossGameUI, PausedUI,
                GameOverUI, CreditsUI
            );
        }
    }
}
*/

