using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class CircleLoading : MonoBehaviour
{

	public TextMeshProUGUI LoadingInt;
	public Image circleLoading;
	float loadValue;
	public float speed;

	void Update()
	{
		if (loadValue < 100)
		{
			loadValue += speed * Time.deltaTime;
			LoadingInt.text = ((int)loadValue).ToString();
			
		}


		circleLoading.fillAmount = loadValue / 100;
	}
}