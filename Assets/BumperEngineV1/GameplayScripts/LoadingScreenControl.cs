using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.IO;

public class LoadingScreenControl : MonoBehaviour {

    int count;
    int endCount;
    bool over;

    public Animator Anim;
    public int LoadingStart;
    public Camera Cam;
    public string stage;

    int LevelToLoad;
    AsyncOperation levelLoader;

    public static bool StageLoaded;
    public static string StageName1;
    public static string StageName2;
    private string currentStage;

    public Text StageName1txt;
    public Text StageName2txt;

    void Start()
    {
        LevelToLoad = SceneController.LevelToLoad;
        StageName1txt.text = StageName1;
        StageName2txt.text = StageName2;
    }

    void FixedUpdate()
    {
        currentStage = File.ReadAllText("Saves/CurrentStage.txt");
        StageLoaded = SceneManager.GetSceneByName(currentStage).isLoaded;
        StageTitle();

        count += 1;
        if (count == LoadingStart)
        {
			SceneManager.LoadSceneAsync(currentStage, LoadSceneMode.Additive);
        }

        if(!over && StageLoaded)
        {
            over = true;
        }

        if(over)
        {
            endCount += 1;
            if(endCount == 10)
            {
                SceneManager.SetActiveScene(SceneManager.GetSceneByName(currentStage));
                if (Cam) { Destroy(Cam.gameObject); }
                Anim.SetInteger("Action", 1);
            }
    //        if(endCount > 60)
    //        {
				//SceneManager.UnloadSceneAsync(SceneManager.GetSceneByName("LoadingScreen"));
    //        }
        }

    }

    void StageTitle()
    {
        if (currentStage == "IntroZone") { StageName1txt.text = "Intro Zone"; StageName2txt.text = "Act 1"; }
        else if (currentStage == "DemoZone") { StageName1txt.text = "Demo Zone"; StageName2txt.text = "Act 1"; }
        else if (currentStage == "BlushCanyonZone") { StageName1txt.text = "Blush Canyon"; StageName2txt.text = "Act 1"; }
        else if (currentStage == "GreenHill1") { StageName1txt.text = "Green Hill"; StageName2txt.text = "Act 1"; }
        else if (currentStage == "CrisisCity") { StageName1txt.text = "Crisis City"; StageName2txt.text = "Act 1"; }
        else { StageName1txt.text = "Unknown"; StageName2txt.text = "Act 1"; }
    }
}
