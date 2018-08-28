using UnityEngine;
using System.Collections;

public class Gravity : MonoBehaviour {

	public Vector3 gravity = new Vector3 (0, -2, 0);

	void Awake () {
		Physics.gravity = gravity;
	}
}
