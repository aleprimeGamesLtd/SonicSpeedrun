using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class jumpball_rot : MonoBehaviour
{
    public float rotationSpeed = 10;
    float currentRot;

    void Update()
    {
        currentRot += rotationSpeed;
        transform.rotation = Quaternion.Euler(currentRot, transform.eulerAngles.y, transform.eulerAngles.z);
        Debug.Log(transform.localEulerAngles.ToString("F2"));
    }
}
