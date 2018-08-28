using UnityEngine;
using System.Collections;

public class TRAMPOLINE : MonoBehaviour {

	private float MinBounceBooster = 1100f;
	private float MaxBounceBooster = 3000f;
	private float BounceBooster;

	private GameObject TheController;

	public int BonusAmount = 500;

	public AudioClip BounceSound;
	public AudioSource SoundPlayer;
	private float LastBounceSound;


	void Start() {
		TheController = GameObject.Find("GAME-CONTROLLER");

		// set bounce amount
		BounceBooster = map(transform.position.y, GameObject.Find("Bottom Boundary").transform.position.y, GameObject.Find("Top Boundary").transform.position.y, MaxBounceBooster, MinBounceBooster);
	}

	void OnCollisionEnter(Collision coll) {

		// otherwise he gets stuck...
		GetComponent<BoxCollider> ().enabled = false;

		// only add bounce or make sound once at a time

		if (Time.time > LastBounceSound + BounceSound.length) {
			SoundPlayer.PlayOneShot (BounceSound);

			//TheController.SendMessage ("addBonus", BonusAmount);
			TheController.GetComponent<GuiController> ().addBonus (BonusAmount);

			// add upward force
			coll.rigidbody.AddForce (transform.up * BounceBooster);

			// and slight forward motion for good measure
			coll.rigidbody.AddForce (transform.forward * BounceBooster * -.15f);

			LastBounceSound = Time.time;

			// reset starting position so camera angle updates from this position?
			//Debug.Log("resetting camera angle");
			GameObject.Find ("Main Camera").GetComponent<BankerCam> ().setAngle(transform.position);
		
			// start trampoline disapear
			Destroy(this.gameObject, 1f);
		}
	}

	float map(float s, float a1, float a2, float b1, float b2) {
		return b1 + (s-a1)*(b2-b1)/(a2-a1);
	}
}