using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class volume_fadein : MonoBehaviour {

    public AudioSource audio;

    public float endVolume = .75f;
    public float increaseBy = .1f;

	// Use this for initialization
	void Start () {
        audio = GetComponent<AudioSource>();
    }
	
	// Update is called once per frame
	void Update () {
        if(audio.volume < endVolume) {
            audio.volume += increaseBy;
        } else {
            Destroy(this);
        }
	}
}
