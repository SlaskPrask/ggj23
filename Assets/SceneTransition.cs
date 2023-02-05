using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class SceneTransition : MonoBehaviour
{
    private Action doneCallback;
    private UIDocument uiDocument;
    private VisualElement screen;

    public void Awake()
    {
        uiDocument = GetComponent<UIDocument>();
        screen = uiDocument.rootVisualElement.Query("screen");
    }

    public void Start()
    {
        StartCoroutine(doTransition());
    }

    private IEnumerator doTransition()
    {
        yield return null;
        screen.style.opacity = 0;
        yield return new WaitForSeconds(3.5f);
        if (doneCallback != null)
        {
            doneCallback();
        }
    }

    public void OnTransition(Action callback)
    {
        doneCallback = callback;
    }

    public void Transition(Action callback)
    {
        screen.style.opacity = 1;
        doneCallback = callback;
        StartCoroutine(doTransition());
    }
}
