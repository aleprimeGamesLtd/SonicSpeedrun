using UnityEngine;
using System.Collections;

public class SonicEffectsControl : MonoBehaviour {

    public PlayerBhysics Player;
    public ParticleSystem RunningDust;
	public ParticleSystem SpeedLines;
    public ParticleSystem SpinDashDust;
    public float RunningDustThreshold;
	public float SpeedLinesThreshold;

	void FixedUpdate () {
	
		if(Player.rigidbody.velocity.sqrMagnitude > RunningDustThreshold && Player.Grounded && RunningDust != null)
        {
            RunningDust.Emit(Random.Range(0,20));
        }

		if (Player.Actions.Action00.interactions.boosting && SpeedLines != null && SpeedLines.isPlaying == false) 
		{
			SpeedLines.Play ();
		} 
		else if (!Player.Actions.Action00.interactions.boosting && SpeedLines.isPlaying == true) 
		{
			SpeedLines.Stop ();
		}

	}
    public void DoSpindashDust(int amm, float speed)
    {
        SpinDashDust.startSpeed = speed;
        SpinDashDust.Emit(amm);
    }

}
