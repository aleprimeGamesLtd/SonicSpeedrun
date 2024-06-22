using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.IO;

public class MainMenu : MonoBehaviour
{
    public Button[] mainButtons;
    public Button[] settingsButtons;
    public GameObject settingsMenu;
    public Text currentGraphics;
    public GameObject stagesMenu;

    void Start(){
        settingsMenu.SetActive(false);
        stagesMenu.SetActive(false);
        if (!File.Exists("Saves/GraphicsSettings.json")){
            File.Create("Saves/GraphicsSettings.json");
            GraphicsData graphics = new GraphicsData();
            graphics.GraphicsSettings = QualitySettings.GetQualityLevel();
            string json = JsonUtility.ToJson(graphics);
            if (File.ReadAllText("Saves/GraphicsSettings.json") != json)
            {
                File.WriteAllText("Saves/GraphicsSettings.json", json);
            }
        }
    }

    public void SettingsToggle(){
        settingsMenu.SetActive(!settingsMenu.activeSelf);
        if (!settingsMenu.activeSelf) {
            mainButtons[1].Select();
        }
        else {
            string graphicsSettings = File.ReadAllText("Saves/GraphicsSettings.json");
            GraphicsData graphics = JsonUtility.FromJson<GraphicsData>(graphicsSettings);

            switch (graphics.GraphicsSettings) {
                case 0:
                    currentGraphics.text = "Current Graphics Mode: High";
                    break;
                case 1:
                    currentGraphics.text = "Current Graphics Mode: Medium";
                    break;
                case 2:
                    currentGraphics.text = "Current Graphics Mode: Low";
                    break;
            }
            settingsButtons[graphics.GraphicsSettings].Select();
        }
        for (int i = 0; i < mainButtons.Length; i++){
            mainButtons[i].enabled = !mainButtons[i].enabled;
        }
    }

    public void PlayToggle(){
        stagesMenu.SetActive(!stagesMenu.activeSelf);
        if (!stagesMenu.activeSelf){
            mainButtons[0].Select();
        }
        else{
            GameObject.Find("MountainButton").GetComponent<Button>().Select();
        }
        for (int i = 0; i < mainButtons.Length; i++){
            mainButtons[i].enabled = !mainButtons[i].enabled;
        }
    }

    public void SetGraphicsSettings(int graphicsTierNum)
    {
        QualitySettings.SetQualityLevel(graphicsTierNum);
        GraphicsData graphicsData = new GraphicsData();
        graphicsData.GraphicsSettings = graphicsTierNum;
        string graphics = JsonUtility.ToJson(graphicsData);
        File.WriteAllText("Saves/GraphicsSettings.json", graphics);

        switch (graphicsTierNum)
        {
            case 0:
                currentGraphics.text = "Current Graphics Mode: High";
                break;
            case 1:
                currentGraphics.text = "Current Graphics Mode: Medium";
                break;
            case 2:
                currentGraphics.text = "Current Graphics Mode: Low";
                break;
        }
    }
}
