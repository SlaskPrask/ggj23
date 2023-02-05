using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class DialogueTimer : UIBehaviour
{
    private VisualElement bar;
    private VisualElement cursor;
    private WaitForSeconds wait = new WaitForSeconds(0.25f);
    private Func<float> getTime;
    private VisualElement root;

    public override void InitUI(VisualElement element)
    {
        root = element;
        bar = element.Query("timer-progressbar");
        cursor = element.Query("timer-cursor");

        StartCoroutine(Grow());
    }

    public void SetUp(Func<float> getTime)
    {
        this.getTime = getTime;
    }

    private IEnumerator Grow()
    {
        cursor.style.paddingLeft = 7;
        cursor.style.paddingRight = 7;
        cursor.style.paddingTop = 2;
        cursor.style.paddingBottom = 2;
        yield return wait;
        StartCoroutine(Shrink());
    }

    private IEnumerator Shrink()
    {
        cursor.style.paddingLeft = 0;
        cursor.style.paddingRight = 0;
        cursor.style.paddingTop = 0;
        cursor.style.paddingBottom = 0;
        yield return wait;
        StartCoroutine(Grow());
    }

    public void Update()
    {
        bar.style.width = Length.Percent(100.0f * getTime());
    }

    public void remove()
    {
        root.parent.Remove(root);
    }
}
