using System;
using System.Collections;
using System.Collections.Generic;
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

	public Material rightWallMaterial;
	public Material leftWallMaterial;
	public Material[] materials;
	public Material material;
	public Camera mainCamera;
	public LayerMask layerMask;
	private RaycastHit hit;
	private bool _isFading;
	private Dictionary<Material, Coroutine> _runningCoroutines = new Dictionary<Material, Coroutine>();

	private void Update()
	{
		Vector3 dir = mainCamera.transform.position - transform.position;
		Ray ray = new Ray(transform.position, dir.normalized);

		if (Physics.Raycast(ray, out hit, 10f, layerMask))
		{
			material = hit.transform.gameObject.GetComponent<Material>();

			if (materials.Contains(material))
			{
				if (_runningCoroutines.ContainsKey(material))
				{
					if (_runningCoroutines[material] != null)
					{
						StopCoroutine(_runningCoroutines[material]);
					}
				}
				_runningCoroutines.Add(material, StartCoroutine(FadeOut(material)));
			}
		}
		else
		{

		}

		Vector2 view = mainCamera.WorldToViewportPoint(transform.position);
		leftWallMaterial.SetVector(PosID, view);
	}

	private IEnumerator FadeOut(Material material)
	{
		float timeToLerp = lerpTime / (1 / material.GetFloat(SizeID));

		while (material.GetFloat(SizeID) < 0.98f && material.GetFloat(TransparancyID) < 0.48f)
		{
			material.SetFloat(SizeID, Mathf.Lerp(material.GetFloat(SizeID), 1f, timeToLerp));
			material.SetFloat(TransparancyID, Mathf.Lerp(material.GetFloat(TransparancyID), 0.5f, timeToLerp / 2));
			yield return null;
		}
		material.SetFloat(SizeID, 1f);
		material.SetFloat(TransparancyID, 0.5f);
	}

	private IEnumerator FadeIn(Material material)
	{
		float timeToLerp = lerpTime * material.GetFloat(SizeID);

		while (material.GetFloat(SizeID) > 0.02f && material.GetFloat(TransparancyID) > 0.02f)
		{
			material.SetFloat(SizeID, Mathf.Lerp(material.GetFloat(SizeID), 0f, timeToLerp));
			material.SetFloat(TransparancyID, Mathf.Lerp(material.GetFloat(TransparancyID), 0f, timeToLerp / 2));
			yield return null;
		}

		material.SetFloat(SizeID, 0f);
		material.SetFloat(TransparancyID, 0f);
	}
}
