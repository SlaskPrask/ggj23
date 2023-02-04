using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    private static GameManager inst;
    public static GameManager instance
    {
        get
        {
            return inst;
        }
    }

    public static new AudioManager audio { get; private set; }

    private void Awake()
    {
        if (inst == null)
        {
            inst = this;
        }
        else if (inst != this)
        {
            Destroy(gameObject);
            return;
        }

        audio = GetComponentInChildren<AudioManager>();
    }
}
