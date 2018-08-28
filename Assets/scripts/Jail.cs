using UnityEngine;
using System.Collections;

public class Jail : MonoBehaviour {
	
	public float BonusAmount = 100;
	private GameObject TheController;
	public GameObject BonusFX;

	public TextMesh BonusText;

	private bool BonusGiven;

	private GameObject TheBanker;

	// up down movement
	private float phase;
	private float phaseStrength;
	private float phaseInc;

	void Start () {

		float dist = Mathf.Abs (0 - transform.position.z);

		BonusAmount = (int)dist * 10;

		BonusText.text = "+" + BonusAmount;

		// for floating jails
		phase = Random.Range (0f, 360f);
		phaseStrength = Random.Range (.002f, .03f);
		phaseInc = Random.Range (.001f, .01f);
	}

	void Update() {
		phase += phaseInc;

		Vector3 FloatOffset = new Vector3 (0, Mathf.Sin(phase) * phaseStrength, 0);

		transform.parent.transform.position += FloatOffset;
	}

	void OnTriggerEnter() {

		if(!BonusGiven) {
			// bonus FX
			Vector3 newPosition = new Vector3(transform.position.x, transform.position.y + 2, transform.position.z);

			Object hearts = Instantiate(BonusFX, newPosition, transform.rotation);
			Destroy (hearts, 2.0f);

			Destroy(this);

			TheController = GameObject.Find("GAME-CONTROLLER");
			TheController.GetComponent<GuiController> ().addBonus (BonusAmount);

			BonusGiven = true;
		}
	}
}