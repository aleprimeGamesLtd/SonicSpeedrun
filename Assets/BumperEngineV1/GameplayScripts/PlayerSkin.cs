using UnityEngine;
using System.Collections;
using System.IO;

public class PlayerSkin : MonoBehaviour {

    public Animator CharacterAnimator;
    float delay;
    ProgressData progress;

    void Update()
    {
        delay++;
        if (delay <= 8)
        {
            progress = JsonUtility.FromJson<ProgressData>(File.ReadAllText("Saves/CurrentProgress.json"));
            Vector3 Rot = new Vector3(progress.rotX, progress.rotY, progress.rotZ);
            CharacterAnimator.transform.rotation = Quaternion.Euler(Rot);
        }
    }

}
