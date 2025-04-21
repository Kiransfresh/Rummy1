using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PrivateTableGameSelectionView : BaseMonoBehaviour
{
    public SlidingEffect[] slidingEffect;
    public FadingColorEffect[] fadingColorEffect;
    private WaitForSeconds startDelay;
    private WaitForSeconds disableDelay;

    public Button backBtn;
    public Button pool101GameBtn;
    public Button pool201GameBtn;
    public Button dealGameBtn;
    public Button pointGameBtn;

    private void Awake()
    {
        startDelay = new WaitForSeconds(0.15f);
        disableDelay = new WaitForSeconds(0.6f);
    }

    private void OnEnable()
    {
        StartCoroutine(DealRummyEntryEffect());
    }

    private void Start()
    {
        backBtn.onClick.AddListener(() =>
        {
            DisablePrivateTableSelectionPanel();
        });


        pool101GameBtn.onClick.AddListener(() =>
        {
            UIManager.instance.lobbyView.isPrivateTable = false;
            UIManager.instance.poolRummyView.selectedPoolType = Constants.POOL_TYPE.POOL_101.ToString();
            UIManager.instance.lobbyView.PoolRummyEnable();
        });

        pool201GameBtn.onClick.AddListener(() =>
        {
            UIManager.instance.lobbyView.isPrivateTable = false;
            UIManager.instance.poolRummyView.selectedPoolType = Constants.POOL_TYPE.POOL_201.ToString();
            UIManager.instance.lobbyView.PoolRummyEnable();
        });

        dealGameBtn.onClick.AddListener(() =>
        {
            UIManager.instance.lobbyView.DealRummyEnable();
        });

        pointGameBtn.onClick.AddListener(() =>
        {
            UIManager.instance.lobbyView.PointRummyEnable();
        });
    }

    private void PlayStartEffects()
    {

        for (int i = 0; i < slidingEffect.Length; i++)
        {
            StartCoroutine(slidingEffect[i].EntryEffect());
        }

        for (int i = 0; i < fadingColorEffect.Length; i++)
        {
            StartCoroutine(fadingColorEffect[i].ChangeColor());
        }
    }

    private void PlayEndEffect()
    {
        for (int i = 0; i < slidingEffect.Length; i++)
        {
            StartCoroutine(slidingEffect[i].ExitEffect());
        }

        for (int i = 0; i < fadingColorEffect.Length; i++)
        {
            StartCoroutine(fadingColorEffect[i].SetInitialColor());
        }
    }

    private IEnumerator DealRummyEntryEffect()
    {
        yield return startDelay;
        PlayStartEffects();
    }

    public IEnumerator PrivateTableSelectionExitEffect()
    {
        PlayEndEffect();
        yield return disableDelay;
        gameObject.SetActive(false);
    }

    public void DisablePrivateTableSelectionPanel()
    {
        StartCoroutine(PrivateTableSelectionExitEffect());
    }
}
