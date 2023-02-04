using System.Collections;
using UnityEngine;

public enum DialogueType
{
    NULL = 0,
    MAIN = 1,
    EVENT = 2,
    OPTION_PICK = 3,
    OPTION_WRITE = 4
}
public abstract class DialogueBase : ScriptableObject
{
    public DialogueType dialogueType { get; protected set; }
}