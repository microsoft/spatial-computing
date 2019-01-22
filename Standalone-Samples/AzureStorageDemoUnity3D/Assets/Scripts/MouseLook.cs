//*********************************************************
//
// Copyright (c) Microsoft. All rights reserved.
// This code is licensed under the MIT License (MIT).
// THIS CODE IS PROVIDED *AS IS* WITHOUT WARRANTY OF
// ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING ANY
// IMPLIED WARRANTIES OF FITNESS FOR A PARTICULAR
// PURPOSE, MERCHANTABILITY, OR NON-INFRINGEMENT.
//
//*********************************************************

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MouseLook : MonoBehaviour
{
    public enum RotationAxes { MouseXAndY = 0, MouseX = 1, MouseY = 2 };
    public RotationAxes axes = RotationAxes.MouseXAndY;
    public float sensitivityX = 15F;
    public float sensitivityY = 15F;

    public float minimumX = -360F;
    public float maximumX = 360F;

    public float minimumY = -60F;
    public float maximumY = 60F;

    public bool disableWhenXRDevicePresented = true;

    float rotationX = 0F;
    float rotationY = 0F;

    float startX = 0F;
    float startY = 0F;
    float startRotY = 0F;

    Quaternion originalRotation;

    void Update()
    {
        if (Input.GetMouseButtonDown(1))
        {
            startX = Input.GetAxis("Mouse X");
            startY = Input.GetAxis("Mouse Y");
            rotationX = 0F;
            rotationY = 0F;
            originalRotation = transform.localRotation;
        }
            // Mouse look only allowed on right-click drag
        else if (Input.GetMouseButton(1))
        {
            if (axes == RotationAxes.MouseXAndY)
            {
                // Read the mouse input axis
                rotationX += (Input.GetAxis("Mouse X") - startX) * sensitivityX;
                rotationY += (Input.GetAxis("Mouse Y") - startY) * sensitivityY;

                rotationX = ClampAngle(rotationX, minimumX, maximumX);
                rotationY = startRotY + ClampAngle(rotationY, minimumY, maximumY);

                Quaternion xQuaternion = Quaternion.AngleAxis(rotationX, Vector3.up);
                Quaternion yQuaternion = Quaternion.AngleAxis(rotationY, -Vector3.right);

                transform.localRotation = originalRotation * xQuaternion * yQuaternion;
            }
            else if (axes == RotationAxes.MouseX)
            {
                rotationX += Input.GetAxis("Mouse X") * sensitivityX;
                rotationX = ClampAngle(rotationX, minimumX, maximumX);

                Quaternion xQuaternion = Quaternion.AngleAxis(rotationX, Vector3.up);
                transform.localRotation = originalRotation * xQuaternion;
            }
            else
            {
                rotationY += Input.GetAxis("Mouse Y") * sensitivityY;
                rotationY = ClampAngle(rotationY, minimumY, maximumY);

                Quaternion yQuaternion = Quaternion.AngleAxis(-rotationY, Vector3.right);
                transform.localRotation = originalRotation * yQuaternion;
            }
        }
        else if (Input.GetMouseButtonUp(1))
        {

        }
    }

    void Start()
    {
#if UNITY_2017_2_OR_NEWER
        bool hasMR = UnityEngine.XR.XRDevice.isPresent;
#else
        bool hasMR = UnityEngine.VR.VRDevice.isPresent;
#endif
        if(hasMR && disableWhenXRDevicePresented)
        {
            enabled = false;
            return;
        }

        // Make the rigid body not change rotation
        if (GetComponent<Rigidbody>())
            GetComponent<Rigidbody>().freezeRotation = true;
        originalRotation = transform.localRotation;
    }

    public static float ClampAngle(float angle, float min, float max)
    {
        if (angle < -360F)
            angle += 360F;
        if (angle > 360F)
            angle -= 360F;
        return Mathf.Clamp(angle, min, max);
    }
}