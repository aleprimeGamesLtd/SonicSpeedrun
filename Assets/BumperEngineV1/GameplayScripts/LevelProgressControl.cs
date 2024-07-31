using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;
using System.IO;
using System;

public class LevelProgressControl : MonoBehaviour {

    public Animator CharacterAnimator;
    public Vector3 ResumePosition { get; set; }
    public Quaternion ResumeRotation { get; set; }
    public Transform ResumeTransform;
    public Transform Skin;
    ActionManager Actions;
    PlayerBhysics Player;
    CameraControl Cam;
    public GameObject CurrentCheckPoint { get; set; }

    public Material LampDone;
    public int LevelToGoNext = 0;
    public string NextLevelNameLeft;
    public string NextLevelNameRight;
    public AudioClip GoalRingTouchingSound;

    public bool readyForNextStage = false;
    float readyCount = 0;

    bool firstime = false;
    [SerializeField]public ProgressData progressDataClass;

    void Start () {

        Cam = GetComponent<CameraControl>();
        Actions = GetComponent<ActionManager>();
        Player = GetComponent<PlayerBhysics>();
        CharacterAnimator.SetBool("Completed", false);

        if (!File.Exists("Saves/CurrentProgress.json")){
            File.Create("Saves/CurrentProgress.json");
        }
            
        Transform position = GameObject.Find("CharacterSpawner").transform;
        string file = File.ReadAllText("Saves/CurrentProgress.json");
        ProgressData progress = JsonUtility.FromJson<ProgressData>(file);

        firstime = !progress.firstime;

        if (!firstime){
            progressDataClass.firstime = true;
            progressDataClass.posX = position.position.x;
            progressDataClass.posY = position.position.y;
            progressDataClass.posZ = position.position.z;
        
            progressDataClass.rotX = position.eulerAngles.x;
            progressDataClass.rotY = position.eulerAngles.y;
            progressDataClass.rotZ = position.eulerAngles.z;
            
            string progressData = JsonUtility.ToJson(progressDataClass);
            File.WriteAllText("Saves/CurrentProgress.json", progressData);
            Debug.Log(progressData);
        }
        else
        {
            transform.position = new Vector3(progress.posX, progress.posY, progress.posZ);
            transform.rotation = Quaternion.Euler(progress.rotX, progress.rotY, progress.rotZ);
        }
        
    }

    void Update()
    {
        //LampDone.SetTextureOffset("_MainTex", new Vector2(0, -Time.time) * 3);
        //LampDone.SetTextureOffset("_EmissionMap", new Vector2(0, -Time.time) * 3);

        if (readyForNextStage)
        {
            Player.MoveInput = Vector3.zero;
            Player.rigidbody.velocity = new Vector3(0, Player.rigidbody.velocity.y, 0);
            transform.position = new Vector3(Cam.Cam.transform.position.x, transform.position.y, Cam.Cam.transform.position.z) + Cam.Cam.transform.rotation * new Vector3(-3, 0, 8);
            Cam.Cam.transform.position = new Vector3(Cam.Cam.transform.position.x, transform.position.y + 2.4f, Cam.Cam.transform.position.z);
            Skin.rotation = Quaternion.Euler(transform.eulerAngles.x, 150 + Cam.Cam.transform.eulerAngles.y, transform.eulerAngles.z);
            readyCount += Time.deltaTime;
            if(readyCount > 1.5f)
            {
                Actions.Action04Control.enabled = false;
                //Color alpha = Color.black;
                //Actions.Action04Control.FadeOutImage.color = Color.Lerp(Actions.Action04Control.FadeOutImage.color, alpha, Time.fixedTime * 0.1f);
            }
            if(readyCount > 2.6f)
            {
                LoadingScreenControl.StageName1 = NextLevelNameLeft;
                LoadingScreenControl.StageName2 = NextLevelNameRight;
                //SceneManager.LoadScene(2);
            }
        }
    }

    void LateUpdate()
    {
        if (!firstime)
        {
            ResumePosition = transform.position;
            ResumeRotation = transform.rotation;
            firstime = true;
        }
    }

    public void ResetToCheckPoint()
    {
        //Debug.Log("Reset");

		Objects_Interaction.RingAmount = 0;

		if (Monitors_Interactions.HasShield) 
		{
		 	Monitors_Interactions.HasShield = false;
		}
        SceneManager.LoadScene("LoadingScreen");
    }

    public void SetCheckPoint(Transform position)
    {
        progressDataClass.firstime = false;

        progressDataClass.posX = position.position.x;
        progressDataClass.posY = position.position.y;
        progressDataClass.posZ = position.position.z;
        
        progressDataClass.rotX = position.eulerAngles.x;
        progressDataClass.rotY = position.eulerAngles.y;
        progressDataClass.rotZ = position.eulerAngles.z;

        string progressData = JsonUtility.ToJson(progressDataClass);
        File.WriteAllText("Saves/CurrentProgress.json", progressData);
        Debug.Log(progressData);
    }

    public void OnTriggerEnter(Collider col)
    {
        if(col.tag == "Checkpoint")
        {
            if (col.GetComponent<CheckPointData>() != null)
            {
                //Set Object
                if (!col.GetComponent<CheckPointData>().IsOn)
                {
                    col.GetComponent<CheckPointData>().IsOn = true;
                    col.GetComponent<CheckPointData>().Renderer.material = LampDone;
                    col.GetComponent<AudioSource>().Play();
                    SetCheckPoint(col.GetComponent<CheckPointData>().CheckPos);
                    CurrentCheckPoint = col.gameObject;
                    ResumeRotation = col.transform.rotation;
                }
            }
        }
        if (col.tag == "GoalRing")
        {
            readyForNextStage = true;

			if (Monitors_Interactions.HasShield) 
			{
				Monitors_Interactions.HasShield = false;
            }
            
            progressDataClass.firstime = true;
            string progressData = JsonUtility.ToJson(progressDataClass);
            File.WriteAllText("Saves/CurrentProgress.json", progressData);
            Debug.Log(progressData);

            transform.rotation = transform.rotation;
            Cam.Cam.transform.position = new Vector3(col.transform.position.x, transform.position.y + 2.4f, col.transform.position.z);
            Cam.Cam.transform.rotation = Quaternion.Euler(transform.eulerAngles.x, col.transform.eulerAngles.y, transform.eulerAngles.z);
            GetComponent<Objects_Interaction>().enabled = false;
            GetComponent<HurtControl>().enabled = false;
            GetComponent<PlayerBinput>().enabled = false;
            GetComponent<Action02_Homing>().enabled = false;
            GetComponent<Action03_SpinDash>().enabled = false;
            GetComponent<Action04_Hurt>().enabled = false;
            Cam.Cam.GetComponent<Camera>().fieldOfView = Cam.Cam.InitialFieldOfView;
            Cam.Cam.enabled = false;
            col.GetComponent<AudioSource>().clip = GoalRingTouchingSound;
            col.GetComponent<AudioSource>().loop = false;
            col.GetComponent<AudioSource>().Play();
            col.transform.localScale = Vector3.zero;
            CharacterAnimator.SetBool("Completed", true);
            if (GameObject.Find("[StageMusic]")) { GameObject.Find("[StageMusic]").SetActive(false); }
            if (GameObject.Find("UI")) { GameObject.Find("UI").SetActive(false); }
            if (!SceneManager.GetSceneByName("StageCompleteScreen").isLoaded)
            {
                CharacterAnimator.SetBool("Completed", true);
                col.GetComponent<SphereCollider>().enabled = false;
                SceneManager.LoadScene("StageCompleteScreen", LoadSceneMode.Additive);
            }
            
		//	StageConpleteControl.LevelToGoNext = SceneManager.GetActiveScene ().buildIndex + 1;
        }
    }
}
