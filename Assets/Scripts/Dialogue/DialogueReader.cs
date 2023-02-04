using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DialogueSystem;


public class DialogueReader : MonoBehaviour
{
    private const float quietTime = 120.0f;
    public float quietTimer;

    [SerializeField]
    private DialogueBase startDialogue;
    private DialogueBase queuedDialogue;
    [SerializeField]
    private DialogueUI dialogueUI;
    private int responseAttempt = 0;
    private string hint;

    private void Awake()
    {
        if (startDialogue == null)
        {
            Debug.LogError("No dialogue set in: " + name);
            return;
        }

        queuedDialogue = startDialogue;
    }

    private void Start()
    {
        AdvanceDialogue();
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
                                   leadToType == DialogueType.TYPE) ||
                                   leadToType == DialogueType.PICK)
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
                SetOptions(leadsTo[0]);
                break;
            case DialogueType.TYPE:
                ActivateTyping();
                break;
            default:
                Debug.LogError("If this is shown, something has gone horribly wrong: " + name);
                break;
        }
    }

    /// <summary>
    /// This is called by SpecialEvent
    /// </summary>
    public void DoEvent(SpecialEvent.GameEvent eventType)
    {
        Debug.Log("TODO: Do Event: " + eventType);
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
        if (optionIndex == -1)
        {
            PrintDialogueText(AnswerResponses.noAnswer);
            return;
        }

        PickOption option = (PickOption)queuedDialogue.GetLeadsTo()[0];
        DialogueBase[] leadsTo = option.GetLeadsTo();
        if (leadsTo == null || leadsTo.Length < 1)
        {
            Debug.LogError("Option has no further dialogue.");
            return;
        }

        if (option.CheckCorrectAnswer(optionIndex))
        {
            queuedDialogue = leadsTo[0];
            PrintDialogueText(AnswerResponses.correctResponses[Random.Range(0, AnswerResponses.correctResponses.Length)]);
        }
        else
        {
            //TODO Game Over
            //queuedDialogue = options[0];
            PrintDialogueText(AnswerResponses.wrongResponses[Random.Range(0, AnswerResponses.wrongResponses.Length)]);
        }
    }

    public void SubmitOption(string option)
    {
        string response = "";
        TypeAnswer leadsTo = (TypeAnswer)queuedDialogue.GetLeadsTo()[0];
        DialogueBase return_val = leadsTo.ValidateAnswer(option, ref response);

        if (return_val != null)
        {
            responseAttempt = 0;
            hint = "";
            queuedDialogue = return_val;
            return;
        }

        responseAttempt++;

        // Give Hint
        if (responseAttempt >= 3)
        {
            hint = " It starts with a " + leadsTo.GetFirstLetterOfAnswer() + ".";
        }

        // Game Over
        if (responseAttempt >= 6)
        {
            Debug.Log("TODO: You died");
            return;
        }

        PrintDialogueText(response);
    }

    private void PrintDialogueText(string dialogue)
    {
        if (string.IsNullOrWhiteSpace(hint))
        {
            dialogueUI.PrintMain(dialogue);
        }
        else
        {
            dialogueUI.PrintMain(dialogue + hint);
        }
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

    public void SetOptions(DialogueBase options)
    {
        dialogueUI.SetOptions(((PickOption)options).GetShuffeledOption());
    }

    public void StartQuietTimer(DialogueType dialogueType)
    {
        StartCoroutine(StartQuietTimerIEnumerator(dialogueType));
    }

    private void ActivateTyping()
    {
        dialogueUI.SetTyping();
    }

    private IEnumerator StartQuietTimerIEnumerator(DialogueType dialogueType)
    {
        quietTimer = quietTime;

        while (quietTimer > 0f)
        {
            yield return null;
            quietTimer -= Time.deltaTime;
        }

        quietTimer = quietTime;

        if (dialogueType == DialogueType.TYPE)
        {
            SubmitOption("");
        }
        else
        {
            SelectOption(-1);
        }
    }

    public float GetTimerNormalized()
    {
        return quietTimer / quietTime;
    }
}
