using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class menulogic_arcade: MonoBehaviour {

	public GameObject LoadingPanel;
	public GameObject InfoPanel;

	public GameObject LaunchText;
	public GameObject AText;
	public GameObject BankerText;
	public GameObject ScoreText;

	private bool introDone = false;
	private float timeStarted = 0f;

	// colors for game name text
	public Color LaunchColor;
	public Color AColor;
	public Color BankerColor;
	public Color ScoreColor;

	// sound
	public AudioSource SoundSource;

	public Button[] Buttons;
	private int CurrentSelection;
	private float RepeatDelay = .3f;
	private float StartDelay;

	void Start () {

		Cursor.visible = false;

		timeStarted = Time.time;

		ScoreText.GetComponent<Text>().text = "HIGH SCORE: " + PlayerPrefs.GetString("initials") + ", " + PlayerPrefs.GetInt("High Score");

		LaunchText.GetComponent<Text> ().color = Color.black;
		AText.GetComponent<Text> ().color = Color.black;
		BankerText.GetComponent<Text> ().color = Color.black;
		ScoreText.GetComponent<Text> ().color = Color.black;

		CurrentSelection = 1;
		Buttons [1].GetComponent<Image> ().color = new Color32 (255, 255, 255, 255);
		Buttons [0].GetComponent<Image> ().color = new Color32 (255, 255, 255, 127);

	}
	
	void Update () {

		if (Input.GetAxis ("Horizontal") < 0) {
			CurrentSelection = 0;
			Buttons [0].GetComponent<Image> ().color = new Color32 (255, 255, 255, 255);
			Buttons [1].GetComponent<Image> ().color = new Color32 (255, 255, 255, 127);
		}

		if (Input.GetAxis ("Horizontal") > 0) {
			CurrentSelection = 1;
			Buttons [1].GetComponent<Image> ().color = new Color32 (255, 255, 255, 255);
			Buttons [0].GetComponent<Image> ().color = new Color32 (255, 255, 255, 127);
		}

		if (Input.GetButtonUp("Fire1")) {
			if(CurrentSelection == 0) {
				HowPlayButton ();
			} else {
				PlayButton ();
			}
		}

		if (Input.GetKeyDown ("space")) {
			PlayerPrefs.SetFloat("High Score", 0);
			//SoundSource.Play(); // play sound
		}

		if(!introDone) {
			if(Time.time - timeStarted > .55) {
				//LaunchMat.color = LaunchColor;
				LaunchText.GetComponent<Text> ().color = LaunchColor;
			}
			if(Time.time - timeStarted > 1.8) {
				//AMat.color = AColor;
				AText.GetComponent<Text> ().color = AColor;
			}
			if(Time.time - timeStarted > 3) {
				//BankerMat.color = BankerColor;
				BankerText.GetComponent<Text> ().color = BankerColor;
			}
			if(Time.time - timeStarted > 3.3) {
				//LaunchMat.color = Color.black;
				LaunchText.GetComponent<Text> ().color = Color.black;
				//AMat.color = Color.black;
				AText.GetComponent<Text> ().color = Color.black;
				//BankerMat.color = Color.black;
				BankerText.GetComponent<Text> ().color = Color.black;
				//ScoreMat.color = Color.black;
				ScoreText.GetComponent<Text> ().color = Color.black;
			}
			if(Time.time - timeStarted > 3.6) {
				LaunchText.GetComponent<Text> ().color = LaunchColor;
				AText.GetComponent<Text> ().color = AColor;
				BankerText.GetComponent<Text> ().color = BankerColor;
				ScoreText.GetComponent<Text> ().color = ScoreColor;

			}
		}
	}

	public void PlayButton() {
		if (!InfoPanel.activeSelf) {
			LoadingPanel.SetActive (true);
			SceneManager.LoadScene ("new");
		}
	}

	public void HowPlayButton() {
		StartDelay = Time.time;
		InfoPanel.SetActive(!InfoPanel.activeSelf);
	}
}
