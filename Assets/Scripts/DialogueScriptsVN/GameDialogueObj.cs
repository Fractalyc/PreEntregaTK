using UnityEngine;

[CreateAssetMenu(fileName = "GameDialogueObj", menuName = "Scriptable Objects/GameDialogueObj")]
public class GameDialogueObj : ScriptableObject
{
    public string NameOfChar;

    [TextArea(3, 10)]
    public string[] SentencesToRead;
}
