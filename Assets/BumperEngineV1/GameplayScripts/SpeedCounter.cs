using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SpeedCounter : MonoBehaviour
{
    public Image fill;
    public Text SpeedAmmount;
    PlayerBhysics Player;

    void Update()
    {
        Player = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerBhysics>();

        if (SpeedAmmount != null)
        {
            SpeedAmmount.text = Player.Actions.Action00.interactions.boostAmount.ToString("F0") + " %";
        }
        if (fill != null)
        {
            fill.fillAmount = Player.Actions.Action00.interactions.boostAmount / 100;
        }
    }
}
