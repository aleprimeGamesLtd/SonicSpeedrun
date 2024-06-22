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
            SpeedAmmount.text = Player.rigidbody.velocity.magnitude.ToString("F0") + " m/s";
        }
        if (fill != null)
        {
            fill.fillAmount = Player.rigidbody.velocity.magnitude / Player.MaxSpeed;
        }
    }
}
