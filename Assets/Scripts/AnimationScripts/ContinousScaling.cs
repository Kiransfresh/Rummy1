using UnityEngine;

public class ContinousScaling : MonoBehaviour {

    #region PUBLIC_VARS
    [Header("Animation Curve")]
    public AnimationCurve animationCurve;
    public float scalingMultiplier;
    #endregion

    #region PRIVATE_VARS
    private Vector3 initialScale;
    private Vector3 finalScale;
    private float graphValue;
    #endregion

    #region UNITY_CALLBACKS
    private void Awake()
    {
        initialScale = transform.localScale;
        finalScale = Vector3.one* scalingMultiplier; 
        animationCurve.postWrapMode = WrapMode.PingPong;
    } 

    private void Update () {
        graphValue = animationCurve.Evaluate(Time.time);
        transform.localScale = (finalScale * graphValue);
	}
    #endregion
}
