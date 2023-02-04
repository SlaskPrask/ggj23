using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class DialogueUI : MonoBehaviour
{
    public UIComponent optionComponent;

    private UIDocument uiDocument;
    private Label dialogueText;
    private VisualElement optionsContainer;


    void Awake()
    {
        uiDocument = GetComponent<UIDocument>();

        dialogueText = uiDocument.rootVisualElement.Query<Label>("dialogue-text");
        optionsContainer = uiDocument.rootVisualElement.Query("options");
    }

    void Start()
    {
        StartDialogue("i'm slask", "but i'm slask", "i'm also slask");
    }

    public void StartDialogue(string text, params string[] options)
    {
        optionsContainer.Clear();

        dialogueText.text = text;
        foreach (string option in options)
        {
            DialogueOption optionElement = optionComponent.Instantiate<DialogueOption>(optionsContainer, gameObject);
            optionElement.SetText(option);
        }
    }
}
