using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class DialogueUI : MonoBehaviour
{
    private enum DialoguePhase
    {
        MainText,
        ShowOptions,
        Ready
    }

    public UIComponent optionComponent;
    public UIComponent continueComponent;
    public float textCharacterDelay = 0.05f;

    private UIDocument uiDocument;
    private Label dialogueFullText;
    private Label dialogueText;
    private VisualElement optionsContainer;
    private VisualElement dialogueBox;
    private IEnumerator scroll;
    private IEnumerator optionShowing;
    private WaitForSeconds textAppearDelay;
    private DialoguePhase phase;
    private static WaitForSeconds WAIT_HALF = new WaitForSeconds(0.5f);

    void Awake()
    {
        textAppearDelay = new WaitForSeconds(textCharacterDelay);
        uiDocument = GetComponent<UIDocument>();

        dialogueBox = uiDocument.rootVisualElement.Query("dialogue");
        dialogueFullText = uiDocument.rootVisualElement.Query<Label>("dialogue-text-full");
        dialogueText = uiDocument.rootVisualElement.Query<Label>("dialogue-text-scroll");
        optionsContainer = uiDocument.rootVisualElement.Query("options");

        phase = DialoguePhase.Ready;

        uiDocument.rootVisualElement.AddManipulator(new Clickable(e => { ProgressDialogue(); }));
    }

    void Start()
    {
    }

    public void PrintMain(string text)
    {
        optionsContainer.Clear();
        dialogueFullText.text = text;
        dialogueText.text = "";

        if (scroll != null)
        {
            StopCoroutine(scroll);
        }

        StartCoroutine(scroll = ScrollText());

        optionsContainer.style.height = 0;
        optionsContainer.RemoveFromClassList("animate-dialogue-height");
        optionsContainer.AddToClassList("animate-dialogue-height");

        phase = DialoguePhase.MainText;
    }

    public void QueueNext()
    {
        optionsContainer.style.height = new StyleLength(StyleKeyword.Auto);

        DialogueOption continueElement = continueComponent.Instantiate<DialogueOption>(optionsContainer, gameObject);
    }

    public void SetOptions(string[] options)
    {
        foreach (string option in options)
        {
            DialogueOption optionElement = optionComponent.Instantiate<DialogueOption>(optionsContainer, gameObject);
            optionElement.SetText(option);
        }
    }

    public void SetTyping()
    {
    }

    private IEnumerator ScrollText()
    {
        while (dialogueText.text.Length < dialogueFullText.text.Length)
        {
            yield return textAppearDelay;
            dialogueText.text += dialogueFullText.text[dialogueText.text.Length];
        }

        scroll = null;
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
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }
    }

    private void FinishScroll()
    {
        phase = DialoguePhase.ShowOptions;

        dialogueText.text = dialogueFullText.text;
        if (scroll != null)
        {
            StopCoroutine(scroll);
            scroll = null;
        }

        optionsContainer.AddToClassList("animate-dialogue-height");

        float optionsHeight = 0;
        foreach (VisualElement option in optionsContainer.Children())
        {
            option.AddToClassList("show-option");
            optionsHeight += option.worldBound.height;
        }

        optionsContainer.style.height = optionsHeight * 2;

        StartCoroutine(optionShowing = DialogueDone());
    }

    private IEnumerator DialogueDone()
    {
        yield return WAIT_HALF;
        phase = DialoguePhase.Ready;
        optionShowing = null;
    }
}
