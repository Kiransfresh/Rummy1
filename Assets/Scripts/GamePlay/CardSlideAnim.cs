using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class CardSlideAnim : MonoBehaviour
{
    public Vector3 TargetPos;

    public Vector3 EndingScale, offset;

    public AnimationCurve AnimCurve;

    [SerializeField]
    private float time = 0.25f;

    Vector3 velocity = Vector3.zero;

    public  bool TakingAnim, DiscardAnim;
    Sprite DefaultSprite;//**
    public bool IsFinishCard;
    private void Awake()
    {
        DefaultSprite = GetComponent<Image>().sprite;
    }

    private void OnEnable()
    {
        if(IsFinishCard)
        GetComponent<Image>().sprite = DefaultSprite;
    }

    private void Update()
    {
        if (TakingAnim)
        {
            transform.position = Vector3.SmoothDamp(transform.position, TargetPos + offset, ref velocity, time);
            transform.localScale = Vector3.Lerp(transform.localScale, EndingScale, Time.deltaTime);
            if (Mathf.RoundToInt(Vector3.Distance(transform.position, TargetPos + offset)) <= 0)
            {
                transform.localPosition = Vector3.zero;

                transform.localScale = new Vector3(1, 1, 1);
                if(this== GamePlayManager.instance.cardForAnimation)
                GamePlayManager.instance.cardForAnimation.gameObject.SetActive(false);
                GamePlayManager.instance.isAnimationFinish = true;
                GamePlayManager.instance.OnCardAnimationEnd();
                TakingAnim = false;
            }
        }
        if (DiscardAnim)
        {
            transform.localPosition = Vector3.SmoothDamp(transform.localPosition, Vector3.zero, ref velocity, time);
            transform.localScale = Vector3.Lerp(transform.localScale, new Vector3(1, 1, 1), Time.deltaTime * 16);
            if (Mathf.RoundToInt(Vector3.Distance(transform.localPosition, Vector3.zero)) <= 0)
            {
                GamePlayManager.instance.isAnimationFinish = true;

                DiscardAnim = false;

                if (this == GamePlayManager.instance.cardForAnimation)
                    GamePlayManager.instance.cardForAnimation.gameObject.SetActive(false);
            }
        }
    }

    public void TakeOpenCard(Vector3 TargetPosition)
    {
        if (DiscardAnim == false)
        {
            TargetPos = TargetPosition;
            TakingAnim = true;
            GamePlayManager.instance.isAnimationFinish = false;
        }
        else
        {
            Debug.Log("Discard Animation Still Going On");
        }
    }
    public void DiscardAnimation(Vector3 _sourcePos)
    {
        if (TakingAnim == false)
        {
            transform.position = _sourcePos;
            DiscardAnim = true;
            GamePlayManager.instance.isAnimationFinish = false;
        }
        GamePlayManager.instance.SendGroup();
    }

   
    public void CheckForDiscardAnimStatus() {

        if (this == GamePlayManager.instance.cardForAnimation && DiscardAnim) {

            transform.localPosition = Vector3.zero;
            GamePlayManager.instance.isAnimationFinish = true;
            DiscardAnim = false;
            GamePlayManager.instance.cardForAnimation.gameObject.SetActive(false);
            Debug.Log(" Discard Anim status setting Right");
        }
}
    private void OnDisable()
    {
        CheckForDiscardAnimStatus();
    }
    public Sprite GetOpenCardSprite()
    {
        return GetComponent<Image>().sprite;
    }
    public void SetOpenCardSprite(Sprite OpenCardImage)
    {
        GetComponent<Image>().sprite = OpenCardImage;
    }
}