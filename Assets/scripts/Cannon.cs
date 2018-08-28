using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class Cannon : MonoBehaviour {

	// drag aim stuff
	private bool Clicked = false;
	private bool SafeZone = false;
	private Vector3 FirstMousePos;
	public Slider VerticalSlider;
	public Slider HorizontalSlider;
	private float prevH;
	private float prevV;
	private float SafeX, SafeY;
	public Button[] PowerupButtons;

	// what're we shootin' today?
	public Transform projectile;
	public Transform ProjectorBanker;
	// and where from?
	public Transform shootFrom;
	// and how fast?
	public float bulletSpeed = 1000f;

	// main camera
	public Camera mainCam;

	// smoke/explosion
	public Transform poof;

	// stuff to handle waiting before following w camera
	private float startFollowCounter = 0.0f;
	public float waitFollowDuration = 1.5f;

	// we in aimin' mode?
	private bool aiming = true;

	// UI
	public GameObject TheController;
	public Button theLaunchButton;
	public GameObject EventSystem;

	// what kind of controller?
	public bool useBrowserController = false;
	public bool useMobileController = true;

	// cannon rotation
	private float canX;
	private float canY;

	// when did scene begin?
	private float sceneStarted = 0.0f;

	// sound effects
	public AudioSource SoundPlayer;
    public AudioSource MusicPlayer;
    public AudioClip MusicLoopClip;
	public AudioClip CannonFireSound;
	public AudioClip CannonAimSound;
	private float lastCannonClick = 0.0f;


	// Use this for initialization
	void Start () {
		
		sceneStarted = Time.time;

		// drag aim setup
		SafeX = Screen.width * .14f;
		SafeY = Screen.height * .35f;

		// if using web or arcade controls, disable input system
		if(TheController.GetComponent<GuiController>().UseArcadeControls || TheController.GetComponent<GuiController>().UseWebControls)
			EventSystem.SetActive(false);

	}

	// Update is called once per frame
	void Update () {
		// enable launch button after half a sec
		if(Time.time > sceneStarted + .25 && !theLaunchButton.interactable && TheController.GetComponent<GuiController> ().UseMobileControls) {
			theLaunchButton.interactable = true;
			theLaunchButton.Select();
		}

		if (aiming == true) {

			if (TheController.GetComponent<GuiController> ().UseOSCControls) {

				HorizontalSlider.value = Input.mousePosition.x / Screen.width;
				VerticalSlider.value = Input.mousePosition.y / Screen.height;

				if (Input.GetMouseButtonDown(0)) {
					LaunchButton();
				}
			}

			if (TheController.GetComponent<GuiController> ().UseWebControls) {

				HorizontalSlider.value += (Input.GetAxis ("Horizontal") / 160f);
				VerticalSlider.value += (Input.GetAxis ("Vertical") / 160f);

				if (Input.GetButtonDown("Fire1")) {
					LaunchButton();
				}
			}

			if (TheController.GetComponent<GuiController> ().UseArcadeControls) {
				
				HorizontalSlider.value += (Input.GetAxis ("Horizontal") / 3f ) *-1f * Time.deltaTime;
				VerticalSlider.value += (Input.GetAxis ("Vertical") / 3f ) *-1f * Time.deltaTime;

				if (Input.GetButtonDown ("Fire1")) {
					LaunchButton ();
				}
			} 

			//TheController.GetComponent<GuiController>().MobileStartingTip.text = "" + Input.GetButtonDown ("Fire1");




			// ------------- drag aim stuff -----------
			if (TheController.GetComponent<GuiController> ().UseMobileControls) {
				if (Input.mousePosition.x > SafeX && Input.mousePosition.y > SafeY) {
					SafeZone = true;
				} else {
					SafeZone = false;
				}

				if (Input.GetMouseButtonDown (0) && SafeZone) {
			
					Clicked = true;

					FirstMousePos = Input.mousePosition;
				}

				if (Input.GetMouseButtonUp (0)) {
					Clicked = false;
				}

				if (Clicked) {
					// option 1: aiming power increases with distance from start drag position
					Vector3 DiffPos = Input.mousePosition - FirstMousePos;

					float MappedX = DiffPos.x / Screen.width / 15;
					float MappedY = DiffPos.y / Screen.height / 15;

					HorizontalSlider.value += MappedX;
					VerticalSlider.value += MappedY;

				}
			}
		}

		// start following projectile after a short wait
		if(startFollowCounter > 0f  && Time.time > startFollowCounter + waitFollowDuration) {
			//mainCam.GetComponent<CAMERA> ().isFollowing = true;
			mainCam.GetComponent<BankerCam> ().isFollowing = true;
		}
	}

	// mobile: launch button has been pressed
	public void LaunchButton() {
		Launch();
		VerticalSlider.gameObject.SetActive (false);
		HorizontalSlider.gameObject.SetActive (false);
		theLaunchButton.gameObject.SetActive(false);

		// show powerups
		foreach (Button butt in PowerupButtons) {
			butt.interactable = true;
			butt.gameObject.SetActive (true);
		}

		// play sound sound of cannon firing
		SoundPlayer.PlayOneShot(CannonFireSound, 1);
        MusicPlayer.clip = MusicLoopClip;
        MusicPlayer.Play();
        MusicPlayer.loop = true;
	}

	void Launch() {
		// get rid of starting tip on mobile
		TheController.GetComponent<GuiController>().MobileStartingTip.text = "";

		// release camera from parent
		mainCam.transform.parent = null;

		// shake camera
		mainCam.GetComponent<BankerCam> ().Shake ();

		// if showing instructions, go bye bye
		//mainCam.SendMessage ("HideInstructions");

		// burst of smoke
		Instantiate(poof, shootFrom.transform.position, shootFrom.transform.rotation);

		// Instantiate the projectile at the position and rotation of this transform
		// the instantiate function creates an instance of a prefab, in our case the 'projectile'
		// it requires a position and rotation at which to instantiate said object
		Transform bankerClone;

		if (TheController.GetComponent<GuiController> ().UseOSCControls) {
			bankerClone = (Transform)Instantiate(ProjectorBanker, shootFrom.transform.position, shootFrom.transform.rotation);
			bankerClone.gameObject.name = "Banker(Clone)";
		} else {
			bankerClone = (Transform)Instantiate(projectile, shootFrom.transform.position, shootFrom.transform.rotation);
		}

		// Add force to the cloned object in the object's forward direction
		bankerClone.GetComponent<Rigidbody>().AddForce(shootFrom.transform.forward * bulletSpeed);
		bankerClone.transform.rotation = Random.rotation;

		// attach camera to banker
		startFollowCounter = Time.time;
		mainCam.GetComponent<BankerCam> ().targetPos = bankerClone;

		// no longer in aiming mode
		aiming = false;

		// start up GUI
		TheController.GetComponent<GuiController> ().canvas.enabled = true;
		TheController.GetComponent<GuiController> ().counting = true;
		TheController.GetComponent<GuiController> ().showing = true;
		TheController.GetComponent<GuiController> ().startedCounting = Time.time;

	}

	// aiming via mobile sliders
	public void UpdateSliderVert(Slider slider) {
		transform.localEulerAngles = new Vector3 (slider.value * 90f - 45f, transform.localEulerAngles.y, transform.localEulerAngles.z);

		// play sound
		if(Time.time > lastCannonClick + CannonAimSound.length) {
			SoundPlayer.PlayOneShot(CannonAimSound, 1);
			lastCannonClick = Time.time;
		}
	}

	public void UpdateSliderHorz(Slider slider) {
		transform.localEulerAngles = new Vector3 (transform.localEulerAngles.x, slider.value * 50 - 25, transform.localEulerAngles.z);

		// play sound
		if(Time.time > lastCannonClick + CannonAimSound.length) {
			SoundPlayer.PlayOneShot(CannonAimSound, 1);
			lastCannonClick = Time.time;
		}
	}

	float map(float s, float a1, float a2, float b1, float b2) {
		return b1 + (s-a1)*(b2-b1)/(a2-a1);
	}
}