using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QuickstepTrigger : MonoBehaviour
{
    public int Value;
    public Action00_Regular Action00;

    void OnTriggerStay(Collider col)
    {
        if (Action00.quickstep == 0)
        { 
            Action00.quickstep = Value;
        }
    }

    void OnTriggerExit(Collider col)
    {
        if (Action00.quickstep == Value)
        { 
            Action00.quickstep = 0;
        }
    }
}
