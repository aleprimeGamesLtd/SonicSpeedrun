using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MouthManager : MonoBehaviour
{
    public GameObject Mouth1;
    public GameObject Mouth2;
    public Transform HeadReference;
    public Transform Camera;
    Vector3 dir;

    void Update()
    {
        dir = Quaternion.Euler(Vector3.up * HeadReference.eulerAngles.y).eulerAngles;
        if (dir.y < Camera.eulerAngles.y)
        {
            Mouth1.SetActive(true);
            Mouth2.SetActive(false);
        }
        else
        {
            Mouth1.SetActive(false);
            Mouth2.SetActive(true);
        }
    }
}
