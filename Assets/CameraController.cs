using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour {

	public Transform target;
	public float distance = 30f;
	public float xSpeed = 120.0f;
	public float ySpeed = 120.0f;

	public float xAxis;
	public float yAxis;

	public float yMinLimit = -20f;
	public float yMaxLimit = 80f;

	public float distanceMin = .5f;
	public float distanceMax = 100f;

	private Rigidbody rigidbody;

	float x = 0.0f;
	float y = 0.0f;

	// Use this for initialization
	void Start () {
		Vector3 angles = transform.eulerAngles;
		x = angles.y;
		y = angles.x;
	}
	
	// Update is called once per frame
	void Update () {
		if (target) {
			if (!Manager.instance.paused && !Manager.instance.openingRom) {
				if (Input.GetButton("Fire1")) {
					xAxis += Input.GetAxis("Mouse X") * Time.deltaTime;
					yAxis += Input.GetAxis("Mouse Y") * Time.deltaTime;
				}
				xAxis = Mathf.Lerp(xAxis, 0, Time.deltaTime * 6);
				yAxis = Mathf.Lerp(yAxis, 0, Time.deltaTime * 6);
				x += xAxis * xSpeed;
				y -= yAxis * ySpeed;

				distance = Mathf.Clamp(distance - Input.GetAxis("Mouse ScrollWheel") * Vector3.Distance(target.position, transform.position), distanceMin, distanceMax);
			}

			y = ClampAngle(y, yMinLimit, yMaxLimit);

			Quaternion rotation = Quaternion.Euler(y, x, 0);

			Vector3 negDistance = new Vector3(0.0f, 0.0f, -distance);
			Vector3 position = rotation * negDistance + target.position;

			transform.rotation = rotation;
			transform.position = position;
		}
	}

	public static float ClampAngle(float angle, float min, float max) {
		if (angle < -360F)
			angle += 360F;
		if (angle > 360F)
			angle -= 360F;
		return Mathf.Clamp(angle, min, max);
	}
}
