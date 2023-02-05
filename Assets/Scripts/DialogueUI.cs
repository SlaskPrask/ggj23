using System;
using System.Collections;
using System.Collections.Generic;
using DialogueSystem;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;

public class DialogueUI : MonoBehaviour
{
    private enum DialoguePhase
    {
        Unavailable,
        MainText,
        ShowOptions,
        Ready,
        GameOver
    }

    public UIComponent optionComponent;
    public UIComponent continueComponent;
    public UIComponent inputComponent;
    public UIComponent timerComponent;
    public DialogueReader reader;
    public float textCharacterDelay = 0.05f;
    public SceneTransition transition;
    public GameOver gameOver;

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
    private AsyncOperation gameOverAction;
    private VisualElement menu;

    void Awake()
    {
        transition.OnTransition(startDialogue);

        textAppearDelay = new WaitForSeconds(textCharacterDelay);
        uiDocument = GetComponent<UIDocument>();

        dialogueBox = uiDocument.rootVisualElement.Query("dialogue");
        dialogueText = uiDocument.rootVisualElement.Query<Label>("dialogue-text-scroll");
        optionsContainer = uiDocument.rootVisualElement.Query("options");

        phase = DialoguePhase.Unavailable;

        VisualElement fullUI = uiDocument.rootVisualElement.Query("dialogue-ui");
        fullUI.RegisterCallback<ClickEvent>(optionsContinue, TrickleDown.TrickleDown);

        VisualElement quit = uiDocument.rootVisualElement.Query("quit-to-menu");
        UIInputs.Button(quit, () => { reader.MainMenu(); });

        SetupSlider("volume-master", () => AudioManager.masterVol, AudioManager.SetMaster);
        SetupSlider("volume-music", () => AudioManager.musicVol, AudioManager.SetMusic);
        SetupSlider("volume-sounds", () => AudioManager.sfxVol, AudioManager.SetSFX);
        SetupSlider("volume-voice", () => AudioManager.voiceVol, AudioManager.SetVoice);

        menu = uiDocument.rootVisualElement.Query("menu");
    }

    private void SetupSlider(string name, Func<float> getter, Action<float> setter)
    {
        Slider slider = uiDocument.rootVisualElement.Query<Slider>(name);
        slider.value = getter() * 100.0f;
        slider.RegisterValueChangedCallback(e => { setter(e.newValue / 100.0f); });
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
            case DialoguePhase.GameOver:
                Debug.Log("game over ok");
                phase = DialoguePhase.Unavailable;
                transition.Transition(() => { gameOverAction.allowSceneActivation = true; });
                break;
            case DialoguePhase.Unavailable:
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

        FocusText();

        StartCoroutine(optionShowing = DialogueDone());
    }

    private void FocusText()
    {
        TextField textField = uiDocument.rootVisualElement.Query<TextField>(null, "unity-text-field");
        if (textField != null)
        {
            textField.Focus();
        }
    }

    private IEnumerator DialogueDone()
    {
        yield return WAIT_HALF;
        phase = DialoguePhase.Ready;
        optionShowing = null;

        FocusText();
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

    private void optionsContinue(ClickEvent e)
    {
        ProgressDialogue();
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
            if (menu.style.display == DisplayStyle.None)
            {
                menu.style.display = DisplayStyle.Flex;
            }
            else
            {
                menu.style.display = DisplayStyle.None;
            }
        }
    }

    public void GameOver(AsyncOperation sceneAsync)
    {
        PrintMain("");
        phase = DialoguePhase.Unavailable;
        gameOver.Activate(() =>
        {
            phase = DialoguePhase.GameOver;
            gameOverAction = sceneAsync;
            Debug.Log("game over ready");
        });
    }

    public void LoadScene(AsyncOperation sceneAsync)
    {
        PrintMain("");
        phase = DialoguePhase.Unavailable;
        transition.Transition(() => { sceneAsync.allowSceneActivation = true; });
    }
}
