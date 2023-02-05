using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class GameOver : MonoBehaviour
{
    private UIDocument uiDocument;
    private VisualElement element;
    private VisualElement red;
    private Action callback;
    private Label text;
    private Label continueText;

    private void Awake()
    {
        uiDocument = GetComponent<UIDocument>();
        element = uiDocument.rootVisualElement.Query("fall");
        text = uiDocument.rootVisualElement.Query<Label>("gameover");
        red = uiDocument.rootVisualElement.Query("red");
        continueText = uiDocument.rootVisualElement.Query<Label>("continue");
    }

    public void Activate(Action callback)
    {
        this.callback = callback;
        StartCoroutine(Run());
    }

    private IEnumerator Run()
    {
        yield return new WaitForSeconds(0.25f);

        float top = 100;
        while (top > -50)
        {
            yield return null;
            top -= Time.deltaTime * 400.0f;
            element.style.top = new StyleLength(new Length(top, LengthUnit.Percent));
        }

        top = -50;
        element.style.top = new StyleLength(new Length(top, LengthUnit.Percent));

        text.style.opacity = 1;

        yield return new WaitForSeconds(3.25f);

        Stab();

        yield return new WaitForSeconds(1f);

        continueText.style.opacity = 1;

        yield return new WaitForSeconds(1f);

        callback();
    }

    private void Stab()
    {
        red.style.visibility = Visibility.Visible;
        red.style.opacity = 0.1f;
    }
}
