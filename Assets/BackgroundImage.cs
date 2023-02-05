using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class BackgroundImage : MonoBehaviour
{
    public Sprite image;
    private UIDocument uiDocument;
    private VisualElement background;

    public void Awake()
    {
        uiDocument = GetComponent<UIDocument>();
        background = uiDocument.rootVisualElement.Query("background");
        background.style.backgroundImage = new StyleBackground(image);
    }

    public void SetBackground(Sprite sprite)
    {
        image = sprite;
        background.style.backgroundImage = new StyleBackground(image);
    }
}
