using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class DialogueManager : MonoBehaviour
{
    public GameStateManager gameStateManager;

    public Queue<string> sentences;

    public TextMeshProUGUI npcNameText;
    public TextMeshProUGUI npcDialogueText;

    public Animator animator;

    [Range(0.05f, 0f)]
    public float textAnimationSpeed;

    string sentenceInProgress;

    public event System.Action OnDialogueEnded;

    // Start is called before the first frame update
    void Start()
    {
        sentences = new Queue<string>();
    }

    public void StartDialogue(Dialogue _dialogue)
    {
        animator.SetBool("IsOpen", true);

        npcNameText.text = _dialogue.npcName;

        sentences.Clear();

        foreach (string sentence in _dialogue.npcSentences)
        {
            sentences.Enqueue(sentence);
        }

        DisplayNextSentence();
    }

    public void DisplayNextSentence()
    {

        if(npcDialogueText.text != sentenceInProgress) // La frase no ha terminado de animarse
        {
            npcDialogueText.text = sentenceInProgress;
            StopAllCoroutines();
            return;
        }

        if (sentences.Count == 0)
        {
            EndDialogue();
            gameStateManager.ChangeState(GameState.Overworld);
            OnDialogueEnded?.Invoke();
            return;
        }

        StopAllCoroutines();
        string sentence = sentences.Dequeue();
        StartCoroutine(TypeSentence(sentence));
    }

    IEnumerator TypeSentence (string sentence)
    {
        sentenceInProgress = sentence;

        npcDialogueText.text = "";
        foreach(char letter in sentence.ToCharArray())
        {
            npcDialogueText.text += letter;
            yield return new WaitForSeconds(textAnimationSpeed);
        }
    }

    void EndDialogue()
    {
        animator.SetBool("IsOpen", false);
    }

}
