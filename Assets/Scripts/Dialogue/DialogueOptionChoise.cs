using System.Collections;
using UnityEngine;

[CreateAssetMenu(menuName = "Dialogue/Option")]
public class DialogueOptionChoise : DialogueOptionBase
{
    [Multiline]
    [SerializeField]
    private string dialogue;

    DialogueOptionChoise()
    {
        dialogueType = DialogueType.OPTION_PICK;
    }
    public string GetOption()
    {
        return dialogue;
    }
}