
using UnityEngine;
using UnityEngine.Audio;
[System.Serializable]
public class SoundScript
{
    public string name;
    public AudioClip Clip;
    
    [Range(0,1)]
    public float Volume;
    [Range(0, 1)]
    public float Pitch;
    public bool Looping;
    [HideInInspector]
    public AudioSource source;
}
