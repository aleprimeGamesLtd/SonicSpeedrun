using UnityEngine;
using System.Collections;
using System;

public class Rail_Interaction : MonoBehaviour {

    public Rail rail { get; set; }
    PlayerBhysics Player;
    ActionManager Actions;
    public Animator CharacterAnimator;
    AudioSource Sound;

    public int currentSeg { get; set; }
    float transition = 1;
    bool backwards;
    bool isCompleted;
    Vector3 CharRot;
    Vector3[] currentRot;
    int railActiveCount;
    Vector3 Speed;

    void Awake()
    {
        Actions = GetComponent<ActionManager>();
        Player = GetComponent<PlayerBhysics>();
    }

    void FixedUpdate()
    {

        if (rail != null)
        {
            railActiveCount += 1;

            //Get Out of rail
            if(Actions.Action == 1)
            {
                if (Sound != null)
                {
                    Sound.Stop();
                }
                rail = null;
                Player.rigidbody.velocity =
                    (CharacterAnimator.transform.up * Actions.Action01.JumpSpeed * 4) +
                    (CharacterAnimator.transform.forward * Speed.magnitude);
            }
            else
            {
                OnRail(Speed.magnitude);
            }

        }
    }

    void Update()
    {
        if (rail != null)
        {
            if (Input.GetButtonDown("A"))
            {
                Actions.Action01.InitialEvents();
                Actions.ChangeAction(1);
                railActiveCount = 0;
            }
        }
    }

    void OnRail(float speed)
    {
        //Get out when over

        if (currentSeg > rail.RailArray.Length || currentSeg <= 0)
        {
            Actions.ChangeAction(0);
            CharacterAnimator.SetInteger("Action", 0);
            Speed = CharacterAnimator.transform.forward * speed;
            Player.rigidbody.velocity = Speed;
            railActiveCount = 0;
            if (Sound != null)
            {
                Sound.Stop();
            }
            rail = null;
            return;
        }
        else
        {
            if (!backwards)
            {
                currentSeg += ((int)speed / 25);
                CharacterAnimator.transform.rotation = Quaternion.Euler(CharRot);
            }
            else
            {
                currentSeg -= ((int)speed / 25);
                CharacterAnimator.transform.rotation = Quaternion.Euler(-CharRot.x, CharRot.y - 180, -CharRot.z);
            }

            CharRot = rail.LinearRotation(currentSeg);

            transform.position = rail.LinearPosition(currentSeg);
            CharacterAnimator.SetInteger("Action", 5);
            Actions.Action00.interactions.Cam.RotateDirection(CharacterAnimator.transform.forward, 2, Actions.Action00.interactions.Cam.HeightToLock);
            Player.rigidbody.velocity = Vector3.zero;
            Player.Grounded = true;
        }
    }

    public void OnTriggerEnter(Collider col)
    {
        Vector3 PrevCharRot = CharacterAnimator.transform.forward;

        if(col.gameObject.tag == "Rail")
        {
            if(col.gameObject.transform.parent.GetComponent<Rail>() != null)
            {
                if (rail == null)
                {
                    Sound = col.transform.parent.GetComponent<AudioSource>();
                    if (Sound != null && !Sound.isPlaying)
                    {
                        Sound.Play();
                        col.GetComponent<AudioSource>().Play();
                    }
                    Speed = Player.rigidbody.velocity;
                    rail = col.gameObject.transform.parent.GetComponent<Rail>();
                    currentSeg = GetClosestPos(col.gameObject.transform.parent.GetComponent<Rail>().RailArray, transform.position);

                    Debug.Log(Vector3.Angle(PrevCharRot, col.transform.forward));
                    if (Vector3.Angle(PrevCharRot, col.transform.forward) < 90)
                    {
                        backwards = false;
                    }
                    else
                    {
                        backwards = true;
                    }

                    if (Actions.Action != 5)
                    {
                        Actions.Action01.JumpBall.SetActive(false);
                        Actions.ChangeAction(5);
                        CharacterAnimator.SetBool("Grounded", true);
                    }
                }
            }
        }
    }

    public int GetClosestPos(Vector3[] pos, Vector3 playerPos)
    {
        int seg = 0;
        Vector3 tMin = Vector3.zero;
        float minDist = Mathf.Infinity;
        Vector3 currentPos = playerPos;
        for(int i = 0; i < pos.Length; i++)
        {
            float dist = Vector3.Distance(pos[i], currentPos);
            if (dist < minDist)
            {
                seg = i;
                minDist = dist;
            }
        }
        return seg;
    }

}
