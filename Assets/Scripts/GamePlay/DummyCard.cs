using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class DummyCard : MonoBehaviour
{


	[SerializeField]
	private Image CardImage;
	public GameObject RightCollider, LeftCollider, CenterCollider;

	public void SetCardSprite(Sprite CardSprite)
	{
		CardImage.sprite = CardSprite;
	}


	public Sprite GetCardSprite()
	{
		return CardImage.sprite;
	}
}
