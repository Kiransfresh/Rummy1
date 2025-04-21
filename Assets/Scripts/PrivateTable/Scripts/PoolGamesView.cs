using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PoolGamesView : MonoBehaviour
{
    public SlidingEffect slidingEffect;
    public Button backBtn;

    #region PRIVATE_VARS
    private WaitForSeconds startDelay = new WaitForSeconds(0.15f);
    private WaitForSeconds disableDelay = new WaitForSeconds(0.6f);
    #endregion

    void Start()
    {
        backBtn.onClick.AddListener(() => 
        {
            CloseScreen();
        });
    }
    private void OnEnable()
    {
        StartCoroutine(EntryEffect());
    }

    private IEnumerator EntryEffect()
    {
        yield return startDelay;
        StartCoroutine(slidingEffect.EntryEffect());
    }

    private IEnumerator ExitEffect()
    {
        StartCoroutine(slidingEffect.ExitEffect());
        yield return disableDelay;
        gameObject.SetActive(false);
    }

    public void CloseScreen()
    {
        StartCoroutine(ExitEffect());

    }


    void Update()
    {
        
    }
}
