using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public abstract class UIBehaviour : MonoBehaviour
{
    public UIDocument uiDocument;
    public string existingElement;

    public abstract void InitUI(VisualElement element);

    public virtual void Awake()
    {
        if (existingElement != null)
        {
            uiDocument = GetComponentInParent<UIDocument>();
            InitUI(uiDocument.rootVisualElement.Query(existingElement));
        }
    }
}
