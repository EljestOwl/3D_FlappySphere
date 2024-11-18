using UnityEngine;

public class ObstacleSpawner : MonoBehaviour
{
	[SerializeField] private GameObject _spawnableObject;
	[SerializeField] private Transform _spawnParentTransform;
	private Rotator _rotator;

	public float spawnCooldown;
	public float maxSpawnOffset;

	private float _elapsedTime;
	private float _spawnOffset;

	private void Awake()
	{
		_rotator = GetComponent<Rotator>();
	}

	private void Start()
	{
		_elapsedTime = spawnCooldown;
	}

	private void FixedUpdate()
	{
		if (_elapsedTime >= spawnCooldown)
		{
			SpawnObject();
			_elapsedTime = 0;
		}
		else
		{
			_elapsedTime += Time.fixedDeltaTime;
		}
	}

	private void SpawnObject()
	{
		_spawnOffset = Random.Range(-maxSpawnOffset, maxSpawnOffset);
		Instantiate(_spawnableObject, new Vector3(transform.position.x, _spawnOffset, transform.position.z), Quaternion.identity, _spawnParentTransform);
	}

	private void Rotate(int diraction, float duration)
	{
		StartCoroutine(_rotator.Rotate(diraction, duration));
	}
}
