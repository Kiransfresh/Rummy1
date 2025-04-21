using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameRoomCanvas : MonoBehaviour
{
    public SlidingEffect slidingEffect;

    private WaitForSeconds delay;
    [SerializeField] private Button cashDetailsBtn;

    private void OnEnable()
    {
      //  cashDetailsBtn.gameObject.SetActive(Constants.CONFIG.is_paid);
    }

    private void Awake()
    {
        delay = new WaitForSeconds(0.6f);
    }

    public void DisableRoom()
    {
        StartCoroutine(PoolRummyExitEffect());
    }

    private IEnumerator PoolRummyExitEffect()
    {
        StartCoroutine(slidingEffect.ExitEffect());
        yield return delay;
        gameObject.SetActive(false);
    }
}
