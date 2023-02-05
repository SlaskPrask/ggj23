using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DialogueSystem;
using UnityEngine.SceneManagement;

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
    private int currentScene;


    private MainDialogue tempDialogue;
    private SpecialEvent gameOverDialogue;
    Coroutine routine;

#if DEBUG
    public bool testing = false;
#endif


    private void Awake()
    {
        if (startDialogue == null)
        {
            Debug.LogError("No dialogue set in: " + name);
            return;
        }

        tempDialogue = MainDialogue.CreateInstance<MainDialogue>();
        gameOverDialogue = SpecialEvent.CreateInstance<SpecialEvent>();
        queuedDialogue = startDialogue;
        currentScene = SceneManager.GetActiveScene().buildIndex;
        quietTimer = quietTime;
    }

    private void Start()
    {
#if DEBUG
        if (testing)
        {
            AdvanceDialogue();
        }
#endif
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
                NextScene();
                break;
            default:
                break;
        }
    }

    public void AdvanceDialogue()
    {
        if (queuedDialogue == null)
        {
            return;
        }

        if (queuedDialogue.dialogueType == DialogueType.NULL)
        {
            Debug.LogError("Dialogue queue is null: " + name);
            return;
        }

        queuedDialogue.Invoke(this);
    }

    public void SelectOption(int optionIndex)
    {
        if (routine != null)
            StopCoroutine(routine);

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
        if (routine != null)
            StopCoroutine(routine);

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
        if (routine != null)
            StopCoroutine(routine);

        routine = StartCoroutine(StartQuietTimerIEnumerator(dialogueType));
    }

    private void ActivateTyping()
    {
        dialogueUI.SetTyping();
    }

    private void GameOver()
    {
        queuedDialogue = null;
        AsyncOperation sceneLoad = LoadScene(currentScene);
        AudioManager.PlayMusic(AudioManager.MusicID.GAME_OVER);
        dialogueUI.GameOver(sceneLoad);
    }

    private void NextScene()
    {
        queuedDialogue = null;
        AsyncOperation sceneLoad = LoadScene(currentScene + 1);
        dialogueUI.LoadScene(sceneLoad);
    }

    private AsyncOperation LoadScene(int index)
    {
        AsyncOperation loadScene = SceneManager.LoadSceneAsync(index);
        loadScene.allowSceneActivation = false;
        return loadScene;
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
