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

    public static float masterVol { get; private set; }
    public static float sfxVol { get; private set; }
    public static float musicVol { get; private set; } 
    public static float voiceVol { get; private set; }  

    private StudioEventEmitter musicEmitter;
    private EventInstance musicInstance;

    public EventReference[] soundEFX;
    public EventReference[] music;
    public EventReference voice;

    private MusicID currentID;

    public void Initialize()
    {
        masterBus = RuntimeManager.GetBus("bus:/Master");
        SFXBus = RuntimeManager.GetBus("bus:/Master/SoundFX");
        musicBus = RuntimeManager.GetBus("bus:/Master/Music");
        voiceBus = RuntimeManager.GetBus("bus:/Master/Voice");

        masterVol = (PlayerPrefs.GetFloat("master-bus", .5f));
        sfxVol = (PlayerPrefs.GetFloat("sfx-bus", .5f));
        musicVol = (PlayerPrefs.GetFloat("music-bus", .5f));
        voiceVol = (PlayerPrefs.GetFloat("voice-bus", .5f));
        SetMaster(masterVol);
        SetSFX(sfxVol);
        SetMusic(musicVol);
        SetVoice(voiceVol);
    
        musicEmitter = transform.GetChild(0).GetComponent<StudioEventEmitter>();

        currentID = MusicID.NULL;
        PlayMusic(MusicID.DRIPPING);
    }

    public static void SaveSettings()
    {
        PlayerPrefs.SetFloat("master-bus", masterVol);
        PlayerPrefs.SetFloat("sfx-bus", sfxVol);
        PlayerPrefs.SetFloat("music-bus", musicVol);
        PlayerPrefs.SetFloat("voice-bus", voiceVol);
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
        WRONG = 3
    }

    public static void PlayAudio(SoundClip clip)
    {
        RuntimeManager.PlayOneShot(GameManager.audio.soundEFX[(byte)clip]);
    }

    public enum MusicID : sbyte
    {
        NULL = -1,
        CHAPTER_1 = 0,
        GAME_OVER = 1,
        DRIPPING = 2
    }

    public static void PlayMusic(MusicID id)
    {
        if (id == GameManager.audio.currentID)
            return;

        if (GameManager.audio.musicInstance.isValid())
        {
            GameManager.audio.musicInstance.stop(id == MusicID.GAME_OVER ? FMOD.Studio.STOP_MODE.IMMEDIATE : FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
            GameManager.audio.musicInstance.release();
        }

        if (id == MusicID.NULL)
        {
            return;
        }

        GameManager.audio.musicInstance = RuntimeManager.CreateInstance(GameManager.audio.music[(sbyte)id]);
        GameManager.audio.musicInstance.start();
    }

    public enum Mood : byte
    {
        NEUTRAL = 0,
        BAD = 1,
        GOOD = 2
    }

    public static void PlayVoice(Mood mood, byte character)
    {
        EventInstance instance = RuntimeManager.CreateInstance(GameManager.audio.voice);

        FMOD.RESULT res = instance.setParameterByName("Mood", (byte)mood); ;
        if (res != FMOD.RESULT.OK)
        {
            Debug.Log("Something wrong mood");
        }
        res = instance.setParameterByName("Character", character / 2f);
        if (res != FMOD.RESULT.OK)
        {
            Debug.Log("Something wrong char");
        }

        instance.start();
        instance.release();
    }
}
