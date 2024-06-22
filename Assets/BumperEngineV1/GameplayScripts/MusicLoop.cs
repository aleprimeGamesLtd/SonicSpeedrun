using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MusicLoop : MonoBehaviour {
    AudioSource audioSource;
    public bool loop;
    public float loopStart;
    public float loopEnd;

	void Start () { audioSource = GetComponent<AudioSource>(); }
	
	void Update ()
    {
		if (loop)
        {
            audioSource.loop = true;
            if (audioSource.time >= loopEnd) { audioSource.time = loopStart; }
        }
	}
}
