using UnityEngine;
using System.Collections;

public class Rail_Interaction : MonoBehaviour {

    public Rail rail { get; set; }
    PlayerBhysics Player;
    ActionManager Actions;
    public Animator CharacterAnimator;

    public int currentSeg { get; set; }
    float transition = 1;
    bool isCompleted;
    Quaternion CharRot;
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
        railActiveCount += 1;

        if (rail != null)
        {

            //Get Out of rail
            if(Actions.Action == 1)
            {
                rail = null;
                Player.rigidbody.velocity = 
                    (CharacterAnimator.transform.up * Actions.Action01.JumpSpeed * 4) +
                    (CharacterAnimator.transform.forward * Speed.magnitude);
            }
            else{
                OnRail(Speed.magnitude);
            }

        }
    }

    void Update()
    {
        if (rail != null)
        {
            if (Input.GetButton("A") && railActiveCount > 10)
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
            rail = null;
            return;
        }

        currentSeg += ((int)speed / 50) * (int)transition;

        Vector3 CharRot = rail.LinearRotation(currentSeg);

        transform.position = rail.LinearPosition(currentSeg);
        CharacterAnimator.SetInteger("Action", 5);
        CharacterAnimator.transform.rotation = Quaternion.Euler(CharRot);
        transform.rotation = Quaternion.Euler(CharRot);
        Player.rigidbody.velocity = Vector3.zero;
    }

    public void OnTriggerEnter(Collider col)
    {
        if(col.gameObject.tag == "Rail")
        {
            if(col.gameObject.transform.parent.GetComponent<Rail>() != null)
            {
                if (railActiveCount > 3 && rail == null)
                {
                    Speed = Player.rigidbody.velocity;
                    rail = col.gameObject.transform.parent.GetComponent<Rail>();
                    currentSeg = GetClosestPos(col.gameObject.transform.parent.GetComponent<Rail>().RailArray, transform.position);
                    
                    if (Actions.Action != 5)
                    {
                        Actions.Action01.JumpBall.SetActive(false);
                        Actions.ChangeAction(5);
                        railActiveCount = 0;
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
