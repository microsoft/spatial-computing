using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(LUISTester))]
public class LUISTesterEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        if (Application.isPlaying)
        {
            if (GUILayout.Button("Predict"))
            {
                LUISTester tester = (LUISTester)target;
                tester.luisManager.PredictAndHandle(tester.testUtterence);
            }
        }
    }

}
