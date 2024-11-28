using UnityEngine;

public class WallShaderSync : MonoBehaviour
{
	[Header("Shader Properties:")]
	public static int DissolveID = Shader.PropertyToID("_DissolveProcentage");
	public static int PosID = Shader.PropertyToID("_PlayerPosition");
	public static int SizeID = Shader.PropertyToID("_CircleSize");
	public static int SmoothnessID = Shader.PropertyToID("_Smoothness");
	public static int OpacityID = Shader.PropertyToID("_Opacity");

	public Material wallMaterial;
	public Camera mainCamera;
	public LayerMask layerMask;

	private void Update()
	{
		Vector3 dir = mainCamera.transform.position - transform.position;
		Ray ray = new Ray(transform.position, dir.normalized);

		if (Physics.Raycast(ray, 3000, layerMask))
		{
			wallMaterial.SetFloat(SizeID, 1);
		}
		else
		{
			wallMaterial.SetFloat(SizeID, 0);
		}

		Vector2 view = mainCamera.WorldToViewportPoint(transform.position);
		wallMaterial.SetVector(PosID, view);
	}
}
