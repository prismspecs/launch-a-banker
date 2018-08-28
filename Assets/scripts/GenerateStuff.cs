using UnityEngine;
using System.Collections;

public class GenerateStuff : MonoBehaviour {

	public GameObject LeftBounds;
	public GameObject RightBounds;
	public GameObject NearBounds;
	public GameObject FarBounds;
	public GameObject BottomBounds;
	public GameObject TopBounds;

	public GameObject Trampoline;
	public GameObject Jail;
	public GameObject Magnet;
	public GameObject SuperTramp;

	float xRange1, xRange2, yRange1, yRange2;

	public int TrampolineCount = 100;
	public int JailCount = 500;
	public int TrampolinePowerupCount = 100;
	public int MagnetPowerupCount = 100;

	// Use this for initialization
	void Start () {

		// set initial parameters
		xRange1 = LeftBounds.transform.position.x;
		xRange2 = RightBounds.transform.position.x;
		yRange1 = BottomBounds.transform.position.y + 5f;	// small buffer
		yRange2 = TopBounds.transform.position.y;
		float zRange1 = FarBounds.transform.position.z;
		float zRange2 = NearBounds.transform.position.z;

		// banker Z starts at zero, and don't do it as a coroutine
		// because the first one should just load them all at once
		GenStuff (0, zRange1, zRange2, false);

	}

	float map(float s, float a1, float a2, float b1, float b2) {

		return b1 + (s-a1)*(b2-b1)/(a2-a1);
	}



	public void GenStuff(float BankerZ, float zRange1, float zRange2, bool asCoroutine) {

		DeleteStuff (BankerZ);
		
		StartCoroutine(GenCo(zRange1, zRange2, asCoroutine));

	}

	public void DeleteStuff(float BankerZ) {
		//Debug.Log ("New deletion method");

		foreach (GameObject obj in Object.FindObjectsOfType(typeof(GameObject))) {

			// dont forget i stupidly set everything up backwards so banker flies into negative Z
			if (obj.transform.position.z > BankerZ + 20 && obj.tag != "dontdelete" && obj.tag != "banker") {
				Destroy (obj);
			}
		}
	}

	// co-routine to generat objects
	IEnumerator GenCo(float zRange1, float zRange2, bool asCoroutine) {

		//Debug.Log ("generating!");


		// spawn some jails
		int jailIterator = 0;
		while(jailIterator < JailCount) {

			// generate a random position in bounds of game world
			float xPos = Random.Range (xRange1, xRange2);
			float yPos = Random.Range (yRange1, yRange2);
			//float zPos = Random.Range (zRange1, zRange2);

			// go from near to far
			float zPos = map(jailIterator, 0, JailCount, zRange1, zRange2);

			Vector3 position = new Vector3 (xPos, yPos, zPos);

			// check if any objects are around
			Collider[] hitColliders = Physics.OverlapSphere (position, 8);

			// if there's nothin around...
			if (hitColliders.Length == 0) {
				// make a jail
				Instantiate (Jail, position, Quaternion.identity);
			}

			jailIterator++;

			if(asCoroutine)
				yield return new WaitForSeconds(0.01f);

		}


		// spawn some trampoline powerups
		int trampPowerIterator = 0;
		while(trampPowerIterator < TrampolinePowerupCount) {

			// generate a random position in bounds of game world
			float xPos = Random.Range (xRange1, xRange2);
			float yPos = Random.Range (yRange1, yRange2);
			//float zPos = Random.Range (zRange1, zRange2);

			// go from near to far
			float zPos = map(trampPowerIterator, 0, TrampolinePowerupCount, zRange1, zRange2);

			Vector3 position = new Vector3 (xPos, yPos, zPos);

			// check if any objects are around
			Collider[] hitColliders = Physics.OverlapSphere (position, 3);

			// if there's nothin around...
			if (hitColliders.Length == 0) {

				Instantiate (SuperTramp, position, Quaternion.identity);
			}

			trampPowerIterator++;

			if (asCoroutine)
				yield return null;

		}

	}
}