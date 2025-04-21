using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class BlinkingEffect : MonoBehaviour
 {
	#region PUBLIC_VARS

	[Header("Animation Curve")]
	public AnimationCurve colorAlphaCurve;
	public Text myText;

	#endregion

	#region PRIVATE_VARS

	private Color initialColor;
	private float graphValue;


	#endregion

	#region UNITY_CALLBACKS

	private void Awake()
	{
		initialColor = myText.color;
		colorAlphaCurve.postWrapMode = WrapMode.PingPong;
	}

	private void Update()
	{
		initialColor.a  = colorAlphaCurve.Evaluate (Time.time);
		myText.color = initialColor;
	}
	#endregion
}