using System;
using UnityEngine;
using UnityEngine.UIElements;

public static class UIInputs
{
    public static void Button(VisualElement element, Action action)
    {
        element.AddManipulator(new Clickable(e =>
        {
            AudioManager.PlayAudio(AudioManager.SoundClip.CLICK);
            action();
        }));
        element.RegisterCallback<MouseEnterEvent>(e => { AudioManager.PlayAudio(AudioManager.SoundClip.HOVER); });
    }
}
