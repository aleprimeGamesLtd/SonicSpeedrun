using UnityEngine;
using System.Collections;

public class Action05_Rail : MonoBehaviour {

    public Animator CharacterAnimator;
    public Quaternion CharRot;

    public Rail_Interaction rail;
    public float skinRotationSpeed = 1;

    void Update()
    {

        //Set Player's rotation while on rails
        if (rail != null)
        {
            CharacterAnimator.SetInteger("Action", 5);
            CharacterAnimator.transform.rotation = Quaternion.Lerp(transform.rotation, CharRot, Time.deltaTime * skinRotationSpeed);
        }
    }

}
