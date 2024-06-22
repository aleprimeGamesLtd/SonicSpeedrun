using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.PostProcessing;
using System.IO;

public class Qualitysettings : MonoBehaviour {

    public PostProcessVolume postProcessing;

	void Start () {

        string graphicsSettings = File.ReadAllText("Saves/GraphicsSettings.json");
        GraphicsData graphicsData = JsonUtility.FromJson<GraphicsData>(graphicsSettings);
        QualitySettings.SetQualityLevel(graphicsData.GraphicsSettings);

        SetQuality();

    }

    public void SetQuality()
    {
        int QS = QualitySettings.GetQualityLevel();

        switch (QS)
        {
            case 0:
                postProcessing.enabled = true;
                break;
            case 1:
                postProcessing.enabled = false;
                break;
            case 2:
                postProcessing.enabled = false;
                break;
        }
    }
	

}
