using System;
using System.Collections;
using System.Collections.Generic;
using DialogueSystem;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UIElements;

public class DialogueUI : MonoBehaviour
{
    private enum DialoguePhase
    {
        Unavailable,
        MainText,
        ShowOptions,
        Ready
    }

    public UIComponent optionComponent;
    public UIComponent continueComponent;
    public UIComponent inputComponent;
    public UIComponent timerComponent;
    public DialogueReader reader;
    public float textCharacterDelay = 0.05f;
    public SceneTransition transition;

    private UIDocument uiDocument;
    private int textPosition;
    private string currentText = "";
    private Label dialogueText;
    private VisualElement optionsContainer;
    private VisualElement dialogueBox;
    private IEnumerator scroll;
    private IEnumerator optionShowing;
    private WaitForSeconds textAppearDelay;
    private DialoguePhase phase;
    private static WaitForSeconds WAIT_HALF = new WaitForSeconds(0.5f);
    private bool noSubmit;
    private DialogueTimer timer;

    void Awake()
    {
        transition.OnTransition(startDialogue);

        textAppearDelay = new WaitForSeconds(textCharacterDelay);
        uiDocument = GetComponent<UIDocument>();

        dialogueBox = uiDocument.rootVisualElement.Query("dialogue");
        dialogueText = uiDocument.rootVisualElement.Query<Label>("dialogue-text-scroll");
        optionsContainer = uiDocument.rootVisualElement.Query("options");

        phase = DialoguePhase.Unavailable;

        uiDocument.rootVisualElement.AddManipulator(new Clickable(e => { ProgressDialogue(); }));
    }

    private void startDialogue()
    {
        phase = DialoguePhase.Ready;
        reader.AdvanceDialogue();
    }

    public void PrintMain(string text)
    {
        RemoveTimer();
        dialogueBox.RemoveFromClassList("full-size");

        noSubmit = false;
        optionsContainer.Clear();
        currentText = text;
        textPosition = 0;
        dialogueText.text = "";

        if (scroll != null)
        {
            StopCoroutine(scroll);
        }

        StartCoroutine(scroll = ScrollText());

        phase = DialoguePhase.MainText;
    }

    public void QueueNext()
    {
        DialogueContinue continueElement = continueComponent.Instantiate<DialogueContinue>(optionsContainer, gameObject);
        noSubmit = true;
        dialogueBox.AddToClassList("full-size");
    }

    public void SetOptions(string[] options)
    {
        StartTimer(DialogueType.PICK);
        for (int i = 0; i < options.Length; i++)
        {
            {
                DialogueOption optionElement = optionComponent.Instantiate<DialogueOption>(optionsContainer, gameObject);
                optionElement.SetUp(this, i, options[i]);
            }
        }
    }

    public void SetTyping()
    {
        StartTimer(DialogueType.TYPE);
        DialogueTextInput textElement = inputComponent.Instantiate<DialogueTextInput>(optionsContainer, gameObject);
        textElement.SetUp(this);
    }

    private IEnumerator ScrollText()
    {
        while (textPosition < currentText.Length)
        {
            dialogueText.text = currentText.Substring(0, textPosition) + "<color=#00000000>" + currentText.Substring(textPosition) + "</color>";
            textPosition++;
            yield return textAppearDelay;
        }

        scroll = null;

        ProgressDialogue();
    }

    private void ProgressDialogue()
    {
        switch (phase)
        {
            case DialoguePhase.MainText:
                FinishScroll();
                break;
            case DialoguePhase.ShowOptions:
                if (optionShowing != null)
                {
                    StopCoroutine(optionShowing);
                    optionShowing = null;
                }

                foreach (VisualElement option in optionsContainer.Children())
                {
                    option.RemoveFromClassList("show-option");
                    option.style.opacity = 1;
                }

                phase = DialoguePhase.Ready;

                break;
            case DialoguePhase.Ready:
                if (noSubmit)
                {
                    ContinueAction();
                }

                break;
            default:
                throw new ArgumentOutOfRangeException();
        }
    }

    private void FinishScroll()
    {
        phase = DialoguePhase.ShowOptions;

        dialogueText.text = currentText;
        if (scroll != null)
        {
            StopCoroutine(scroll);
            scroll = null;
        }

        foreach (VisualElement option in optionsContainer.Children())
        {
            option.AddToClassList("show-option");
        }

        TextField textField = uiDocument.rootVisualElement.Query<TextField>(null, "unity-text-field");
        if (textField != null)
        {
            textField.Focus();
        }

        StartCoroutine(optionShowing = DialogueDone());
    }

    private IEnumerator DialogueDone()
    {
        yield return WAIT_HALF;
        phase = DialoguePhase.Ready;
        optionShowing = null;
    }

    private void RemoveTimer()
    {
        if (timer)
        {
            timer.remove();
            timer = null;
        }
    }

    private void StartTimer(DialogueType type)
    {
        reader.StartQuietTimer(type);
        RemoveTimer();

        timer = timerComponent.Instantiate<DialogueTimer>(dialogueBox, gameObject);
        timer.SetUp(reader.GetTimerNormalized);
    }

    public void SelectOption(int option)
    {
        if (phase != DialoguePhase.Ready)
        {
            return;
        }

        reader.SelectOption(option);
        reader.AdvanceDialogue();
    }

    public void SubmitAnswer(string answer)
    {
        if (phase != DialoguePhase.Ready)
        {
            return;
        }

        reader.SubmitOption(answer);
        reader.AdvanceDialogue();
    }

    public void ContinueAction()
    {
        if (phase != DialoguePhase.Ready)
        {
            return;
        }

        reader.AdvanceDialogue();
    }

    public void OnKeyboardAccept(InputAction.CallbackContext value)
    {
        if (value.performed)
        {
            ProgressDialogue();
        }
    }

    public void OnKeyboardBack(InputAction.CallbackContext value)
    {
        if (value.performed)
        {
        }
    }

    public void GameOver(AsyncOperation sceneAsync)
    {
    }

    public void LoadScene(AsyncOperation sceneAsync)
    {
        PrintMain("");
        transition.Transition(() => { sceneAsync.allowSceneActivation = true; });
    }
}
