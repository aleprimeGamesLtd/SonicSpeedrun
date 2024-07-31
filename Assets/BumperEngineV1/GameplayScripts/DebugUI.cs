using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class DebugUI : MonoBehaviour {

    public PlayerBhysics phys;
    public ActionManager action;
    public PlayerBinput bimp;

    public Vector3 inputDirection;
    public float inputMagnitude;

    public Vector3 velocity;
    public Vector3 localVelocity;

    public float normalSpeed;
    public Vector3 normalVelocity;
    public Vector3 tangentVelocity;

    public Vector3 modTangent;



    void Update () {

        string debug = " DEBUG \n" +
            " Position: " + phys.transform.position + "\n" +
            " Rotation: " + phys.transform.eulerAngles + "\n" +
            " SkinRotation: " + action.Action00.CharacterAnimator.transform.eulerAngles + "\n" +
            " Speed: " + phys.rigidbody.velocity + "\n" +
            " SpeedMagnitude: " + phys.rigidbody.velocity.magnitude + "\n" +
            " SlopePower: " + phys.curvePosSlope + "\n" +
            " Grounded: " + phys.Grounded + "\n" +
            " TangentialDragOver: " + phys.curvePosTang + "\n" +
            " InputDirection: " + bimp.moveInp + "\n" +
            " InputMagnitude: " + bimp.moveInp.magnitude + "\n" +
            " LockInput: " + bimp.LockInput + "\n" +
            " LockedInputCounter: " + bimp.LockedCounter + "\n" +
            " NormalSpeed: " + phys.b_normalSpeed + "\n" +
            " NormalVelocity: " + phys.b_normalVelocity + "\n" +
            " TangentVelocity: " + phys.b_tangentVelocity + "\n" +
            " Action: " + action.Action + "\n" +
            " AnimatorAction: " + action.Action00.CharacterAnimator.GetInteger("Action") + "\n" +
            " Input A: " + Input.GetButton("A") + "\n" +
            " Input APress: " + Input.GetButtonDown("A") + "\n" +
            " GroundNormal: " + phys.GroundNormal + "\n" +
            " FPS: " + 1 / Time.deltaTime;

        gameObject.GetComponent<Text>().text = debug;
	
	}
}
