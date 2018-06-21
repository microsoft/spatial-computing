using System.Collections;
using System.Collections.Generic;
using UnityEditor;

[CustomEditor(typeof(AzureBlobStorageClient))]
public class AzureBlobStorageManagerEditor : Editor {

    public override void OnInspectorGUI()
    {
        //base.OnInspectorGUI();
        DrawDefaultInspector();

        EditorGUILayout.HelpBox("This script contains a proxy class for all Azure Blob Storage operations in your scene. " +
                                "For more information on setting-up your Azure Blob Storage account, please visit " +
                                "https://azure.microsoft.com/services/storage/blobs/.", MessageType.Info);
    }
}
