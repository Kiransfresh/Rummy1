using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
[CustomEditor(typeof(FortuneWheel))]
public class FortuneWheelEditor : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        FortuneWheel Wheel = (FortuneWheel)target;

        if (GUILayout.Button("Generate Wheel")) {

            Wheel.GenerateSlices();
        }
        if (GUILayout.Button("Refresh Timer"))
        {

            Wheel.RefreshTimer();
        }

    }
}
