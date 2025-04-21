using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class OrientationController 
{
    public IEnumerator ChangeToLandscape(GameObject panel1, GameObject panel2)
    {
        var preOrient = ScreenOrientation.Portrait;
        if (preOrient != ScreenOrientation.Portrait)
        {
            panel1.SetActive(false);
        }
        Screen.orientation = ScreenOrientation.LandscapeLeft;
        var orient = ScreenOrientation.LandscapeLeft;
        yield return new WaitForFixedUpdate();
        if (orient == ScreenOrientation.LandscapeLeft)
        {
            panel2.SetActive(true);
        }
    }

    public IEnumerator ChangeToPortrait(GameObject panel1, GameObject panel2)
    {
        panel1.SetActive(false);
        Screen.orientation = ScreenOrientation.Portrait;
        var orient = ScreenOrientation.Portrait;
        yield return new WaitForFixedUpdate();
        if (orient == ScreenOrientation.Portrait)
        {
            panel2.SetActive(true);
        }
    }
}
