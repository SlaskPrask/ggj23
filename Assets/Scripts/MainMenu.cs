using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;

public class MainMenu : MonoBehaviour
{
    private UIDocument uiDocument;

    public void Awake()
    {
        uiDocument = GetComponent<UIDocument>();
        VisualElement start = uiDocument.rootVisualElement.Query("start-button");
        UIInputs.Button(start, () => { SceneManager.LoadScene(1); });

        VisualElement quit = uiDocument.rootVisualElement.Query("quit-button");
        UIInputs.Button(quit, () => { Application.Quit(0); });

        SetupSlider("volume-master", () => AudioManager.masterVol, AudioManager.SetMaster);
        SetupSlider("volume-music", () => AudioManager.musicVol, AudioManager.SetMusic);
        SetupSlider("volume-sounds", () => AudioManager.sfxVol, AudioManager.SetSFX);
        SetupSlider("volume-voice", () => AudioManager.voiceVol, AudioManager.SetVoice);
    }

    private void SetupSlider(string name, Func<float> getter, Action<float> setter)
    {
        Slider slider = uiDocument.rootVisualElement.Query<Slider>(name);
        slider.value = getter() * 100.0f;
        slider.RegisterValueChangedCallback(e => { setter(e.newValue / 100.0f); });
    }
}
