using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using System;
using UnityEngine.UI;

public class AudioManagerScript : MonoBehaviour
{
    public SoundScript[] Sounds;
    [HideInInspector] public bool soundOn = true;
    [HideInInspector] public AudioSource emptyAudioSource;
    [SerializeField] private AudioSource bgMusicSource;
    [SerializeField] private Button bgmToggleIcon;

    private void Awake()
    {
        foreach (SoundScript CurrentSound in Sounds)
        {
            CurrentSound.source = gameObject.AddComponent<AudioSource>();
            CurrentSound.source.clip = CurrentSound.Clip;
            CurrentSound.source.volume = CurrentSound.Volume;
            CurrentSound.source.pitch = CurrentSound.Pitch;
            CurrentSound.source.loop = CurrentSound.Looping;
        }
        CheckBgm();
    }


    public void ToggleBgm(bool _state)
    {
        bgMusicSource.mute = _state;
        bgmToggleIcon.interactable = !_state;
    }

    public void ToggleBgm()
    {
        if (PlayerPrefs.GetInt("BGMusic", 0) == 1)
        {
            PlayerPrefs.SetInt("BGMusic", 0);
        }
        else if (PlayerPrefs.GetInt("BGMusic", 0) == 0)
        {
            PlayerPrefs.SetInt("BGMusic", 1);
        }
        CheckBgm();
    }
        

    public void CheckBgm()
    {
        if(!PlayerPrefs.HasKey("BGMusic")) PlayerPrefs.SetInt("BGMusic", 0);


        if (PlayerPrefs.GetInt("BGMusic",0) == 0)
        {
            ToggleBgm(false);
            
        }
        else if(PlayerPrefs.GetInt("BGMusic", 0) == 1)
        {
            ToggleBgm(true);
        }
    }


    public void changeVol(string sound, float vol)
    {
        if (soundOn)
        {
            SoundScript s = Array.Find(Sounds, SoundScript => SoundScript.name == sound); ;
            if (s != null)
            {
                s.Volume = vol;
                s.source.volume = vol;
            }
        }
    }
    public void PlaySound(string SoundName)
    {
        //StopAllSounds();
        SoundScript PlayS = Array.Find(Sounds, SoundScript => SoundScript.name == SoundName);
        if (PlayS != null && soundOn)
        {
            if (!PlayS.source.isPlaying) {

                PlayS.source.Play();
            }
        }
    }

    public void Stop(string sound)
    {
        if (soundOn)
        {
            SoundScript s = Array.Find(Sounds, SoundScript => SoundScript.name == sound);

            if (s != null && s.source.isPlaying)
            {
                s.source.Stop();
            }
        }
    }
    public void StopAllSounds()
    {
        if (soundOn)
        {
            foreach (SoundScript sound in Sounds)
            {
                if(sound.source == null) return;
                if (sound.source.isPlaying)
                {
                    sound.source.Stop();
                }
            }
        }
    }

    public void playclip(AudioClip clip)
    {

        if (soundOn)
        {
            addAudioSource();

            if (!emptyAudioSource.isPlaying)
            {
                emptyAudioSource.PlayOneShot(clip);
            }


        }


    }

    public void stopClip()
    {
        if (emptyAudioSource.isPlaying && emptyAudioSource != null)
            emptyAudioSource.Stop();
    }



    public void addAudioSource()
    {
        if (emptyAudioSource == null)
        {
            emptyAudioSource = gameObject.AddComponent<AudioSource>();
            emptyAudioSource.volume = 1.5f;
        }
    }
}
