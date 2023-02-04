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

    private MainDialogue tempDialogue;

    private void Awake()
    {
        if (startDialogue == null)
        {
            Debug.LogError("No dialogue set in: " + name);
            return;
        }

        tempDialogue = MainDialogue.CreateInstance<MainDialogue>();
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
        DialogueBase leadsTo = dialogue.GetLeadsTo();

        if (leadsTo == null)
        {
            Debug.LogError("Dialogue doesn't lead to anything: " + name);
            return;
        }

        DialogueType leadToType = leadsTo.dialogueType;

        if (leadToType == DialogueType.NULL)
        {
            Debug.LogError("Dialogue type is NULL for option in: " + name);
            return;
        }

        switch (leadToType)
        {
            case DialogueType.MAIN:
            case DialogueType.EVENT:
                QueueNext(leadsTo);
                break;
            case DialogueType.PICK:
                SetOptions(leadsTo);
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
        switch (eventType)
        {
            case SpecialEvent.GameEvent.GAME_OVER:
                GameOver();
                break;
            case SpecialEvent.GameEvent.LOAD_SCENE:
                LoadScene();
                break;
            default:
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
        if (optionIndex == -1)
        {

            SetTempAsQueue(AnswerResponses.noAnswer);
            return;
        }

        PickOption option = (PickOption)queuedDialogue.GetLeadsTo();
        DialogueBase leadsTo = option.GetLeadsTo();
        if (leadsTo == null)
        {
            Debug.LogError("Option has no further dialogue.");
            return;
        }

        if (option.CheckCorrectAnswer(optionIndex))
        {
            queuedDialogue = leadsTo;
            SetTempAsQueue(AnswerResponses.correctResponses[Random.Range(0, AnswerResponses.correctResponses.Length)]);
        }
        else
        {
            GameOver();
        }
    }

    public void SubmitOption(string option)
    {
        string response = "";
        TypeAnswer leadsTo = (TypeAnswer)queuedDialogue.GetLeadsTo();
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
            GameOver();
            return;
        }

        SetTempAsQueue(response);
    }

    private void SetTempAsQueue(string dialogue)
    {
        tempDialogue.SetText(dialogue);
        tempDialogue.SetLeadsTo(queuedDialogue);
        queuedDialogue = tempDialogue;
    }

    private void PrintDialogueText(MainDialogue dialogue)
    {
        if (queuedDialogue == tempDialogue || string.IsNullOrWhiteSpace(hint))
        {
            dialogueUI.PrintMain(dialogue.GetParsedDialogue());
        }
        else
        {
            dialogueUI.PrintMain(dialogue.GetParsedDialogue() + hint);
        }
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

    private void GameOver()
    {
        dialogueUI.GameOver();
    }

    private void LoadScene()
    {
        dialogueUI.LoadScene();
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
