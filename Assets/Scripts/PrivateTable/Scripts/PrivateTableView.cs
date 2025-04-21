using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PrivateTableView : BaseMonoBehaviour
{
    public SlidingEffect[] slidingEffect;
    public FadingColorEffect[] fadingColorEffect;
    public Button backBtn;
    private WaitForSeconds startDelay;
    private WaitForSeconds disableDelay;

    public PrivateTableGameSelectionView privateTableGameSelectionView;
    public Button createTableBtn;
    public Button joinTableBtn;

    public JoinPrivateTable joinPrivateTable;

    private void Awake()
    {
        startDelay = new WaitForSeconds(0.15f);
        disableDelay = new WaitForSeconds(0.6f);
    }

    private void OnEnable()
    {
        StartCoroutine(PrivateTableViewEntryEffect());
    }

    private void Start()
    {
        backBtn.onClick.AddListener(() =>
        {
            DisablePrivateTable();
        });

        createTableBtn.onClick.AddListener(() =>
        {
            StartCoroutine(PrivateTableExitEffect());
            privateTableGameSelectionView.gameObject.SetActive(true);
        });

        joinTableBtn.onClick.AddListener(() =>
        {
            joinPrivateTable.gameObject.SetActive(true);
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

    private IEnumerator PrivateTableViewEntryEffect()
    {
        yield return startDelay;
        PlayStartEffects();
    }

    public IEnumerator PrivateTableExitEffect()
    {
        PlayEndEffect();
        yield return disableDelay;

        gameObject.SetActive(false);
    }

    public void DisablePrivateTable()
    {
        StartCoroutine(PrivateTableExitEffect());
    }
}
