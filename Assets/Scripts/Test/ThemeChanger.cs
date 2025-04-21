using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class ThemeChanger : MonoBehaviour
{
    public ScalingEffect[] ScalingEffect;
    public Image TableImage,CurrentTheme,SelectTheme;
    public Button SelectThemeButton;
    public Theme[] Themes;
    private int Selectindex;
    private WaitForSeconds startDelay;
    private WaitForSeconds disableDelay;
    private void Start()
    {
        SelectThemeButton.onClick.AddListener(OnSelectTheme);  
    }
    private void Awake()
    {
        startDelay = new WaitForSeconds(0.15f);
        disableDelay = new WaitForSeconds(0.6f);
    }
    private void OnEnable()
    {
        if (TableImage.sprite.name.Equals(Themes[0].ThemeTableBg.name)) {

            CurrentTheme.sprite = Themes[0].Themeicon;
            SelectTheme.sprite = Themes[1].Themeicon;
            Selectindex = 1;
        }
        else
        {
            CurrentTheme.sprite = Themes[1].Themeicon;
            SelectTheme.sprite = Themes[0].Themeicon;
            Selectindex = 0;
        }
        StartCoroutine(ThemeChnagePanelViewEntryEffect());
    }
   
    
    void OnSelectTheme() {

        TableImage.sprite = Themes[Selectindex].ThemeTableBg;
        DisableThemeChnagePanelPanelView();


    }
    private IEnumerator ThemeChnagePanelViewEntryEffect()
    {
        yield return startDelay;
        PlayStartEffects();
    }

    private IEnumerator ThemeChnagePanelViewExitEffect()
    {
        PlayEndEffect();
        yield return disableDelay;
        gameObject.SetActive(false);
    }




    public void DisableThemeChnagePanelPanelView()
    {
        StartCoroutine(ThemeChnagePanelViewExitEffect());
    }


    private void PlayStartEffects()
    {
        for (int i = 0; i < ScalingEffect.Length; i++)
        {
            StartCoroutine(ScalingEffect[i].EntryEffect());
        }
    }

    private void PlayEndEffect()
    {
        for (int i = 0; i < ScalingEffect.Length; i++)
        {
            StartCoroutine(ScalingEffect[i].ExitEffect());
        }
    }
}
