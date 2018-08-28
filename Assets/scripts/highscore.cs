using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class highscore : MonoBehaviour {

	// constrain x -280, 280
	// y -20, -150

	public float speed = 2.0f;
	public Color notSelected = Color.gray;
	public Color yesSelected = Color.cyan;

	public Text HighScore;
	public Text InitialsText;
	public GameObject okButton;

	private string initials = "";

	// Use this for initialization
	void Start () {

		HighScore.text = "" + PlayerPrefs.GetInt ("High Score");

		var pos = transform.position;
		pos.x =  0.0f;
		pos.y = -10.0f;
		transform.position = pos;
	}
	
	// Update is called once per frame
	void Update () {
	
		Vector3 moveDir = Vector3.zero;
		moveDir.x = Input.GetAxis("Horizontal");
		moveDir.y = Input.GetAxis("Vertical");

		// move this object at frame rate independent speed:
		transform.position += moveDir * speed * Time.deltaTime;

		var pos = transform.position;
		pos.x =  Mathf.Clamp(transform.position.x, -73.0f, 74.0f);
		pos.y =  Mathf.Clamp(transform.position.y, -40.0f, -3.0f);
		//Debug.Log (pos.y);
		transform.position = pos;

		var CharInQuestion = FindClosestLetter ();
		CharInQuestion.GetComponent<Text> ().color = Color.cyan;

		// hit key
		if (Input.GetButtonDown ("Fire1")) {
			var Initial = FindClosestLetter ();
			if (Initial.name != "del" && Initial.name != "ok!") {

				// player entering a letter
				if(initials.Length < 3)
					initials += Initial.GetComponent<Text> ().text;

				// if its the last letter, teleport to ok!
				if (initials.Length == 3)
					transform.position = okButton.transform.position;
					//transform.position = new Vector3 (67.6f, -36.0f, transform.position.z);
				
			} else {
				if (Initial.name == "del") {
					if(initials.Length > 0)
						initials = initials.Substring(0, initials.Length - 1);
				} else {
					// ok! enter it in
					PlayerPrefs.SetString("initials",initials);
					SceneManager.LoadScene("menu_arcade");
				}
			}

			InitialsText.text = initials;
		}

		//Debug.Log (transform.position);

	}

	GameObject FindClosestLetter() {
		GameObject[] gos;
		gos = GameObject.FindGameObjectsWithTag("highscore letter");
		GameObject closest = null;
		float distance = Mathf.Infinity;
		Vector3 position = transform.position;
		foreach (GameObject go in gos) {
			Vector3 diff = go.transform.position - position;
			float curDistance = diff.sqrMagnitude;

			float clampedDistance = Mathf.Clamp (curDistance, 0.0f, 100.0f);

			float mappedDistance = map (clampedDistance, 100.0f, 0.0f, 0.0f, 1.0f);

			// change color based on distance
			go.GetComponent<Text>().color = Color.Lerp(notSelected, yesSelected, mappedDistance);

			if (curDistance < distance) {
				closest = go;
				distance = curDistance;
			}
		}
		//Debug.Log (distance);
		return closest;
	}

	float map(float s, float a1, float a2, float b1, float b2) {
		return b1 + (s-a1)*(b2-b1)/(a2-a1);
	}
}