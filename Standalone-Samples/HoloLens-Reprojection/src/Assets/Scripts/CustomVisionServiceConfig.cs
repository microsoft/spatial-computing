using UnityEngine;

[CreateAssetMenu(fileName = "CustomVisionServiceConfig", menuName = "Azure/CustomVisionServiceConfig", order = 1)]
public class CustomVisionServiceConfig : ScriptableObject
{
    public string PredictionEndpoint;

    public string PredictionKey;

    public int RequestTimeout;
}
