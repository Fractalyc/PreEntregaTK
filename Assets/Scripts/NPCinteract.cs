using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(DialogueTrigger))]
public class NpcInteraction : MonoBehaviour
{
    [SerializeField] private Key interactKey = Key.Space;
    [SerializeField] private float interactRadius = 1.5f;   // distancia a la que se puede hablar
    [SerializeField] private int microgamesToPlay = 3;
    [SerializeField] private float startDelay = 0.6f;       // pausa para ver cerrarse la caja de diálogo
    [SerializeField] private GameObject interactPrompt;     // opcional: un cartelito que avisa que se puede hablar

    private DialogueTrigger dialogueTrigger;
    private DialogueManager dialogueManager;
    private PlayerController player;
    private bool talking = false;

    private void Awake()
    {
        dialogueTrigger = GetComponent<DialogueTrigger>();
    }

    private void Start()
    {
        player = FindFirstObjectByType<PlayerController>();
        if (player == null)
        {
            Debug.LogError("No hay un PlayerController en la escena.");
        }

        SetPrompt(false);
    }

    private void Update()
    {
        if (player == null) return;

        // Cerca del NPC = dentro del radio (no depende de colliders ni triggers)
        bool inRange = !talking &&
            Vector2.Distance(player.transform.position, transform.position) <= interactRadius;

        SetPrompt(inRange);

        if (Keyboard.current == null) return;
        if (!Keyboard.current[interactKey].wasPressedThisFrame) return;

        if (talking)
        {
            // Con la misma tecla también se avanza el diálogo (además del botón Continue)
            if (dialogueManager != null) dialogueManager.DisplayNextSentence();
        }
        else if (inRange)
        {
            StartTalking();
        }
    }

    private void StartTalking()
    {
        talking = true;
        SetPrompt(false);

        // El jugador no se mueve mientras se habla
        player.enabled = false;

        // Muestra la UI del diálogo a través del GameStateManager
        if (GameStateManager.Instance != null)
        {
            GameStateManager.Instance.ChangeState(GameState.Dialogue);
        }

        StartCoroutine(BeginDialogue());
    }

    private IEnumerator BeginDialogue()
    {
        // El GameStateManager activa la UI con un retraso (delay), así que
        // se espera a que el DialogueManager esté activo para poder encontrarlo
        float waited = 0f;
        dialogueManager = FindFirstObjectByType<DialogueManager>();
        while (dialogueManager == null && waited < 5f)
        {
            waited += Time.unscaledDeltaTime;
            yield return null;
            dialogueManager = FindFirstObjectByType<DialogueManager>();
        }

        if (dialogueManager == null)
        {
            Debug.LogError("No se encontró un DialogueManager activo. Revisa que esté en la UI del estado Dialogue.");
            CancelTalking();
            yield break;
        }

        dialogueManager.OnDialogueEnded += HandleDialogueEnded;
        dialogueTrigger.TriggerDialogue();
    }

    private void CancelTalking()
    {
        talking = false;
        player.enabled = true;

        if (GameStateManager.Instance != null)
        {
            GameStateManager.Instance.ChangeState(GameState.Overworld);
        }
    }

    private void HandleDialogueEnded()
    {
        dialogueManager.OnDialogueEnded -= HandleDialogueEnded;

        // Por si el GameStateManager lo reactivó al cerrar el diálogo
        player.enabled = false;

        StartCoroutine(StartMicrogames());
    }

    private IEnumerator StartMicrogames()
    {
        yield return new WaitForSecondsRealtime(startDelay);

        // Se guarda dónde está para devolverlo ahí al volver
        OverworldState.playerPosition = player.transform.position;
        OverworldState.hasSaved = true;
        OverworldState.returningFromRun = true;

        GameManager.StartRun(microgamesToPlay);
    }

    private void SetPrompt(bool visible)
    {
        if (interactPrompt != null && interactPrompt.activeSelf != visible)
        {
            interactPrompt.SetActive(visible);
        }
    }

    // Dibuja el radio en la vista Scene al seleccionar el NPC
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, interactRadius);
    }
}
