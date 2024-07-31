using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

public class FileScreen : MonoBehaviour
{
    float delay;

    void Start()
    {
        if (!Directory.Exists("Saves"))
        {
            Directory.CreateDirectory("Saves");
        }

        if (!File.Exists("Saves/CurrentProgress.json"))
        {
            File.Create("Saves/CurrentProgress.json");
        }

        if (!File.Exists("Saves/GraphicsSettings.json"))
        {
            File.Create("Saves/GraphicsSettings.json");
        }

        if (!File.Exists("Saves/CurrentStage.txt"))
        {
            File.Create("Saves/CurrentStage.txt");
        }
    }

    void Update()
    {
        ProgressData progressData = new ProgressData();
        if (File.Exists("Saves/CurrentProgress.json"))
        {
            string newProgressData = JsonUtility.ToJson(progressData);
            File.WriteAllText("Saves/CurrentProgress.json", newProgressData);
        }

        GraphicsData graphicsData = new GraphicsData();
        if (File.Exists("Saves/GraphicsSettings.json"))
        {
            if (JsonUtility.FromJson<GraphicsData>(File.ReadAllText("Saves/GraphicsSettings.json")) == null)
            {
                File.WriteAllText("Saves/GraphicsSettings.json", JsonUtility.ToJson(graphicsData));
            }

            if (JsonUtility.FromJson<GraphicsData>(File.ReadAllText("Saves/GraphicsSettings.json")).GraphicsSettings == -1)
            {
                graphicsData.GraphicsSettings = QualitySettings.GetQualityLevel();
                string newGraphicsData = JsonUtility.ToJson(graphicsData);
                File.WriteAllText("Saves/GraphicsSettings.json", newGraphicsData);
            }
        }

        if (File.Exists("Saves/CurrentStage.txt"))
        {
            File.WriteAllText("Saves/CurrentStage.txt", "LogoScreen");
        }
    }
}
