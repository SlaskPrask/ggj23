using FMODUnity;
using FMOD.Studio;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Timeline;

public class AudioManager : MonoBehaviour
{
    private Bus masterBus;
    private Bus SFXBus;
    private Bus musicBus;
    private Bus voiceBus;

    private float masterVol;
    private float sfxVol;
    private float musicVol;
    private float voiceVol;

    public EventReference[] soundEFX;

    public void Initialize()
    {
        masterBus = RuntimeManager.GetBus("bus:/");
        SFXBus = RuntimeManager.GetBus("bus:/SoundEFX");
        musicBus = RuntimeManager.GetBus("bus:/Music");
        voiceBus = RuntimeManager.GetBus("bus:/Voices");

        masterVol = (PlayerPrefs.GetFloat("master-bus", .5f));
        sfxVol = (PlayerPrefs.GetFloat("sfx-bus", .5f));
        musicVol = (PlayerPrefs.GetFloat("music-bus", .5f));
        voiceVol = (PlayerPrefs.GetFloat("voice-bus", .5f));
        SetMaster(masterVol);
        SetSFX(sfxVol);
        SetMusic(musicVol);
        SetVoice(voiceVol);
    }

    public static void SaveSettings()
    {
        PlayerPrefs.SetFloat("master-bus", GameManager.audio.masterVol);
        PlayerPrefs.SetFloat("sfx-bus", GameManager.audio.sfxVol);
        PlayerPrefs.SetFloat("music-bus", GameManager.audio.musicVol);
        PlayerPrefs.SetFloat("voice-bus", GameManager.audio.voiceVol);
        PlayerPrefs.Save();

    }

    public static void SetMaster(float value)
    {
        GameManager.audio.masterBus.setVolume(value);
    }

    public static void SetSFX(float value)
    {
        GameManager.audio.SFXBus.setVolume(value);
    }

    public static void SetMusic(float value)
    {
        GameManager.audio.musicBus.setVolume(value);
    }

    public static void SetVoice(float value)
    {
        GameManager.audio.voiceBus.setVolume(value);
    }

    public enum SoundClip : byte
    {
        HOVER = 0,
        CLICK = 1,
        CORRECT = 2,
        WRONG = 3,
        GAME_OVER = 4
    }

    public static void PlayAudio(SoundClip clip)
    {
        RuntimeManager.PlayOneShot(GameManager.audio.soundEFX[(byte)clip]);
    }

}
