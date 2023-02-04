using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UIElements;

public class DialogueContinue : UIBehaviour
{
    private static readonly WaitForSeconds WAIT = new WaitForSeconds(0.5f);
    private Label text;
    private int phase;

    public override void InitUI(VisualElement element)
    {
        phase = 0;
        text = element.Query<Label>("continue-text");

        StartCoroutine(Animate());
    }

    private IEnumerator Animate()
    {
        for (;;)
        {
            yield return WAIT;
            phase = phase < 2 ? phase + 1 : 0;
            text.text = "Continue" + string.Concat(Enumerable.Repeat(" .", phase + 1));
        }
    }
}
