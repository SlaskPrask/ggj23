using FMODUnity;
using FMOD.Studio;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    private Bus masterBus;
    private Bus SFXBus;
    private Bus musicBus;
    private Bus voiceBus;

    public void Initialize()
    {
        masterBus = RuntimeManager.GetBus("bus:/");
        SFXBus = RuntimeManager.GetBus("bus:/SoundEFX");
        musicBus = RuntimeManager.GetBus("bus:/Music");
        voiceBus = RuntimeManager.GetBus("bus:/Voices");
    }

    public static void SetMaster(float value)
    {
        
    }

    public static void SetSFX(float value)
    {

    }

    public static void SetMusic(float value)
    {

    }

    public static void SetVoice(float value)
    {

    }

}
