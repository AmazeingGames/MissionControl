using FMOD.Studio;
using FMODUnity;
using System.Collections.Generic;
using System;
using UnityEngine;


// The AudioManager is solely responsibly for playing every sound in the game.
public class AudioManager : MonoBehaviour
{
    [SerializeField] AudioHelper audioHelper;
    public static AudioManager inst;

    void Awake()
    {
        if (inst == null)
            inst = this;
        else
            Destroy(gameObject);
    }

    private void OnEnable()
    {
        
    }

    private void OnDisable()
    {

    }



    public void PlayOneShot(EventReference audioClip)
    {
        RuntimeManager.PlayOneShot(audioClip);
    }
}

