using UnityEngine;
using System.Collections;

public class Cloud : MonoBehaviour {

	void Start () {
		// random rotation on start
		int rando = Random.Range(0,360);
		transform.rotation = Quaternion.Euler(0, rando, 0);
	}
}