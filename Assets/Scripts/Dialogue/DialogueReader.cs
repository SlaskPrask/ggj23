using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DialogueSystem;


public class DialogueReader : MonoBehaviour
{
    [SerializeField]
    private DialogueBase startDialogue;
    private DialogueBase queuedDialogue;

    private void Awake()
    {
        if (startDialogue == null)
        {
            Debug.LogError("No dialogue set in: " + name);
            return;
        }
        queuedDialogue = startDialogue;
    }

    /// <summary>
    /// This is called by the main dialogue
    /// </summary>
    public void PrintDialogue(MainDialogue dialogue)
    {
        PrintDialogueText(dialogue);
        DialogueBase[] leadsTo = dialogue.GetLeadsTo();

        if (leadsTo == null || leadsTo.Length < 1)
        {
            Debug.LogError("Dialogue doesn't lead to anything: " + name);
            return;
        }

        DialogueType leadToType = leadsTo[0].dialogueType;

        if (leadToType == DialogueType.NULL)
        {
            Debug.LogError("Dialogue type is NULL for option in: " + name);
            return;
        }

        if (leadsTo.Length > 1 && (leadToType == DialogueType.MAIN || 
            leadToType == DialogueType.EVENT ||
            leadToType == DialogueType.TYPE))
        {
            Debug.LogError("Dialogue type is NULL for option in: " + name);
            return;
        }

        for (int i = 1; i < leadsTo.Length; i++)
        {
            if (leadsTo[i].dialogueType != leadToType)
            {
                Debug.LogError("Lead to type is different in options for: " + name + "\nThis is not allowed >:(");
                return;
            }
        }

        switch (leadToType)
        {
            case DialogueType.MAIN:
            case DialogueType.EVENT:
                QueueNext(leadsTo[0]);
                break;
            case DialogueType.PICK:
                SetOptions((PickOption[])leadsTo);
                break;
            case DialogueType.TYPE:
                ActivateTyping();
                break;
            default:
                Debug.LogError("If this is shown, something has gone horribly wrong: " + name);
                break;
        }
    }

    public void AdvanceDialogue()
    {
        if (queuedDialogue == null || queuedDialogue.dialogueType == DialogueType.NULL)
        {
            Debug.LogError("Dialogue queue is null: " + name);
            return;
        }

        queuedDialogue.Invoke(this);
    }

    public void SelectOption(int optionIndex)
    {
        DialogueBase option = queuedDialogue.GetLeadsTo()[optionIndex];
        DialogueBase[] options = option.GetLeadsTo();

        if (options == null || options.Length < 1)
        {
            Debug.LogError("Option has no further dialogue.");
            return;
        }
        queuedDialogue = options[0];
    }

    public void SubmitOption(string option)
    {

    }

    private void PrintDialogueText(MainDialogue dialogue)
    {
        dialogueUI.PrintMain(dialogue.GetParsedDialogue());
    }

    private void QueueNext(DialogueBase next)
    {
        queuedDialogue = next;
        dialogueUI.QueueNext();
    }

    public void SetOptions(PickOption[] options)
    {
        string[] parsedOptions = new string[options.Length];
        for (int i = 0; i < options.Length; i++)
        {
            parsedOptions[i] = options[i].GetParsedOption();
        }
        dialogueUI.SetOptions(parsedOptions);
    }

    private void ActivateTyping()
    {
        dialogueUI.SetTyping();
    }
}
