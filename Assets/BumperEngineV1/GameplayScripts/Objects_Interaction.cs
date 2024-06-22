using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using UnityEngine.Playables;
using System;
using UnityEngine.SceneManagement;

public enum AnimationType
{
    Basic, Dynamic, Rolling, DashPad
}

public class Objects_Interaction : MonoBehaviour {

    [Header("For Rings, Springs and so on")]

    public PlayerBhysics Player;
    public Transform PlayerSkin;
    public HedgeCamera Cam;
    public SonicSoundsControl Sounds;
    public ActionManager Actions;
    public PlayerBinput Inp;
    public SonicSoundsControl sounds;
    Spring_Proprieties spring;
    int springAmm;
    float playTime = 0;
    public bool lockedSpeed;
    bool lockedOnAir;
    Vector3 lockedSpeedValue;

    public GameObject RingCollectParticle;
    public Material SpeedPadTrack;
    public Material DashRingMaterial;

    [Header("Enemies")]

    public float BouncingPower;
	public float HomingBouncingPower;
	public float EnemyHomingStoppingPowerWhenAdditive;
    public bool StopOnHomingAttackHit;
    public bool StopOnHit;
    public bool updateTargets { get; set; }

    public float EnemyDamageShakeAmmount;
    public float EnemyHitShakeAmmount;

    [Header("UI objects")]

    public PauseCotrol PauseCotrol;
    public Text RingsCounter;
    public Text ScoreCounter;

    public static int RingAmount { get; set; }
    public static int Score { get; set; }

    MovingPlatformControl Platform;
    Vector3 TranslateOnPlatform;
    public Color DashRingLightsColor;
    private bool CanHitAgain = true;
    public bool speedSec;
    public bool Mode2D;
    float CamInitialDistance;
    Vector3 InitialGravity;
    float InitialAcell;

    private void Awake()
    {
        RingAmount = 0;
        Score = 0;
        CamInitialDistance = Cam.CameraMaxDistance;
        InitialGravity = Player.Gravity;
        InitialAcell = Player.MoveAccell;
    }

    void Update()
    {
        RingsCounter.text = ": " + RingAmount.ToString("F0");
        if (ScoreCounter != null)
        {
            ScoreCounter.text = "Score: " + Score.ToString("F0");
        }
        
        if (updateTargets)
        {
            HomingAttackControl.UpdateHomingTargets();
			if (Actions.Action02 != null) {
				if (Actions.Action02 != null) {
			Actions.Action02.HomingAvailable = true;
		}
			}
            updateTargets = false;
        }

        //Set speed pad trackpad's offset
        SpeedPadTrack.SetTextureOffset("_MainTex", new Vector2(0, -Time.time) * 3);
        DashRingMaterial.SetColor("_EmissionColor", (Mathf.Sin(Time.time * 15) * 1.3f) * DashRingLightsColor);
        
        if (lockedSpeed)
        {
            if (Inp.LockInput)
            {
                if (lockedOnAir)
                {
                    Player.rigidbody.velocity = lockedSpeedValue;
                }
                else 
                {
                    if (Player.Grounded)
                    {
                        Player.rigidbody.velocity = PlayerSkin.forward * lockedSpeedValue.magnitude;
                    }
                }
            }
            else
            {
                lockedSpeed = false;
            }
        }

        if (GameObject.FindGameObjectWithTag("WaterSurface"))
        {
            MeshCollider surfaceMesh = GameObject.FindGameObjectWithTag("WaterSurface").gameObject.GetComponent<MeshCollider>();
            if (Player.XZmag >= Player.TopSpeed)
            {
                surfaceMesh.enabled = true;
            }
            else
            {
                surfaceMesh.enabled = false;
            }
        }
    }

	private IEnumerator ResetTriggerBool()
	{
		CanHitAgain = false;
		yield return new WaitForSeconds (0.05f);
		CanHitAgain = true;
	}

    void FixedUpdate()
    {
        if(Platform != null)
        {
            transform.position += (-Platform.TranslateVector);
        }
        if (!Player.Grounded)
        {
            Platform = null;
        }
    }

	public void OnTriggerEnter(Collider col)
    {
        //Speed Pads Collision
        if (col.tag == "SpeedTrigger")
        {
            speedSec = true;
            if (GetComponent<PlayerBinput>().LockInput) { GetComponent<PlayerBinput>().LockInput = false; }
        }

        if (col.tag == "StageSelector")
        {
            Actions.ChangeAction(0);
            PauseCotrol.StageSelector.SetActive(true);
            Player.rigidbody.velocity = Vector3.zero;
            transform.position = col.transform.position;
            transform.rotation = col.transform.rotation;
            PlayerSkin.rotation = col.transform.rotation;
        }

        if(col.tag == "SpeedPad")
        {
			Actions.Action01.JumpBall.SetActive(false);
			if (Actions.Action08 != null) {
				if (Actions.Action08.DropEffect.isPlaying == true) {
					Actions.Action08.DropEffect.Stop ();
				}
			}
            if(col.GetComponent<SpeedPadData>() != null)
            {
                Player.transform.rotation = col.transform.rotation;
                Actions.Action00.CharacterAnimator.transform.rotation = col.transform.rotation;

                if (col.GetComponent<SpeedPadData>().LockControl)
                {
                    Inp.LockInputForAWhile(col.GetComponent<SpeedPadData>().LockControlTime, true);
                    lockedSpeed = true;
                    lockedSpeedValue = col.transform.forward * col.GetComponent<SpeedPadData>().Speed;
                }
                if (col.GetComponent<SpeedPadData>().Snap)
                {
                    transform.position = col.transform.position + col.GetComponent<BoxCollider>().center;
                }
                if (col.GetComponent<SpeedPadData>().LockToDirection)
                {
                    Player.rigidbody.velocity = col.transform.forward * col.GetComponent<SpeedPadData>().Speed;
                }
                else
                {
                    Player.AddVelocity(col.transform.forward * col.GetComponent<SpeedPadData>().Speed);
                }
                if (col.GetComponent<SpeedPadData>().isDashRing)
                {
                    Actions.ChangeAction(0);
                    Actions.Action00.CharacterAnimator.SetBool("Grounded", false);
                    lockedOnAir = true;
                    Player.KeepNormalCounter = 16;
                }
                else 
                { 
                    lockedOnAir = false;
                }
                if (col.GetComponent<SpeedPadData>().AffectCamera)
                {
                    Vector3 dir = col.transform.forward;
                    Cam.SetCamera(dir, 0, Cam.HeightToLock);
                    Cam.RotateDirection(dir, (int)Cam.CameraMoveSpeed, Cam.HeightToLock);
                }

                switch (col.GetComponent<SpeedPadData>().AnimationType)
                {
                    case AnimationType.Basic:
                        Actions.Action00.CharacterAnimator.SetInteger("Action", 0);
                        break;
                    case AnimationType.Dynamic:
                        Actions.Action00.CharacterAnimator.SetInteger("Action", -1);
                        Actions.Action00.CharacterAnimator.SetBool("Grounded", false);
                        break;
                    case AnimationType.DashPad:
                        Actions.Action00.CharacterAnimator.SetInteger("Action", -2);
                        Actions.Action00.CharacterAnimator.SetBool("Grounded", false);
                        break;
                    case AnimationType.Rolling:
                        Actions.Action00.CharacterAnimator.SetInteger("Action", 1);
                        Actions.Action00.CharacterAnimator.SetBool("isRolling", true);
                        break;
                }


                col.GetComponent<AudioSource>().Play();
            }
        }

        //Rings Collision
        if (col.tag == "Ring")
        {
			Instantiate(RingCollectParticle, col.transform.position, Quaternion.identity);
			Destroy(col.gameObject);
			StartCoroutine(IncreaseRing ());
            
            
        }
        if (col.tag == "MovingRing")
        {
            if (col.GetComponent<MovingRing>() != null)
            {
                if (col.GetComponent<MovingRing>().colectable)
                {
					StartCoroutine(IncreaseRing ());
                    Instantiate(RingCollectParticle, col.transform.position, Quaternion.identity);
                    Destroy(col.gameObject);
                }
            }
        }

		//Switch
		if(col.tag == "Switch")
		{	
			if (col.GetComponent<Switch_Properties> () != null) {
				col.GetComponent<Switch_Properties> ().Activate ();
			}
		}

        //Hazard
        if(col.tag == "Hazard")
        {
			Actions.Action01.JumpBall.SetActive(false);
			if (Actions.Action08 != null) {
				if (Actions.Action08.DropEffect.isPlaying == true) {
					Actions.Action08.DropEffect.Stop ();
				}
			}
            DamagePlayer();
            HedgeCamera.Shakeforce = EnemyDamageShakeAmmount;
        }

        //Enemies
        if (col.tag == "Enemy")
        {
            HedgeCamera.Shakeforce = EnemyHitShakeAmmount;
            //If 1, destroy, if not, take damage.
            if (Actions.Action == 3)
            {
                col.transform.parent.GetComponent<EnemyHealth>().DealDamage(1);
                updateTargets = true;
                Score += 1000;
            }
            if (Actions.Action00.CharacterAnimator.GetInteger("Action") == 1)
            {
				//Actions.Action01.JumpBall.enabled = false;
                if (col.transform.parent.GetComponent<EnemyHealth>() != null)
                {
                    if (!Player.isRolling)
                    {

                        Vector3 newSpeed = new Vector3(1, 0, 1);

						if ((Actions.Action == 1 || Actions.Action == 0) && CanHitAgain)
						{
							StartCoroutine (ResetTriggerBool ());

							////Debug.Log ("AfterJumping");
							newSpeed = new Vector3(Player.rigidbody.velocity.x, 0, Player.rigidbody.velocity.z);
							newSpeed.y = BouncingPower + Mathf.Abs (Player.rigidbody.velocity.y);
							////Debug.Log (newSpeed);
							Player.rigidbody.velocity = newSpeed;
						}

						else if ((Actions.Action == 2 || Actions.PreviousAction == 2) && !StopOnHit && CanHitAgain)
                        {
							StartCoroutine (ResetTriggerBool ());

							//Debug.Log ("AfterHoming");
							//Vector3 Direction = col.transform.position - Player.transform.position;
							newSpeed = new Vector3(Player.rigidbody.velocity.x*(1/EnemyHomingStoppingPowerWhenAdditive), HomingBouncingPower, Player.rigidbody.velocity.z*(1/EnemyHomingStoppingPowerWhenAdditive));
							////Debug.Log (newSpeed);
							Player.rigidbody.velocity = newSpeed;
                        }
						else if(StopOnHit)
						{
							//Debug.Log ("AfterHomingStop");
							newSpeed = new Vector3(0, 0, 0);
							newSpeed = Vector3.Scale(Player.rigidbody.velocity, newSpeed);
							newSpeed.y = HomingBouncingPower;
							Player.rigidbody.velocity = newSpeed;
						}

						if (Actions.Action == 6 && CanHitAgain)
						{
							StartCoroutine (ResetTriggerBool ());
						//	//Debug.Log ("AfterBouncing");
							newSpeed = new Vector3(1, 0, 1);
							////Debug.Log ("AfterHoming");
							newSpeed = Vector3.Scale(Player.rigidbody.velocity, newSpeed);
							newSpeed.y = HomingBouncingPower*2;
							Player.rigidbody.velocity = newSpeed;

							Actions.Action01.JumpBall.SetActive(false);
							if (Actions.Action08 != null) {
								if (Actions.Action08.DropEffect.isPlaying == true) {
									Actions.Action08.DropEffect.Stop ();
								}
							}
						}
                        

							
                        
                    }
                    col.transform.parent.GetComponent<EnemyHealth>().DealDamage(1);
                    updateTargets = true;
					Actions.Action01.JumpBall.SetActive(false);
					if (Actions.Action08 != null) {
						if (Actions.Action08.DropEffect.isPlaying == true) {
							Actions.Action08.DropEffect.Stop ();
						}
					}
                    Actions.ChangeAction(0);
                    Score += 1000;

                }
            }


            else if(Actions.Action != 3)
            {
                DamagePlayer();
            }
        }

		//Monitors
		if (col.tag == "Monitor")
		{
			if (Actions.Action00.CharacterAnimator.GetInteger("Action") == 1)
			{
					if (!Player.isRolling)
					{

						Vector3 newSpeed = new Vector3(1, 0, 1);

						if ((Actions.Action == 1 || Actions.Action == 0) && CanHitAgain)
						{
							StartCoroutine (ResetTriggerBool ());

							////Debug.Log ("AfterJumping");
							newSpeed = new Vector3(Player.rigidbody.velocity.x, 0, Player.rigidbody.velocity.z);
							newSpeed.y = BouncingPower + Mathf.Abs (Player.rigidbody.velocity.y);
							////Debug.Log (newSpeed);
							Player.rigidbody.velocity = newSpeed;
						}

						else if ((Actions.Action == 2 || Actions.PreviousAction == 2) && !StopOnHit && CanHitAgain)
						{
							StartCoroutine (ResetTriggerBool ());

							//Debug.Log ("AfterHoming");
							//Vector3 Direction = col.transform.position - Player.transform.position;
							newSpeed = new Vector3(Player.rigidbody.velocity.x*(1/EnemyHomingStoppingPowerWhenAdditive), HomingBouncingPower, Player.rigidbody.velocity.z*(1/EnemyHomingStoppingPowerWhenAdditive));
							////Debug.Log (newSpeed);
							Player.rigidbody.velocity = newSpeed;
						}
						else if(StopOnHit)
						{
							//Debug.Log ("AfterHomingStop");
							newSpeed = new Vector3(0, 0, 0);
							newSpeed = Vector3.Scale(Player.rigidbody.velocity, newSpeed);
							newSpeed.y = HomingBouncingPower;
							Player.rigidbody.velocity = newSpeed;
						}

						if (Actions.Action == 6 && CanHitAgain)
						{
							StartCoroutine (ResetTriggerBool ());
							//	//Debug.Log ("AfterBouncing");
							newSpeed = new Vector3(1, 0, 1);
							////Debug.Log ("AfterHoming");
							newSpeed = Vector3.Scale(Player.rigidbody.velocity, newSpeed);
							newSpeed.y = HomingBouncingPower*2;
							Player.rigidbody.velocity = newSpeed;

							Actions.Action01.JumpBall.SetActive(false);
							if (Actions.Action08 != null) {
								if (Actions.Action08.DropEffect.isPlaying == true) {
									Actions.Action08.DropEffect.Stop ();
								}
							}
						}




					}
					updateTargets = true;
					Actions.Action01.JumpBall.SetActive(false);
					if (Actions.Action08 != null) {
						if (Actions.Action08.DropEffect.isPlaying == true) {
							Actions.Action08.DropEffect.Stop ();
						}
					}
					Actions.ChangeAction(0);
			}
			
		}


        //Spring Collision

        if (col.tag == "Spring")
        {
            Player.KeepNormalCounter = 16;
            lockedOnAir = true;
			Actions.Action01.JumpBall.SetActive(false);
			if (Actions.Action08 != null) {
				if (Actions.Action08.DropEffect.isPlaying == true) {
					Actions.Action08.DropEffect.Stop ();
				}
			}


            if (col.GetComponent<Spring_Proprieties>() != null)
            {
                if (col.GetComponent<Spring_Proprieties>().LockControl)
                {
                    Inp.LockInputForAWhile(col.GetComponent<Spring_Proprieties>().LockTime, false);
                    lockedSpeed = true;
                    lockedSpeedValue = col.GetComponent<Spring_Proprieties>().transform.up * col.GetComponent<Spring_Proprieties>().SpringForce;
                }
                else
                {
                    Actions.Action00.CharacterAnimator.SetInteger("Action", 0);
                }

                spring = col.GetComponent<Spring_Proprieties>();
                if (spring.IsAdditive)
                {
                    transform.position = col.transform.GetChild(0).position;
                    if (col.GetComponent<AudioSource>()) { col.GetComponent<AudioSource>().Play(); }
                    if (Actions.Action02 != null) 
                    {
			            Actions.Action02.HomingAvailable = true;
		            }
                    Player.rigidbody.velocity += (spring.transform.up * spring.SpringForce);
                    Actions.ChangeAction(0);
                    spring.anim.SetTrigger("Hit");
                }
                else
                {
                    transform.position = col.transform.GetChild(0).position;
                    if (col.GetComponent<AudioSource>()) { col.GetComponent<AudioSource>().Play(); }
                    if (Actions.Action02 != null) {
						Actions.Action02.HomingAvailable = true;
					}

                    if (spring.AnimationType == AnimationType.Basic)
                    {
                        Actions.Action00.CharacterAnimator.SetInteger("Action", 0);
                    }
                    else if (spring.AnimationType == AnimationType.Dynamic)
                    {
                        Actions.Action00.CharacterAnimator.SetInteger("Action", -1);
                    }
                    else if (spring.AnimationType == AnimationType.Rolling)
                    {
                        Actions.Action00.CharacterAnimator.SetInteger("Action", 1);
                        Actions.Action00.CharacterAnimator.SetBool("isRolling", true);
                    }

                    Player.rigidbody.velocity = spring.transform.up * spring.SpringForce;
                    Actions.ChangeAction(0);
                    spring.anim.SetTrigger("Hit");
                }

            }
        }

		if (col.tag == "Bumper")
		{
			Actions.Action01.JumpBall.SetActive(false);
			if (Actions.Action08 != null) {
				if (Actions.Action08.DropEffect.isPlaying == true) {
					Actions.Action08.DropEffect.Stop ();
				}
			}


			if (col.GetComponent<Spring_Proprieties>() != null)
			{
				spring = col.GetComponent<Spring_Proprieties>();
				if (spring.IsAdditive)
				{
				//	transform.position = col.transform.GetChild(0).position;
					if (col.GetComponent<AudioSource>()) { col.GetComponent<AudioSource>().Play(); }
					Actions.Action00.CharacterAnimator.SetInteger("Action", 0);
					if (Actions.Action02 != null) {
						Actions.Action02.HomingAvailable = true;
					}
					Player.rigidbody.velocity += (Player.transform.position-spring.transform.position) * spring.SpringForce;

				}
				else
				{
					//transform.position = col.transform.GetChild(0).position;
					if (col.GetComponent<AudioSource>()) { col.GetComponent<AudioSource>().Play(); }
					Actions.Action00.CharacterAnimator.SetInteger("Action", 0);
					if (Actions.Action02 != null) {
						Actions.Action02.HomingAvailable = true;
					}
					Player.rigidbody.velocity = (Player.transform.position-spring.transform.position) * spring.SpringForce;
				
				}

				if (col.GetComponent<Spring_Proprieties>().LockControl)
				{
					Inp.LockInputForAWhile(col.GetComponent<Spring_Proprieties>().LockTime, false);
				}
			}
		}

		//Monitors
		if (col.tag == "CancelHoming") 
		{
			if (Actions.Action == 2) {

				Vector3 newSpeed = new Vector3(1, 0, 1);

				Actions.ChangeAction (0);
				newSpeed = new Vector3(0, HomingBouncingPower, 0);
				////Debug.Log (newSpeed);
				Player.rigidbody.velocity = newSpeed;
				//Player.transform.position = col.ClosestPoint (Player.transform.position);
				if (Actions.Action02 != null) {
					Actions.Action02.HomingAvailable = true;
				}
			}
		}

        if (col.tag == "2DTrigger")
        {
            PlayerSkin.rotation = col.transform.rotation;
        }

        if (col.tag == "GravitySet")
        {
            Player.Gravity = -col.transform.up * Player.Gravity.magnitude;
        }
    }

    public void OnTriggerStay(Collider col)
    {
        //Hazard
        if (col.tag == "Hazard")
        {
            DamagePlayer();
        }

        if (col.gameObject.tag == "MovingPlatform")
        {
            Platform = col.gameObject.GetComponent<MovingPlatformControl>();
        }
        else
        {
            Platform = null;
        }

        if (col.tag == "2DTrigger")
        {
            Mode2D = true;
            Cam.Locked = true;
            Cam.RotateDirection(col.transform.right, 2, 10);
            Cam.CameraMaxDistance = -30;
        }

        if (col.tag == "StageSelector")
        {
            Cam.RotateDirection(-col.transform.forward, 8, 0);
            if (PauseCotrol.StageSelector.activeSelf)
            {
                Inp.LockInput = true;
            }
            if (PauseCotrol.play && playTime < 2)
            {
                
                playTime += 1;
                col.GetComponent<StageSelect>().SetStage(col.GetComponent<StageSelect>().CurrentStage);
                if (playTime == 1)
                {
                    col.GetComponent<StageSelect>().LoadStage();
                }
            }
        }
        if (col.tag == "Water")
        {
            Player.Gravity = Vector3.up * -0.5f;
            Player.MoveAccell = 0.1f;
        }

    }

    private void OnTriggerExit(Collider col)
    {
        if (col.tag == "2DTrigger")
        {
            Mode2D = false;
            Cam.Locked = false;
            Cam.CameraMaxDistance = CamInitialDistance;
        }
        if (col.tag == "StageSelector")
        {
            PauseCotrol.colTime = 0;
        }
        if (col.tag == "Water")
        {
            Player.Gravity = InitialGravity;
            Player.MoveAccell = InitialAcell;
        }
    }

    private IEnumerator IncreaseRing()
	{
		int ThisFramesRingCount = RingAmount;
		RingAmount++;
        Score += 100;
		yield return new WaitForEndOfFrame ();
		if (RingAmount > ThisFramesRingCount + 1) 
		{
			RingAmount--;
		}
			
	}

    public void DamagePlayer()
    {
        if (!Actions.Action04Control.IsHurt && Actions.Action != 4)
        {

            if (!Monitors_Interactions.HasShield)
            {
                if (RingAmount > 0)
                {
                    //LoseRings
                    Sounds.RingLossSound();
                    Actions.Action04Control.GetHurt();
                    Actions.ChangeAction(4);
                    Actions.Action04.InitialEvents();
                }
                if (RingAmount <= 0)
                {
                    //Die
                    if (!Actions.Action04Control.isDead)
                    {
                        Sounds.DieSound();
                        Actions.Action04Control.isDead = true;
                        Actions.ChangeAction(4);
                        Actions.Action04.InitialEvents();
                    }
                }
            }
            if (Monitors_Interactions.HasShield)
            {
                //Lose Shield
                Actions.Action04.sounds.SpikedSound();
                Monitors_Interactions.HasShield = false;
                Actions.ChangeAction(4);
                Actions.Action04.InitialEvents();
            }
        }
    }
}
