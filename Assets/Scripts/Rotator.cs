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
		// If not given a pivot point: use objects own transform as pivot point.
		if (pivotPoint == null) pivotPoint = transform;

		// Rotation started (for other scripts to check)
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

		// Clean up rotation:
		pivotPoint.transform.rotation = targetRotation;

		// Rotation is finnished
		isRotating = false;
	}

	public void UpdateWorldDirection(float angle)
	{
		worldDiraction = RotateVector3(worldDiraction, angle);
	}

	public Vector3 RotateVector3(Vector3 input, float angle)
	{
		if (input == Vector3.forward)
		{
			if (angle >= 0) input = Vector3.right;
			else input = Vector3.left;
		}
		else if (input == Vector3.right)
		{
			if (angle >= 0) input = Vector3.back;
			else input = Vector3.forward;
		}
		else if (input == Vector3.back)
		{
			if (angle >= 0) input = Vector3.left;
			else input = Vector3.right;
		}
		else if (input == Vector3.left)
		{
			if (angle >= 0) input = Vector3.forward;
			else input = Vector3.back;
		}
		else Debug.Log("Current world direction not recognized! worldDirection: " + input + "angle: " + angle);

		return input;
	}
}