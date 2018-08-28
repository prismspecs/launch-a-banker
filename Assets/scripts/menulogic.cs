using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

public class menulogic : MonoBehaviour {

	public GameObject AboutPanel;
	public GameObject LoadingPanel;

	public GameObject LaunchText;
	public GameObject AText;
	public GameObject BankerText;
	public TextMesh ScoreText;

	// for quick reference...
	private Material LaunchMat;
	private Material AMat;
	private Material BankerMat;

	private bool introDone = false;

	private float timeStarted = 0f;

	// colors for game name text
	public Color LaunchColor;
	public Color AColor;
	public Color BankerColor;
	public Color ScoreColor;

	// sound
	public AudioSource SoundSource;

	private string tweetText = "I launched a Banker and you can too! My high score is ..., what about you? %23LaunchABanker www.launchabanker.com";

	void Start () {
		// just for convenience...
		LaunchMat = LaunchText.gameObject.GetComponent<Renderer>().material;
		AMat = AText.gameObject.GetComponent<Renderer>().material;
		BankerMat = BankerText.gameObject.GetComponent<Renderer>().material;

		LaunchMat.color = Color.black;
		AMat.color = Color.black;
		BankerMat.color = Color.black;

		timeStarted = Time.time;

		ScoreText.text = "HIGH SCORE: " + PlayerPrefs.GetInt("High Score");
	}
	
	void Update () {
		if (Input.GetKeyDown ("space")) {
			PlayerPrefs.SetFloat("High Score", 0);
			SoundSource.Play(); // play sound
		}

		if(!introDone) {
			if(Time.time - timeStarted > .55) {
				LaunchMat.color = LaunchColor;
			}
			if(Time.time - timeStarted > 1.8) {
				AMat.color = AColor;
			}
			if(Time.time - timeStarted > 3) {
				BankerMat.color = BankerColor;
			}
			if(Time.time - timeStarted > 3.3) {
				LaunchMat.color = Color.black;
				AMat.color = Color.black;
				BankerMat.color = Color.black;
				ScoreText.color = Color.black;
			}
			if(Time.time - timeStarted > 3.6) {
				LaunchMat.color = LaunchColor;
				AMat.color = AColor;
				BankerMat.color = BankerColor;
				ScoreText.color = ScoreColor;
			}
		}
	}

	public void PlayButton() {
		LoadingPanel.SetActive(true);
		SceneManager.LoadScene("new");
	}

	public void AboutButton() {
		AboutPanel.SetActive(!AboutPanel.activeSelf);
		Debug.Log ("back button");
	}

	public void SiteButton() {
		Application.OpenURL("http://www.graysonearle.com/launch-a-banker/");
	}

	public void TwitterButton() {
		// replace spaces with %20
		tweetText = tweetText.Replace(" ", "%20");
		tweetText = tweetText.Replace("...", PlayerPrefs.GetFloat("High Score").ToString("F1"));
		Debug.Log ("tweet");
		//Application.OpenURL ("https://twitter.com/intent/tweet?text=" + message + "&amp;lang=en");
		string message = "https://twitter.com/intent/tweet?text=" + tweetText + "&amp;lang=en";

		Application.OpenURL (message);
	}

	public void FacebookButton() {
		Application.OpenURL ("https://www.facebook.com/dialog/feed?app_id=140586622674265&link=www.launchabanker.net&name=Launch%20A%20Banker&redirect_uri=http%3A%2F%2Fs7.addthis.com%2Fstatic%2Fpostshare%2Fc00.html");

	}

}
