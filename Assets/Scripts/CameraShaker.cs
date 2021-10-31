using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraShaker : MonoBehaviour
{
    public MilkShake.Shaker cameraToShake;

    public MilkShake.ShakePreset shakePreset;

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.E))
        {
            cameraToShake.Shake(shakePreset);
        }
    }
}
