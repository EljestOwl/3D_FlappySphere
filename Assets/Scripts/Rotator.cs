using System.Collections;
using UnityEngine;

public class Rotator : MonoBehaviour
{
	public Transform pivotPoint;

	public bool isRotating;
	public Vector3 worldDiraction;

	private void Awake()
	{
		worldDiraction = Vector3.right;
	}

	public IEnumerator Rotate(int diraction, float _duration = 0)
	{
		Debug.Log("Rotation Started!");
		// If not given a pivot point: use objects own transform as pivot point.
		if (pivotPoint == null) pivotPoint = transform;

		isRotating = true;

		float _elapsedTime = 0;

		Quaternion startRotation = pivotPoint.transform.rotation;
		Quaternion targetRotation = pivotPoint.transform.rotation * Quaternion.Euler(0, 90 * diraction, 0);

		while (_elapsedTime < _duration)
		{
			pivotPoint.transform.rotation = Quaternion.Slerp(startRotation, targetRotation, _elapsedTime / _duration);
			_elapsedTime += Time.deltaTime;
			yield return null;
		}

		// Update in which direction the object now is in relation to world origin.
		UpdateWorldDirection(diraction);

		pivotPoint.transform.rotation = targetRotation;

		isRotating = false;
		Debug.Log("Rotation Ended!");
	}

	public void UpdateWorldDirection(float angle)
	{
		if (worldDiraction == Vector3.forward)
		{
			if (angle > 0) worldDiraction = Vector3.right;
			else worldDiraction = Vector3.left;
		}
		else if (worldDiraction == Vector3.right)
		{
			if (angle > 0) worldDiraction = Vector3.back;
			else worldDiraction = Vector3.forward;
		}
		else if (worldDiraction == Vector3.back)
		{
			if (angle > 0) worldDiraction = Vector3.left;
			else worldDiraction = Vector3.right;
		}
		else if (worldDiraction == Vector3.left)
		{
			if (angle > 0) worldDiraction = Vector3.forward;
			else worldDiraction = Vector3.back;
		}
		else Debug.Log("Current world direction not recognized! worldDirection: " + worldDiraction + "angle: " + angle);
	}
}