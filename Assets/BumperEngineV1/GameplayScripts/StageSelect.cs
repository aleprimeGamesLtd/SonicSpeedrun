using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;
using System.IO;
public class StageSelect : MonoBehaviour {

	[SerializeField] public string CurrentStage;
	[SerializeField] TMP_Text InfoText;
	[SerializeField] List<GameObject> ObjectsToActivateOnGo;

	void Update()
	{
		if (!File.Exists("Saves/CurrentStage.txt"))
		{
			if (!Directory.Exists("Saves"))
			{
				Directory.CreateDirectory("Saves");
			}
			else
			{
				File.Create("Saves/CurrentStage.txt");
			}
		}
	}

	public void SetStage(string StageName)
	{
		if (File.Exists("Saves/CurrentStage.txt"))
		{
			File.WriteAllText("Saves/CurrentStage.txt", StageName);
		}
        
	}

	public void InfoTextUpdate(Text Info)
	{
		InfoText.text = Info.text;
	}

	public void LoadStage()
	{
		StartCoroutine (BeginLoad());

	}

	public void LoadPastScene()
	{
		if (GameObject.Find ("CharacterSelector") != null) {
			Destroy (GameObject.Find ("CharacterSelector"));
		}
		SceneManager.LoadScene ("LogoScreen");
	}

	private IEnumerator BeginLoad()
	{

		for (int i = 0; i < ObjectsToActivateOnGo.Count; i++) 
		{
			ObjectsToActivateOnGo[i].SetActive (true);
		}
		SceneManager.LoadSceneAsync ("LoadingScreen");

		yield return null;
	}
}
