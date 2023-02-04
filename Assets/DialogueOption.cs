using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class DialogueOption : UIBehaviour
{
    [Space] private Label textElement;

    public override void InitUI(VisualElement element)
    {
        textElement = element.Query<Label>("option-text");

        element.AddManipulator(new Clickable(e => { Debug.Log("i have to change my git config"); }));
    }

    public void SetText(string text)
    {
        textElement.text = text;
    }

    public float GetHeight()
    {
        return textElement.localBound.height;
    }
}
