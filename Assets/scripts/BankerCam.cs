using UnityEngine;
using System.Collections;

public class BankerCam : MonoBehaviour {

	public bool isFollowing = false;
	public Transform targetPos;

	public Vector3 offset = new Vector3(0f, 1.2f, 3f);
	private float maxOffsetX = 6f;
	private float lastOffsetZ;
	public Vector3 cameraAngle = new Vector3(0f, 180f, 0f);
	private Vector3 startingPosition = new Vector3(0f, 0f, 0f);

	// camera lerp
	private float lerpPercent = .1f;
	private float lerpInc = .005f;

	// camera shake
	public float shakeDuration = .3f;
	private float shakeStarted;
	private bool shake = false;
	public float shakeStrength = .01f;
	private Vector3 shakeOffset;

	private float offsetAngle;

	// keep track of cannon
	public Transform cameraFollow;

	// background colors
	public Color skyBlue;
	public Color heavenBlue;

	public GameObject TheController;

	// Use this for initialization
	void Start () {
		startingPosition = transform.position;

		// set up lastOffset on the right foot
		lastOffsetZ = offset.z;

		if (TheController.GetComponent<GuiController> ().UseOSCControls)
			GetComponent<Camera> ().backgroundColor = Color.black;

	}

	public void Shake() {
		shake = true;
		shakeStarted = Time.time;
	}
	
	// Update is called once per frame
	void Update () {
		if(shake && Time.time < shakeStarted + shakeDuration) {
			shakeStrength *= -1;

			shakeOffset.x = shakeStrength;
			shakeOffset.x = -shakeStrength;

			transform.position += shakeOffset;
		}
			
	}


	// when you hit a trampoline or something, reset starting angle
	// so camera changes direction if banker does
	public void setAngle(Vector3 incoming) {
		startingPosition = incoming;
	}

	private bool initialAngleSet = false;

	void FixedUpdate () {

		if(isFollowing) {

			// aim the camera at the banker
			Vector3 relativePos = targetPos.position - transform.position;
			Quaternion rotation = Quaternion.LookRotation(relativePos);
			transform.rotation = Quaternion.Lerp (transform.rotation, rotation, lerpPercent);

			// lock Z rotation!
			transform.localEulerAngles = new Vector3(transform.localEulerAngles.x,transform.localEulerAngles.y,0);

			// get banker object for speed test
			GameObject banker = GameObject.Find("Banker(Clone)");

			if (!initialAngleSet) {
				setAngle (banker.transform.position);
			}

			// if banker exists B^)
			if(banker != null) {

				// get speed of banker
				float speed = Mathf.Abs(banker.GetComponent<Rigidbody>().velocity.y);

				//Debug.Log(speed);

				float clampedSpeed = Mathf.Clamp(speed, 2, 7);

				// get height of banker
				float clampedPosition = Mathf.Clamp(banker.transform.position.y, 2, 7);

				// favor speed over height for zoom
				float newOffsetZ = (clampedPosition * .0f) + (clampedSpeed * 1.0f);

				// dampen/smooth
				offset.z = (newOffsetZ * .05f) + (lastOffsetZ * .95f);

				// if camera is offset far to the side of the banker (angular shot)
				// then don't go so far on Z axis
				//float xDistance = (maxOffsetX - Mathf.Abs(offset.x)) / maxOffsetX * 1.1f;

				// so if the x angle is large, then offset Z will be small
				//offset.z *= xDistance;

				lastOffsetZ = offset.z;

				// set Y offset based on whether banker is going up or down 1.2 is standard
				// BUT if Z offset is low (because banker is at the top of an arc)
				// then constrain Y offset as well for nice effect
				float lastOffsetY = offset.y;
				float unbiasedYOffset = map(banker.GetComponent<Rigidbody>().velocity.y, -40f, 40f, 10f, 1.2f);
				float biasedYOffset = Mathf.Clamp(unbiasedYOffset, 0f, offset.z * 1.2f);

				offset.y = Mathf.Lerp(lastOffsetY, biasedYOffset, .1f);

				// change background color based on height?
				if (!TheController.GetComponent<GuiController> ().UseOSCControls)
					GetComponent<Camera>().backgroundColor = Color.Lerp(skyBlue, heavenBlue, banker.transform.position.y/200);
			}

			// position it
			var direction = startingPosition - transform.position;

			// clamp at -6,6 offset: any farther and it just makes the camera really far from banker
			offset.x = Mathf.Clamp(direction.x * .1f, -maxOffsetX, maxOffsetX);

			transform.position = Vector3.Lerp(transform.position, targetPos.position + offset, lerpPercent);

			//lerpPercent = Mathf.Clamp(lerpPercent + lerpInc, 0, 1);
			lerpPercent += lerpInc;

		} else {

			// aiming mode: angle with the cannon
			transform.position = cameraFollow.position;
			transform.rotation = cameraFollow.rotation;
		}
	}

	float map(float s, float a1, float a2, float b1, float b2) {
		return b1 + (s-a1)*(b2-b1)/(a2-a1);
	}
}
