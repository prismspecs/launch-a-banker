using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class menulogic_web: MonoBehaviour {

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
	private float RepeatDelay = .3f;
	private float StartDelay;

	private string tweetText = "I launched a Banker and you can too! It's a new free online game and app. %23LaunchABanker www.launchabanker.com";


	void Start () {

		Cursor.visible = true;

		timeStarted = Time.time;

		//ScoreText.GetComponent<Text>().text = "HIGH SCORE: " + PlayerPrefs.GetFloat("High Score").ToString("F1");

		LaunchText.GetComponent<Text> ().color = Color.black;
		AText.GetComponent<Text> ().color = Color.black;
		BankerText.GetComponent<Text> ().color = Color.black;
		ScoreText.GetComponent<Text> ().color = Color.black;

		Buttons [1].interactable = true;

	}
	
	void Update () {

		if (Input.GetKeyDown ("space")) {
			PlayerPrefs.SetFloat("High Score", 0);
			SoundSource.Play(); // play sound
		}

//		if (Input.GetButtonDown ("Fire1") && InfoPanel.activeSelf) {
//			if(Time.time > StartDelay + RepeatDelay)
//				InfoPanel.SetActive (!InfoPanel.activeSelf);
//		}

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

		if (Input.anyKeyDown) {
			if (InfoPanel.activeSelf) {
				InfoPanel.SetActive(!InfoPanel.activeSelf);
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

	public void SiteButton() {
		Application.OpenURL("http://www.graysonearle.com/launch-a-banker/");
	}

	public void TwitterButton() {
		// replace spaces with %20
		tweetText = tweetText.Replace(" ", "%20");
//		tweetText = tweetText.Replace("...", PlayerPrefs.GetFloat("High Score").ToString("F1"));
//		Debug.Log ("tweet");
		//Application.OpenURL ("https://twitter.com/intent/tweet?text=" + message + "&amp;lang=en");
		string message = "https://twitter.com/intent/tweet?text=" + tweetText + "&amp;lang=en";

		Application.OpenURL (message);
	}

	public void FacebookButton() {
		Application.OpenURL ("https://www.facebook.com/dialog/feed?app_id=140586622674265&link=www.launchabanker.net&name=Launch%20A%20Banker&redirect_uri=http%3A%2F%2Fs7.addthis.com%2Fstatic%2Fpostshare%2Fc00.html");

	}
}
