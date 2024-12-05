using System;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;

public class WallShaderSync : MonoBehaviour
{
	[Header("Shader Properties:")]
	public static int PosID = Shader.PropertyToID("_PlayerPosition");
	public static int SizeID = Shader.PropertyToID("_CircleSize");
	public static int SmoothnessID = Shader.PropertyToID("_Smoothness");
	public static int TransparancyID = Shader.PropertyToID("_Transparancy");

	public float lerpTime = 1;

	public Material[] affectedMaterials;
	public Camera mainCamera;
	public LayerMask layerMask;
	private RaycastHit hit;
	private Vector3 dir;
	private Ray ray;
	private Vector2 view;

	private void Start()
	{
		// Resett the materials to starting values:
		foreach (var mat in affectedMaterials)
		{
			mat.SetFloat(SizeID, 0);
			mat.SetFloat(TransparancyID, 0);
		}
	}

	private void Update()
	{
		// Create a ray from the camera to the player:
		dir = mainCamera.transform.position - transform.position;
		ray = new Ray(transform.position, dir.normalized);

		// Make a new list to keep track of the materials:
		Material[] newMaterials = new Material[affectedMaterials.Length];

		// Copy the materials from the first list to the above list:
		for (int i = 0; i < newMaterials.Length; i++)
		{
			newMaterials[i] = affectedMaterials[i];
		}

		// Raycast to see if there are any walls between the camera and the player:
		if (Physics.Raycast(ray, out hit, 10f, layerMask))
		{
			// Get the material of the hit object:
			Material material = hit.transform.gameObject.GetComponentInParent<MeshRenderer>().sharedMaterial;

			// Check if the material is in the list of materials:
			if (affectedMaterials.Contains(material))
			{
				// Get the index of that material:
				int index = Array.IndexOf(affectedMaterials, material);

				// Remove that material from the new list, so that we don't try to fade it in:
				newMaterials[index] = null;

				// Fade out the material:
				if (affectedMaterials[index].GetFloat(SizeID) < 2f)
					affectedMaterials[index].SetFloat(SizeID, material.GetFloat(SizeID) + Time.deltaTime);
				if (affectedMaterials[index].GetFloat(TransparancyID) < 0.5f)
					affectedMaterials[index].SetFloat(TransparancyID, material.GetFloat(TransparancyID) + Time.deltaTime);
			}
		}

		foreach (var mat in newMaterials)
		{
			// If the current material is null, skip the rest of the current loop:
			if (mat == null) continue;

			// Fade in the remaining materials:
			Debug.Log("Material Fading In: " + mat.name);
			if (mat.GetFloat(SizeID) > 0f)
				mat.SetFloat(SizeID, mat.GetFloat(SizeID) - Time.deltaTime);
			if (mat.GetFloat(TransparancyID) > 0f)
				mat.SetFloat(TransparancyID, mat.GetFloat(TransparancyID) - Time.deltaTime);
		}

		// Get the players position and convert it to screen space:
		view = mainCamera.WorldToViewportPoint(transform.position);

		foreach (var mat in affectedMaterials)
		{
			// Get the players position and convert it to screen space:
			mat.SetVector(PosID, view);
		}
	}
}
