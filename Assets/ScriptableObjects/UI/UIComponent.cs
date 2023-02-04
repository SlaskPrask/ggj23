using System;
using System.Collections;
using System.Linq;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;
using Object = UnityEngine.Object;

[CreateAssetMenu(fileName = "UI Component", menuName = "UI/UI Component")]
public class UIComponent : ScriptableObject
{
    public VisualTreeAsset uiDocument;
#if UNITY_EDITOR
    [SerializeField] public MonoScript processorScript;
#endif
    [HideInInspector] [SerializeField] private string typeName;
    private Type _type = null;

    private Type type
    {
        get { return _type != null ? _type : _type = Type.GetType(typeName); }
    }

    public UIBehaviour Instantiate(VisualElement container, GameObject gameObject)
    {
        VisualElement element = uiDocument.Instantiate();
        if (element.childCount == 1)
        {
            element = element.Children().First();
        }

        container.Add(element);
        UIBehaviour component = (UIBehaviour)gameObject.AddComponent(type);
        component.InitUI(element);
        element.RegisterCallback<DetachFromPanelEvent>(e => { Destroy(component); });
        return component;
    }

    public T Instantiate<T>(VisualElement container, GameObject gameObject) where T : UIBehaviour
    {
        return (T)Instantiate(container, gameObject);
    }

#if UNITY_EDITOR
    public void OnValidate()
    {
        if (!typeof(UIBehaviour).IsAssignableFrom(processorScript.GetClass()))
        {
            Debug.LogWarning($"Processor script `{name}` does not implement UIBehaviour base class", this);
        }

        typeName = processorScript.GetClass().FullName;
    }
#endif
}
