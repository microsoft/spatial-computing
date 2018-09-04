using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using HoloToolkit.Unity.InputModule;

public class OutdoorTrigger : MonoBehaviour, IInputClickHandler
{
    public SkyboxVideo skyboxManager;

    public void OnInputClicked(InputClickedEventData eventData)
    {
        skyboxManager.SwitchToOutdoorTheater();
    }
}
