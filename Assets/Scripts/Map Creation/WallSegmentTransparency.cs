using UnityEngine;

public class WallSegmentTransparency : MonoBehaviour
{
	[SerializeField] Transform wallSegmentFurthest;
	[SerializeField] Transform wallSegmentClosest;
	GameObject cameraObj;

	public float tolarance;

	private Vector3 _cameraPosition;
	private float _distance1;
	private float _distance2;

	private void Start()
	{
		cameraObj = Camera.main.gameObject;
	}

	private void LateUpdate()
	{
		// Get the cameras current position:
		_cameraPosition = cameraObj.transform.position;

		// Calc the distance between the different wall segments and the camera:
		_distance1 = Vector3.Distance(wallSegmentFurthest.position, _cameraPosition);
		_distance2 = Vector3.Distance(wallSegmentClosest.position, _cameraPosition);
	}

	private void FixedUpdate()
	{
		// Compare the distance of both wall segment with each other,
		// if the difference is higher then the tolaranse, do this:
		if (Mathf.Abs(_distance1 - _distance2) > tolarance)
		{
			// Check wich on of the wall segments is closer to the camera and disable the rendering of it:
			if (_distance1 < _distance2)
			{
				wallSegmentFurthest.gameObject.GetComponent<MeshRenderer>().enabled = false;
				wallSegmentClosest.gameObject.GetComponent<MeshRenderer>().enabled = true;
			}
			else
			{
				wallSegmentFurthest.gameObject.GetComponent<MeshRenderer>().enabled = true;
				wallSegmentClosest.gameObject.GetComponent<MeshRenderer>().enabled = false;
			}
		}
		else
		{
			wallSegmentFurthest.gameObject.GetComponent<MeshRenderer>().enabled = true;
			wallSegmentClosest.gameObject.GetComponent<MeshRenderer>().enabled = true;
		}
	}
}
