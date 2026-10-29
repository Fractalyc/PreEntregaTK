using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    private const string managerSceneName = "Inicio";
    private const int startingLives = 4;

    [System.Serializable]
    public class Microgame
    {
        public string sceneName;
        public string prompt;
    }

    [SerializeField] private float introDuration = 5f;
    [SerializeField] private float resultDisplayTime = 1.5f;

    [SerializeField] private Animator introAnimator;
    [SerializeField] private string introTrigger = "Intro";
    [SerializeField] private float promptDuration = 2f;

    [SerializeField] private TMP_Text promptText;
    [SerializeField] private Microgame[] microgames;

    [SerializeField] private Image[] lifeIcons;
    [SerializeField] private Sprite lifeSprite;
    [SerializeField] private Sprite brokenLifeSprite;
    [SerializeField] private TMP_Text completedText;
    [SerializeField] private TMP_Text messageText;
    [SerializeField] private GameObject gameOverPanel;

    private static int lives = startingLives;
    private static int completed = 0;
    private static bool hasLastResult = false;
    private static bool lastWon = false;
    private static int lastMicrogame = -1;

    private static int roundTarget = 0;          //  (0 = sin límite)
    private static int playedThisRun = 0;
    private static string returnSceneName = "Mundo";

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
    private static void ResetOnPlay()
    {
        ResetGameState();
    }

    private static void ResetGameState()
    {
        lives = startingLives;
        completed = 0;
        hasLastResult = false;
        lastWon = false;
        lastMicrogame = -1;
        roundTarget = 0;
        playedThisRun = 0;
        returnSceneName = "";
    }

    public static void StartRun(int microgameCount)
    {
        ResetGameState();
        roundTarget = microgameCount;
        returnSceneName = SceneManager.GetActiveScene().name;
        SceneManager.LoadScene(managerSceneName);
    }

    public static void ReportResult(bool won)
    {
        if (won)
        {
            completed++;
            AudioManager.Instance.PlaySound(AudioManager.Instance.winSound);
        }
        else
        {
            lives--;
            AudioManager.Instance.PlaySound(AudioManager.Instance.loseSound);
        }

        lastWon = won;
        hasLastResult = true;
        playedThisRun++;
        SceneManager.LoadScene(managerSceneName);
    }

    private void Start()
    {
        if (gameOverPanel != null) gameOverPanel.SetActive(false);
        SetMessage("");
        SetPrompt("");
        StartCoroutine(RunCycle());
    }

    private IEnumerator RunCycle()
    {
        if (hasLastResult)
        {
            UpdateLifeIcons(lives);
            if (!lastWon) ShowBrokenLife(lives);
            UpdateCompletedText(lastWon ? completed - 1 : completed);
            SetMessage(lastWon ? "¡GANASTE!" : "PERDISTE");

            yield return new WaitForSeconds(resultDisplayTime);
            hasLastResult = false;
        }

        UpdateLifeIcons(lives);
        UpdateCompletedText(completed);
        SetMessage("");

        if (lives <= 0)
        {
            AudioManager.Instance.PlaySound(AudioManager.Instance.loseSound);
            ShowGameOver();
            yield break;
        }

        if (roundTarget > 0 && playedThisRun >= roundTarget)
        {
            SetMessage("¡LISTO!");
            yield return new WaitForSeconds(resultDisplayTime);
            FinishRun();
            yield break;
        }

        if (microgames == null || microgames.Length == 0)
        {
            Debug.LogError("Falta llenar la lista Microgames en el GameManager.");
            yield break;
        }

        int index = PickRandomMicrogame();

        if (introAnimator != null)
        {
            introAnimator.SetTrigger(introTrigger);
        }

        float promptTime = Mathf.Min(promptDuration, introDuration);
        yield return new WaitForSeconds(introDuration - promptTime);

        SetPrompt(microgames[index].prompt);
        yield return new WaitForSeconds(promptTime);

        SceneManager.LoadScene(microgames[index].sceneName);
    }

    private int PickRandomMicrogame()
    {
        int index;
        do
        {
            index = Random.Range(0, microgames.Length);
        }
        while (index == lastMicrogame && microgames.Length > 1);

        lastMicrogame = index;
        return index;
    }

    private void ShowGameOver()
    {
        SetMessage("GAME OVER");
        if (gameOverPanel != null) gameOverPanel.SetActive(true);
    }

    public void RestartGame()
    {
        lives = startingLives;
        completed = 0;
        playedThisRun = 0;
        hasLastResult = false;
        lastMicrogame = -1;
        SceneManager.LoadScene(managerSceneName);
    }

    public void ExitRun()
    {
        if (string.IsNullOrEmpty(returnSceneName)) return;
        FinishRun();
    }

    private void FinishRun()
    {
        string scene = returnSceneName;
        roundTarget = 0;
        playedThisRun = 0;
        returnSceneName = "";
        SceneManager.LoadScene(scene);
    }

    private void UpdateLifeIcons(int shownLives)
    {
        for (int i = 0; i < lifeIcons.Length; i++)
        {
            lifeIcons[i].sprite = lifeSprite;
            lifeIcons[i].enabled = i < shownLives;
        }
    }

    private void ShowBrokenLife(int index)
    {
        if (index < 0 || index >= lifeIcons.Length) return;

        lifeIcons[index].sprite = brokenLifeSprite;
        lifeIcons[index].enabled = true;
    }

    private void UpdateCompletedText(int value)
    {
        if (completedText != null) completedText.text = value.ToString();
    }

    private void SetMessage(string text)
    {
        if (messageText != null) messageText.text = text;
    }

    private void SetPrompt(string text)
    {
        if (promptText != null) promptText.text = text;
    }
}