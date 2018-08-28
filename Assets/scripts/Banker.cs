using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using UnityEngine.SceneManagement;

public class Banker : MonoBehaviour {

	// particle systems
	private GameObject MoneyTrail;
	public ParticleSystem MoneyExplosion;
	public GameObject Confetti;
	public GameObject PowerUpFX;
	public GameObject CloudPoof;

	// links to other game objs
	private GameObject TheController;
	private Transform FarBounds;
	private Transform NearBounds;
	private Transform LeftBounds;
	private Transform RightBounds;

	// launch details
	private float LaunchedAt;

	// speed, angles, etc.
	private float MinimumY = -12f;
	private float JailRestingDistance = 8.5f;	// how far banker can be from center of jail to trigger victory if speed is zero

	// sounds!
	public AudioSource WhooshPlayer; // basically just for whoosh
	public AudioSource FXPlayer;
	public AudioClip WhooshSound;
	public AudioClip WhooshWithPaperSound;
	public AudioClip[] Thuds;
	public AudioClip MoneyExplosionSound;
	private float LastThud;	// to prevent audio overlaps

	// end of round stuff
	private float SlowEnoughToJail = 2f;
	private float LastSafeCollision; // trampolines and such
	private bool RoundEndStarted = false;
	private float RoundEndStartedTime;
	private float RoundStartTimeBuffer = .5f;
	private float RoundEndTimeBuffer = 2.0f;
	private bool VictoryStarted = false;
	private bool ConfettiThrown = false;
	private float BackToMenuBuffer = 5f;	// after victory or defeat is certain, wait to load menu...

	// power ups
//	private int MagnetCount = 0;
//	[HideInInspector] public bool MagnetActive = false;
//	private float MagnetStartTime = 0f;
//	private float MagnetDuration = .8f;
	public int TrampolineCount = 1;
	public GameObject tramp;
	private float lastTrampolineSpawn;


	// movement
	private float TiltSpeed = 35f;

	// Use this for initialization
	void Start () {
		// link 'em up

		MoneyTrail = transform.Find("MONEY TRAIL").gameObject;
		TheController = GameObject.Find("GAME-CONTROLLER");
		FarBounds = GameObject.Find ("Far Boundary").transform;
		NearBounds = GameObject.Find ("Near Boundary").transform;
		LeftBounds = GameObject.Find ("Left Boundary").transform;
		RightBounds = GameObject.Find ("Right Boundary").transform;

		// where do you fall off?
		MinimumY = GameObject.Find ("Bottom Boundary").transform.position.y - 5;

		// set launch time
		LaunchedAt = Time.time;

		// start playin sound
		WhooshPlayer = GetComponent<AudioSource>();
		WhooshPlayer.clip = WhooshWithPaperSound;
		WhooshPlayer.Play();
		WhooshPlayer.loop = true;

		// activate powerup button
		GameObject.Find("powerup_bounce").GetComponent<Button>().Select();

	}
	
	// Update is called once per frame
	void FixedUpdate () {


		// check for speed and such
		Vector3 vel = GetComponent<Rigidbody>().velocity;
		float speed = vel.magnitude;


		// is the game hella fucked? as in its been in round for 10 minutes without movement?
//		if (Time.time > LaunchedAt + 1 && speed < 2) {
//			Debug.Log ("restarting from inaction");
//			SceneManager.LoadScene ("menu_arcade");
//		}



//		Debug.Log (vel.z);


		// influence banker movement, but only if it's not the end of the round
		// and a jail isnt near

		if (!RoundEndStarted && !JailNear()) {

			if (TheController.GetComponent<GuiController> ().UseMobileControls) {
				float clampedAcc = Mathf.Clamp (Input.acceleration.x, -.25f, .25f);
				GetComponent<Rigidbody> ().AddForce (clampedAcc * TiltSpeed * -1f, 0, 0);
			} else if (TheController.GetComponent<GuiController> ().UseArcadeControls) {
				
				float clampedAccX = Mathf.Clamp (Input.GetAxis ("Horizontal"), -.25f, .25f);
				float clampedAccY = Mathf.Clamp (Input.GetAxis ("Vertical"), -.25f, .25f);
				if (vel.z > -5 && Input.GetAxis ("Vertical") < 0f) {
					clampedAccY = 0f;
					//Debug.Log ("restricting backwards motion");
				}
				if (vel.z < -50 && Input.GetAxis ("Vertical") > 0f) {
					clampedAccY = 0f;
					//Debug.Log ("restricting forward motion");
				}

				GetComponent<Rigidbody> ().AddForce (clampedAccX * TiltSpeed * -1f, 0, clampedAccY * TiltSpeed * -1f);

			} else if (TheController.GetComponent<GuiController> ().UseWebControls) {
//				float mouseAccX = (Screen.width / 2) - Input.mousePosition.x;
//				mouseAccX = map (mouseAccX, -Screen.width / 2, Screen.width / 2, -.3f, .3f);
//
//				float mouseAccY = (Screen.height / 2) - Input.mousePosition.y;
//				mouseAccY = map (mouseAccY, -Screen.height / 2, Screen.height / 2, -.1f, .1f);
//
//				GetComponent<Rigidbody> ().AddForce (mouseAccX * TiltSpeed, 0, mouseAccY * TiltSpeed);

				float clampedAcc = Mathf.Clamp (Input.GetAxis ("Horizontal"), -.25f, .25f);
				//float clampedAccZ = Mathf.Clamp (Input.GetAxis ("Vertical"), -.25f, .25f);
				float clampedAccZ = 0;
				GetComponent<Rigidbody> ().AddForce (clampedAcc * TiltSpeed * -1f, 0, clampedAccZ * TiltSpeed / 2 * -1f);
			
			} else if (TheController.GetComponent<GuiController> ().UseOSCControls) {
				float clampedAcc = map (Input.mousePosition.x, 0, Screen.width, -.2f, .2f);
				GetComponent<Rigidbody> ().AddForce (clampedAcc * TiltSpeed * -1f, 0, 0);

			}
		}

		// sound
		// set audio volume to his speed for ease (peaks at about 20)
		WhooshPlayer.volume = speed / 20;

		// send height to score counter
		float heightOut = (transform.position.y / 100) * 10;

		heightOut = Mathf.Clamp (heightOut, 1, 10);

		TheController.GetComponent<GuiController> ().SetBankerHeight (heightOut);

		// check for round over, only after time buffer
		if(Time.time > LaunchedAt + RoundStartTimeBuffer) {

			// disable powerups if almost at the minimum Y...
//			if (transform.position.y < MinimumY + 8) {
//				TheController.GetComponent<GuiController> ().HidePowerups ();
//			}

			// was there a bad shot? went too low?
			if (transform.position.y < MinimumY) {
				// to prevent timer resets only do this once
				if (!RoundEndStarted) {

					Debug.Log ("bad shot, went too low");
					// stop counter etc
					TheController.GetComponent<GuiController> ().StopCounting ();
					// go yellow until we determine if they win or lose
					TheController.GetComponent<GuiController> ().ChangeColor(Color.yellow, true);

					// trigger end of round
					RoundEndStarted = true;
					RoundEndStartedTime = Time.time;

					// delete all objects
					foreach (GameObject o in Object.FindObjectsOfType<GameObject>()) {
						if (o.name == "Jail3(Clone)" || o.name == "Trampoline Powerup(Clone)") {
							Destroy(o);
						}
					}
					Destroy (TheController.GetComponent<GenerateStuff> ());

					// let the player know
					TheController.GetComponent<GuiController> ().ShowDefeat();


				}
			} else {

				// if they hit an obstacle but then continue to fall, reset conditions
				if (RoundEndStarted && speed > SlowEnoughToJail) {
					//Debug.Log ("reseting victory conditions");
					RoundEndStarted = false;
					VictoryStarted = false;
					TheController.GetComponent<GuiController>().ShowPowerups();
				}
			}


			// are we resting comfortably in jail?
			if (speed < SlowEnoughToJail) {

				if (JailNear ()) {

					// dont want this to trigger if we just hit trampoline
					if (Time.time > LastSafeCollision + .5) {

						if (!VictoryStarted) {
							RoundEndStarted = true;
							RoundEndStartedTime = Time.time;

							//Debug.Log ("victory started");
							VictoryStarted = true;

							// get rid of powerups
							TheController.GetComponent<GuiController>().HidePowerups();
						}
					}
				}
			}
		}


		// is game checking for win/loss?

		if (RoundEndStarted) {

			//Debug.Log ("round end started");

			// interruption?

			// check if we went enough time to be sure
			if (Time.time > RoundEndStartedTime + RoundEndTimeBuffer) {
				
				// great, now we're certain
				if (VictoryStarted) {

					// VICTORY
					TheController.GetComponent<GuiController> ().ChangeColor(Color.green, true);

					// throw some confetti
					if (!ConfettiThrown) {
						for (int i = 0; i < 10; i++) {	// randomly...
							Vector3 randomOffset = new Vector3 (Random.Range (-3f, 3f), Random.Range (2f, 4f), Random.Range (-3f, 3f));
							Instantiate (Confetti, transform.position + randomOffset, Confetti.transform.rotation);
						}

						TheController.GetComponent<GuiController> ().StopCounting ();

						Vector3 offset = new Vector3 (0, 2, 0);
						Instantiate(Confetti, transform.position + offset, Confetti.transform.rotation);
						TheController.GetComponent<GuiController> ().ShowVictory ();

						ConfettiThrown = true;
					}
				} else {
					
					// DEFEAT
					TheController.GetComponent<GuiController> ().ChangeColor(Color.red, false);
					TheController.GetComponent<GuiController> ().ShowDefeat ();

				}

				// whether won or lost, restart to menu
				if (Time.time > RoundEndStartedTime + RoundEndTimeBuffer + BackToMenuBuffer) {
					
					if (TheController.GetComponent<GuiController> ().UseArcadeControls) {
						// if in arcade mode, check for high score and such
						if(TheController.GetComponent<GuiController>().DoHighScoreScene)
							SceneManager.LoadScene ("highscore_arcade");
						else
							SceneManager.LoadScene ("menu_arcade");
					}
					else if (TheController.GetComponent<GuiController> ().UseWebControls)
						SceneManager.LoadScene("menu_web");
					else if (TheController.GetComponent<GuiController> ().UseMobileControls)
						SceneManager.LoadScene("menu");
					else if (TheController.GetComponent<GuiController> ().UseOSCControls)
						SceneManager.LoadScene("menu_arcade");
					
				}
			}
		}

		// generate more stuff if banker has gone far enough
		float zDifference = NearBounds.position.z - FarBounds.position.z;

		// is banker halfway between the two bounds?
		if (transform.position.z - (zDifference/2) < FarBounds.position.z) {
			
			//Debug.Log(transform.position.z);

			float oldFarBounds = FarBounds.position.z;

			FarBounds.position = new Vector3 (FarBounds.position.x, FarBounds.position.y, FarBounds.position.z - zDifference/2);

			TheController.GetComponent<GenerateStuff> ().GenStuff (transform.position.z, oldFarBounds, FarBounds.position.z, true);
		}
	}

	void Update() {
		if (TheController.GetComponent<GuiController> ().UseArcadeControls || TheController.GetComponent<GuiController> ().UseWebControls) {

			if (Input.GetButtonDown ("Fire1") && TrampolineCount > 0  && Time.time > lastTrampolineSpawn + 1f) {
				createTrampoline ();

				// prevent too many rapid spawns
				lastTrampolineSpawn = Time.time;
			}
		} else if (TheController.GetComponent<GuiController> ().UseOSCControls) {
			if (Input.GetMouseButtonDown (0) && TrampolineCount > 0) {
				createTrampoline ();
			}
		}
	}



	// ---- POWERUPS! ----

	public void createTrampoline() {

		//Debug.Log ("using trampoline");

		// Unity gets confused because the prefab is linked, so
		// it doesnt give the position on the instance, so...
		GameObject Banker = GameObject.Find("Banker(Clone)");

		// angle?
		float zRot = map (Banker.transform.position.x, Banker.GetComponent<Banker>().LeftBounds.position.x, Banker.GetComponent<Banker>().RightBounds.position.x, 45, -45);

		// use banker velocity to figure out where to place trampoline
		Vector3 BankerVelocity = Banker.GetComponent<Rigidbody>().velocity;

		// then make the new trampoline (old offset - new Vector3(0, 2, 5))
		Instantiate (tramp, Banker.transform.position + (BankerVelocity/2f), Quaternion.Euler (new Vector3 (355, 0, zRot)));
		Banker.GetComponent<Banker>().TrampolineCount--;
							
		// delete icon from view if none left
		if (Banker.GetComponent<Banker>().TrampolineCount <= 0) {
			if(GameObject.Find ("powerup_bounce") != null)
			GameObject.Find ("powerup_bounce").GetComponent<Button> ().interactable = false;
	
		}

		if (Banker.GetComponent<Banker>().TrampolineCount <= 1) {
			if(GameObject.Find ("powerup_bounce_count") != null)
			GameObject.Find ("powerup_bounce_count").GetComponent<Text> ().text = "";
		}

		if (Banker.GetComponent<Banker>().TrampolineCount > 1) {
			if(GameObject.Find ("powerup_bounce_count") != null)
			GameObject.Find ("powerup_bounce_count").GetComponent<Text> ().text = "x" + Banker.GetComponent<Banker>().TrampolineCount;
		}
	}

	void OnCollisionEnter(Collision collision) {

		// weed out early hits
		if (Time.time > LaunchedAt + RoundStartTimeBuffer) {

			// weed out trampolines: have a last time hit trampoline timer
			// ignore speed change collisions if within on second of that
			if(collision.gameObject.tag == "trampoline") {
				LastSafeCollision = Time.time;
				//Debug.Log("hit trampoline");

				// explode dollar bills
				DoMoneyExplosion(false);
				// give bonus
				TheController.GetComponent<GuiController> ().addBonus(250);

			} else {

				if(collision.gameObject.tag != "banker") {

					// he hit something solid, get rid of money trail
					DoMoneyExplosion(true);
					// give bonus
					TheController.GetComponent<GuiController> ().addBonus(500);

					// play hilarious thud, but try not to overlap sounds too much
					if(Time.time > LastThud + .3) {
						int rando = (int)Random.Range(0, Thuds.Length);
						FXPlayer.PlayOneShot(Thuds[rando], .25f);
						LastThud = Time.time;
					}

					//Debug.Log("regular collision");
				}
			}
		}
	}

	void OnTriggerExit(Collider collider) {

		// CLOUD POOF

		if (collider.name.StartsWith ("Cloud")) {
			Instantiate (CloudPoof, transform.position, Quaternion.identity);
			//Debug.Log ("cloud hit");
		}
	}

	void OnTriggerEnter(Collider collider) {

		// CLOUD POOF

		if (collider.name.StartsWith ("Cloud")) {
			Instantiate (CloudPoof, transform.position, Quaternion.identity);
			//Debug.Log ("cloud hit");
		}


		// COLLECT TRAMPOLINE POWERUP

		if (collider.name.StartsWith("Trampoline Powerup")) {
			Destroy (collider.gameObject);

			Object PFX = Instantiate (PowerUpFX, transform.position, transform.rotation);
			Destroy (PFX, 2);

			// text feedback
			TheController.GetComponent<GuiController>().ShowPowerup("SUPER\nTRAMP");

			// Unity gets confused because the prefab is linked, so
			// it doesnt give the position on the instance, so...
			GameObject Banker = GameObject.Find("Banker(Clone)");

			// show icon, etc
			Banker.GetComponent<Banker>().TrampolineCount++;
			//Debug.Log (TrampolineCount);

			if(GameObject.Find("powerup_bounce").GetComponent<Button>() != null)
				GameObject.Find("powerup_bounce").GetComponent<Button>().interactable = true;

			if (Banker.GetComponent<Banker>().TrampolineCount > 1) {
				GameObject.Find ("powerup_bounce_count").GetComponent<Text> ().text = "x" + Banker.GetComponent<Banker>().TrampolineCount;
			}

		}
	}

	void DoMoneyExplosion(bool KillTail) {
		// as long as enough time has passed since the last
		if(Time.time > LastThud + .4) {
			
			Instantiate(MoneyExplosion, transform.position, transform.rotation);

			// play money explosion sound effect
			FXPlayer.PlayOneShot(MoneyExplosionSound, .75f);
			LastThud = Time.time;
		}

		if (KillTail) {
			// stop the trail of money from leaking out his butt
			MoneyTrail.GetComponent<ParticleSystem> ().Stop ();
			// now change the whoosh sound so that it doesn't sound like money falling
			WhooshPlayer.clip = WhooshSound;
			WhooshPlayer.Play ();
		}
	}

	bool JailNear () {

		//Debug.Log ("checking for nearby jail");

		// Find all game objects with tag jail

		GameObject[] gos;
		gos = GameObject.FindGameObjectsWithTag("jail");

		// Iterate through them and find the closest one
		float shortestDistance = 99f;

		foreach (GameObject go in gos)  {
			float dist = Vector3.Distance(go.transform.position, transform.position);

			if (dist < shortestDistance) {
				shortestDistance = dist;
			}
		}

		//Debug.Log (shortestDistance);

		if (shortestDistance < JailRestingDistance)
			return true;
		else
			return false;

	}


	float map(float s, float a1, float a2, float b1, float b2) {
		return b1 + (s-a1)*(b2-b1)/(a2-a1);
	}
}