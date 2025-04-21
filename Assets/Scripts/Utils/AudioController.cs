using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(AudioManagerScript))]
public class AudioController : MonoBehaviour
{
    [HideInInspector] public AudioManagerScript audioManager;
    public static AudioController instance;
    public bool isVibrate;

    private void Awake()
    {
        if (instance == null)
        {

            instance = this;
        }
        else
        {
            Destroy(gameObject);
        }

    }

    public void Start()
    {
        audioManager = GetComponent<AudioManagerScript>();
        isVibrate = true;
    }

    public void PlayCardsDealing()
    {
        audioManager.PlaySound("CardsDealing");
    }
    public void PlayCardPick()
    {
        audioManager.PlaySound("CardPick");
    }


    public bool isSoundOn()
    {

        return audioManager.soundOn;
    }

    public void Click()
    {

        audioManager.PlaySound("Click");

    }
    public void OnShow()
    {
        audioManager.PlaySound("Show");
    }
    public void OnGameWin()
    {
        audioManager.PlaySound("Winner");
    }
    public void OnGameLose()
    {

        audioManager.PlaySound("OnLose");

    }
    public void PlayerTurn()
    {
#if !UNITY_WEBGL
        if(isVibrate)
            Handheld.Vibrate();
#endif

        audioManager.PlaySound("PlayerTurn");

    }
    public void StartTimer()
    {
        
        audioManager.PlaySound("Timer");
    }
    
    public void StopTimer()
    {
        audioManager.Stop("Timer");
    }
    public void OnSortCards()
    {

        audioManager.PlaySound("SortCards");

    }
    public void OnSpinWheel()
    {

        audioManager.PlaySound("WheelSpin");

    }
    public void OnFortuneWin()
    {

        audioManager.PlaySound("FortuneWin");

    }

    public void playClip(AudioClip clip)
    {
        audioManager.playclip(clip);
    }

    public AudioSource getEmptySource()
    {
        return audioManager.emptyAudioSource;
    }

    public void stopClip()
    {
        audioManager.stopClip();
    }

    public void AddAudioSource()
    {
        audioManager.addAudioSource();
    }

    public void PlayPanelSlide() {

        audioManager.PlaySound("PanelSlide");
    }
    public void PlayPopUpAlert()
    {

        audioManager.PlaySound("PopUpAlert");
    }
    public void SoundOff()
    {

        SoundScript[] sounds = audioManager.Sounds;
        audioManager.soundOn = false;

        foreach (SoundScript s in sounds)
        {
            if (s.source.isPlaying)
                s.source.Stop();
        }
    }

    public void SoundOn()
    {
        audioManager.soundOn = true;

    }
    public void StopSounds()
    {

        audioManager.StopAllSounds();
    }
    public void OnClick() {

        audioManager.PlaySound("OnClick");

    }
    public void OnScroll()
    {

        audioManager.PlaySound("OnScroll");

    }
    public void Vibratehandheld() {
#if !UNITY_WEBGL
        if (isVibrate)
            Handheld.Vibrate();
#endif
    }


    
}
