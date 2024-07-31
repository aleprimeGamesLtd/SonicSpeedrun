using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

public class SpawnCharacter : MonoBehaviour {

	public GameObject PlayerObject;

	// Use this for initialization
	void Awake () {
		if (GameObject.Find ("CharacterSelector") != null) {
			PlayerObject = GameObject.Find ("CharacterSelector").GetComponent<CharacterSelect>().DesiredCharacter;
		}
		GameObject Player = Instantiate(PlayerObject, transform.position, Quaternion.Euler(Vector3.zero));
        if (GameObject.Find ("CharacterSelector") != null) {
			Destroy (GameObject.Find ("CharacterSelector"));
		}
	}

}
