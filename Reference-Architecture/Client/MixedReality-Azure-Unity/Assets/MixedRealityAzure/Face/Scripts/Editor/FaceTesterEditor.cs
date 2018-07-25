using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(FaceTester))]
public class FaceTesterEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        if (Application.isPlaying)
        {
            if (GUILayout.Button("Detect"))
            {
				FaceTester tester = (FaceTester)target;
				if (tester != null)
				{
					tester.TryDetect();
				}
            }
        }
    }

}
