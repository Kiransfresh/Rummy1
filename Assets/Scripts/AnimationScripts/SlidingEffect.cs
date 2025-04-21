using UnityEngine;
using System.Collections;

public class SlidingEffect : MonoBehaviour
{
	#region PUBLIC_VARS

	[Header ("Animation-Curve")]
	public AnimationCurve slideCurve;
	public Vector3 finalPosition;
	public float startDelay;
	public float exitDelay;

	#endregion

	#region PRIVATE_VARS

	private Vector3 initialPosition;
	private WaitForSeconds startingDelay;
	private WaitForSeconds endingDelay;
	private float time = 0.275f;

	#endregion

	#region UNITY_CALLBACKS

	private void Awake ()
	{
		startingDelay = new WaitForSeconds (startDelay);
		endingDelay = new WaitForSeconds (exitDelay);
		initialPosition = transform.localPosition;
	}

	#endregion

	#region CO-ROUTINESprivate

	public IEnumerator EntryEffect ()
	{
		if(UIManager.instance.lobbyView.gameObject.activeInHierarchy && GetComponent<AccountMenuView>() == null)
		AudioController.instance.PlayPanelSlide();
		float i = 0;
		float rate = 2 / time;
    		float doublerate = 4/time;

		yield return startingDelay;

		while (i < 1) {
			i += rate * Time.deltaTime * doublerate;
			transform.localPosition = Vector3.Lerp (initialPosition, finalPosition, slideCurve.Evaluate (i));
			yield return 0;
		}
	}

	public IEnumerator ExitEffect ()
	{
		if (UIManager.instance.lobbyView.gameObject.activeInHierarchy && GetComponent<AccountMenuView>()==null)
			AudioController.instance.PlayPanelSlide();
		float i = 0;
		float rate = 1 / time;
		float doublerate = 4/time;
		yield return endingDelay;

		while (i < 1) {
			i += rate * Time.deltaTime * doublerate;
			transform.localPosition = Vector3.Lerp (finalPosition, initialPosition, slideCurve.Evaluate (i));
			yield return 0;
		}
	}

	#endregion
}
