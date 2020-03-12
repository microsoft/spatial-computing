using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LookAtCamera : MonoBehaviour
{
    public GameObject objectToLookAt;

    void LateUpdate()
    {
        transform.LookAt(2 * transform.position - objectToLookAt.transform.position);
    }
}
