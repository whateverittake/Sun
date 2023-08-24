using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SunController : MonoBehaviour
{
    Light light;

    private void Start()
    {
        light = GetComponent<Light>();
    }
    public void SetPosition(float alt, float azi)
    {
        Vector3 angles = new Vector3();
        angles.x = (float)alt;
        angles.y = (float)azi;
        transform.localRotation = Quaternion.Euler(angles);
        light.intensity = Mathf.InverseLerp(-12, 0, angles.x);
    }
}
