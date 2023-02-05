using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class DialogueOption : UIBehaviour
{
    private DialogueUI dialogUI;
    [Space] private Label textElement;
    private int id;

    public override void InitUI(VisualElement element)
    {
        textElement = element.Query<Label>("option-text");

        element.AddManipulator(new Clickable(e => { dialogUI.SelectOption(id); }));
    }

    public float GetHeight()
    {
        return textElement.localBound.height;
    }

    public void SetUp(DialogueUI dialogUI, int id, string text)
    {
        this.dialogUI = dialogUI;
        this.id = id;
        this.textElement.text = text;
    }
}
