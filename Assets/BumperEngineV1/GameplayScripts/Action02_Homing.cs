﻿using UnityEngine;
using System.Collections;

public class Action02_Homing : MonoBehaviour {

    public ActionManager Action;
    public Animator CharacterAnimator;
    PlayerBhysics Player;

    public bool isAdditive;
    public float HomingAttackSpeed;
    public float AirDashSpeed;
    public float HomingTimerLimit;
    public float FacingAmount;
	public GameObject HomingTrailContainer;
	public GameObject HomingTrail;
	public GameObject JumpDashParticle;
    public GameObject JumpBall;
    float Timer;
    float Speed;
    float Aspeed;
    Vector3 direction;


    public Transform Target { get; set; }
    public float skinRotationSpeed;
    public bool HomingAvailable { get; set; }
    public bool IsAirDash { get; set; }

    void Awake()
    {
        HomingAvailable = true;
        Player = GetComponent<PlayerBhysics>();
    }

    public void InitialEvents()
    {

		Action.Action01.JumpBall.SetActive(false);
		if (HomingTrailContainer.transform.childCount < 1) 
		{
			GameObject HomingTrailClone = Instantiate (HomingTrail, HomingTrailContainer.transform.position, Quaternion.identity) as GameObject;
			HomingTrailClone.transform.parent = HomingTrailContainer.transform;

			GameObject JumpDashParticleClone = Instantiate (JumpDashParticle, HomingTrailContainer.transform.position, Quaternion.identity) as GameObject;
			JumpDashParticleClone.transform.parent = HomingTrailContainer.transform;
		}
			

        if (Action.Action02Control.HasTarget)
        {
            Target = HomingAttackControl.TargetObject.transform;
            HomingAvailable = true;
        }
        else
        {
            HomingAvailable = false;
        }

        Timer = 0;

        if (isAdditive)
        {
			
            // Apply Max Speed Limit
            float XZmag = new Vector3(Player.rigidbody.velocity.x, 0, Player.rigidbody.velocity.z).magnitude;
            float Ymag = new Vector3(0, Player.rigidbody.velocity.y, 0).magnitude;

            if (XZmag < HomingAttackSpeed)
            {
                Speed = HomingAttackSpeed;
            }
            else
            {
                Speed = XZmag;
            }

            if(Ymag < AirDashSpeed)
            {
                Aspeed = AirDashSpeed;
            }
            else
            {
                Aspeed = Ymag;
            }
        }
        else
        {
            Aspeed = AirDashSpeed;
            Speed = HomingAttackSpeed;
        }

        //Check if not facing Object
        if (!IsAirDash)
        {
            Vector3 TgyXY = HomingAttackControl.TargetObject.transform.position.normalized;
            TgyXY.y = 0;
            float facingAmmount = Vector3.Dot(Player.PreviousRawInput.normalized, TgyXY);
           // //Debug.Log(facingAmmount);
           // if (facingAmmount < FacingAmount) { IsAirDash = true; }
        }

    }

    void Update()
    {

        //Set Animator Parameters
        CharacterAnimator.SetInteger("Action", 1);
        CharacterAnimator.SetFloat("YSpeed", Player.rigidbody.velocity.y);
        CharacterAnimator.SetFloat("GroundSpeed", Player.rigidbody.velocity.magnitude);
        CharacterAnimator.SetBool("Grounded", Player.Grounded);

        //Set Animation Angle
        Vector3 VelocityMod = new Vector3(Player.rigidbody.velocity.x, 0, Player.rigidbody.velocity.z);
        Quaternion CharRot = Quaternion.LookRotation(VelocityMod, transform.up);
        CharacterAnimator.transform.rotation = Quaternion.Lerp(CharacterAnimator.transform.rotation, CharRot, Time.deltaTime * skinRotationSpeed);



    }

    void FixedUpdate()
    {
        Timer += 1;

        CharacterAnimator.SetInteger("Action", 1);

        if (IsAirDash)
        {
            Vector3 XZmag = new Vector3(Player.rigidbody.velocity.x, 0, Player.rigidbody.velocity.z);
            Player.rigidbody.velocity = XZmag + Vector3.up * Aspeed;
            Timer = HomingTimerLimit + 10;
            JumpBall.SetActive(true);
        }
        else
        {
            direction = Target.position - transform.position;
            Player.rigidbody.velocity = direction.normalized * Speed;
            JumpBall.SetActive(false);
        }

		//Set Player location when close enough, for precision.
		if (Target != null && Vector3.Distance (Target.position, transform.position) < 5) 
		{
			transform.position = Target.position;
			//Debug.Log ("CloseEnough");
		}

        //End homing attck if on air for too long
        if (Timer > HomingTimerLimit)
        {
            Action.ChangeAction(0);
        }
    }

    public void ResetHomingVariables()
    {
        Timer = 0;
        HomingTrailContainer.transform.DetachChildren();
        //IsAirDash = false;
    }



}