using UnityEngine;

public class CameraFollow : MonoBehaviour
{
	private Transform player;
	[SerializeField] private Transform cameraPivot;

	private void Update()
	{
		cameraPivot.transform.position = Vector3.zero;
	}
}
