using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OutdoorTrigger : MonoBehaviour
{
    public SkyboxVideo skyboxManager;

    public void OnInputClicked()
    {
        skyboxManager.SwitchToOutdoorTheater();
    }
}
