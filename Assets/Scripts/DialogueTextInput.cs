using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class DialogueTextInput : UIBehaviour
{
    private DialogueUI dialogueUI;
    private TextField textField;

    public override void InitUI(VisualElement element)
    {
        VisualElement button = element.Query("dialogue-submit");
        UIInputs.Button(button, () => { Submit(); });

        textField = element.Query<TextField>("dialogue-text-field");
        textField.RegisterCallback<KeyDownEvent>(e =>
        {
            if (e.keyCode == KeyCode.Return)
            {
                Submit();
            }
        });
    }

    public void SetUp(DialogueUI dialogueUI)
    {
        this.dialogueUI = dialogueUI;
    }

    private void Submit()
    {
        dialogueUI.SubmitAnswer(textField.value);
    }
}
