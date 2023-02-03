using System.Collections;
using UnityEngine;

[CreateAssetMenu(menuName = "Dialogue/Type")]
public class DialogueOptionTyped : DialogueOptionBase
{
    DialogueOptionTyped()
    {
        dialogueType = DialogueType.OPTION_WRITE;
    }
}
