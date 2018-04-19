using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(LuisTester))]
public class LuisTesterEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        if (Application.isPlaying)
        {
            if (GUILayout.Button("Predict"))
            {
				LuisTester tester = (LuisTester)target;
				if (tester != null)
				{
					tester.TryPredict();
				}
            }
        }
    }

}
