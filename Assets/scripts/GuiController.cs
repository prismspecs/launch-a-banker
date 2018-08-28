using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEngine.EventSystems;

public class GuiController : MonoBehaviour {

	public Canvas canvas;

	public Text theScoreText;	// for score
	public Text theVictoryText;	// for loss/victory
	public Text theBonusText;
	public GameObject playAgainButton;
	public Button[] PowerupButtons;	// to hide on victory
	private float bonusStarted;
	public float bonusDuration = .9f;
	private float bonusTally = 0f;	// let bonus aggregate and display so multiple bonuses show up correctly

	public float theCount = 0f;	// the score
	private float BankerHeight = 0f;

	[HideInInspector] public bool showing = false;
	[HideInInspector] public bool counting = false;
	private bool roundOver = false;
	[HideInInspector] public float startedCounting = 0f;

	// flash score
	bool flash = false;
	private float startedFlashing;
	float flashDuration = 3f;
	private float flashRate = .1f;
	private bool flashOn = false;
	private float flashLast = 0f;

	public string scoreText = "Air Time:";

	// to play sound FX
	public AudioSource SoundPlayer;
    public AudioSource MusicPlayer;
	public AudioClip WinSound;
    public AudioClip WinMusic;

    // tips when you lose
    private bool HotTipSelected = false;
	public string[] HotTips;
	public string[] MobileStartTips;
	public Text MobileStartingTip;
	public string[] HotTipsWeb;

	public bool UseMobileControls;
	public bool UseArcadeControls;
	public bool UseWebControls;
	public bool UseOSCControls;

	public EventSystem TheEventSystem;

	public bool DoHighScoreScene = false;

	void Start() {
		Cursor.visible = false;

		if (PlayerPrefs.GetInt ("High Score") < 10000 && UseMobileControls) {
			int rando = (int)Random.Range (0f, (float)MobileStartTips.Length);
			MobileStartingTip.text = MobileStartTips[rando];
		}

		// if arcade controls, disable event system to prevent having to
		// press fire twice etc
		if (UseArcadeControls) {
			TheEventSystem.enabled = false;
		}
			
	}
	
	// Update is called once per frame
	void Update () {
		
		// ARCADE...

		if (playAgainButton.activeSelf && Input.GetButton("Fire1")) {
			PlayAgain ();
		}

		// strobing the gui
		if(flash) {
			if(Time.time < startedFlashing + flashDuration) {
				if(Time.time - flashLast > flashRate) {
					flashLast = Time.time;
					flashOn = !flashOn;
				}
			} else {
				flash = false;
				flashOn = true;
			}

			if(flashOn) {

				Color color = theScoreText.color;
				color.a = 1f;
				theScoreText.color = color;

				color = theVictoryText.color;
				color.a = 1f;
				theVictoryText.color = color;
			} else {
				Color color = theScoreText.color;
				color.a = 0f;
				theScoreText.color = color;

				color = theVictoryText.color;
				color.a = 0f;
				theVictoryText.color = color;
			}
		}

		if(counting) {
			// increase score by time (100 points per second I guess)
			theCount += Time.deltaTime * 100f * BankerHeight;

		}

		if(showing) {
			string scoreDecimal = ""+(int)theCount;

			// should only show "score" at the end once he's hit the ground
			if(!roundOver) {
				theScoreText.text = "\n" + scoreDecimal;
			} else {
				theScoreText.text = scoreText + " " + scoreDecimal;
			}
		}

		// bonus text
		if(Time.time > bonusStarted + bonusDuration) {
			theBonusText.enabled = false;
			bonusTally = 0;
			//theScoreText.color = Color.green;
		}
	}

	// public functions to be accessed from banker for score keeping etc
	public void StopCounting() {
		counting = false;
	}

	public void ChangeColor(Color newColor, bool doFlash) {
		theScoreText.color = newColor;

		if (doFlash) {
			flash = true;
			startedFlashing = Time.time;
		}
	}

	public void SetBankerHeight(float bh) {
		BankerHeight = bh;
	}

	public void HidePowerups() {
		//Debug.Log ("hiding powerups");
		foreach (Button butt in PowerupButtons) {
			butt.gameObject.SetActive(false);
		}
	}

	public void ShowPowerups() {
		//Debug.Log ("showing powerups");
		foreach (Button butt in PowerupButtons) {
			butt.gameObject.SetActive(true);
		}
	}

	public void ShowVictory() {
		HidePowerups ();
		roundOver = true;
		theVictoryText.enabled = true;
		theVictoryText.text = "NAILED IT";
		theVictoryText.color = Color.green;
		flash = true;
		startedFlashing = Time.time;

		// test high score
		int highscore = PlayerPrefs.GetInt("High Score");

		if((int)theCount > highscore && !this.gameObject.GetComponent<GuiController>().UseWebControls) {
			// new high score!!
			PlayerPrefs.SetInt("High Score", (int)theCount);
			theVictoryText.text = "NEW HIGH SCORE";
			DoHighScoreScene = true;
		}

		// play win sound
		SoundPlayer.PlayOneShot(WinSound, .75f);
        MusicPlayer.Stop();
        SoundPlayer.PlayOneShot(WinMusic, .5f);
    }


	public void ShowDefeat() {
		HidePowerups ();
		theCount = 0;
		theVictoryText.enabled = true;
		roundOver = true;

//		if (PlayerPrefs.GetFloat ("High Score") < 20000f && !HotTipSelected) {
//
//			if (this.gameObject.GetComponent<GuiController> ().UseWebControls) {
//				int rando = (int)Random.Range (0f, (float)HotTipsWeb.Length);
//				Debug.Log (rando);
//				HotTipSelected = true;
//
//				theVictoryText.text = "HOT TIP\n" + HotTipsWeb [rando];
//			}
//
//			if (this.gameObject.GetComponent<GuiController> ().UseMobileControls) {
//				int rando = (int)Random.Range (0f, (float)HotTips.Length);
//				Debug.Log (rando);
//				HotTipSelected = true;
//
//				theVictoryText.text = "HOT TIP\n" + HotTips [rando];
//			}
//
//		} else {
//			if(!HotTipSelected)
//				theVictoryText.text = "YOU FELL OUT OF BOUNDS";
//		}

		theVictoryText.text = "YOU FELL OUT OF BOUNDS";
		theVictoryText.color = Color.red;
		//flash = true;
		//startedFlashing = Time.time;

		// play again?
		playAgainButton.SetActive(true);
		playAgainButton.GetComponent<Button> ().Select ();
	}

	public void HideInstructions() {
		theVictoryText.enabled = false;
	}

	public void addBonus(float amount) {
		bonusTally += amount;
		theCount += amount;

		// add bonus text!
		theBonusText.enabled = true;
		theBonusText.text = "BONUS\n+" + bonusTally;
		theBonusText.color = new Color(255,255,0);

		bonusStarted = Time.time;
	}

	public void ShowPowerup(string whichPowerup) {
		theBonusText.enabled = true;
		theBonusText.text = whichPowerup;
		theBonusText.color = new Color(255,255,0);

		bonusStarted = Time.time;
	}

	public void PlayAgain() {
		GameObject Banker = GameObject.Find("Banker(Clone)");
		Banker.GetComponent<Banker> ().TrampolineCount = 0;
		SceneManager.LoadScene("new");
	}


}
