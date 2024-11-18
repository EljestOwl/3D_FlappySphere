using System.Collections.Generic;
using UnityEngine;

public class MauntainSpawner : MonoBehaviour
{
	[SerializeField] GameObject spawnableObject;
	[SerializeField] Transform spawnableObjectParent;
	private Rotator _rotator;

	private List<GameObject> _spawnedObjects = new List<GameObject>();

	public int wallSegmentMaxLength;
	public int wallSegmentMinLength;
	public int _spawnCountMax;
	public float spawnOffset;

	[SerializeField] private int _SegmentSpawnCountMax;
	[SerializeField] private int _SegmentSpawnCountCurrent;

	private Vector3 _spawnLocation;
	private bool _canSpawnObjects;

	private void Awake()
	{
		_rotator = GetComponent<Rotator>();
	}

	private void Start()
	{
		//Generate first wall segment:
		ClearAndGenerateNewSegment();

		// Spawn initial object:
		SpawnObject();

		// Insure that objects can start to spawn:
		_canSpawnObjects = true;
	}

	private void FixedUpdate()
	{
		// Clear the list from all null/removed object, i.e. objects that has been destroyed:
		_spawnedObjects.RemoveAll(x => x == null);

		// Check if can spawn new objects to the current segment:
		if (_spawnedObjects.Count < _spawnCountMax && _canSpawnObjects && CheckSpawnDistance())
		{
			// Spawn new object
			SpawnObject();
		}

		// If it can't spawn, check when the object is finished rotation:
		if (!_canSpawnObjects)
		{
			Debug.Log("_canSpawnObjects = " + _canSpawnObjects);
			_canSpawnObjects = !_rotator.isRotating;
		}
	}

	private bool CheckSpawnDistance()
	{
		// Calculate the different distanses from the world center to the spawner and last spawned object:
		float objectDisToOrigin = Vector3.Distance(_spawnedObjects[_spawnedObjects.Count - 1].transform.position, Vector3.zero);
		float spawnerDisToOrigin = Vector3.Distance(transform.position, Vector3.zero);
		float disBetween = spawnerDisToOrigin - objectDisToOrigin;

		// Check if last spawned object location is closer then spawner, and if so return true:
		if (disBetween > spawnOffset) return true;
		else return false;
	}

	private void CalculateSpawnLocation()
	{
		// Get the position of last spawned object:
		_spawnLocation = _spawnedObjects[_spawnedObjects.Count - 1].transform.position;

		// Get the spawn-offset in relation to the spawners location/rotation, and add it to the final spawn location:
		_spawnLocation += _rotator.worldDiraction * spawnOffset;
	}

	private void SpawnObject()
	{
		// Check if there is a spawnable object and if the list is empty:
		if (spawnableObject == null)
		{
			Debug.LogWarning("ERROR: No Spawnable Object found!");
			return;
		}

		// Get the spawn location:
		if (_spawnedObjects.Count == 0) _spawnLocation = transform.position;
		else CalculateSpawnLocation();

		// Spawn the Object:
		GameObject temp = Instantiate(spawnableObject, _spawnLocation, transform.rotation, spawnableObjectParent);
		// Add it to the list:
		_spawnedObjects.Add(temp);
		// Add 1 to the current segment counter:
		_SegmentSpawnCountCurrent++;

		// Check if it is the final object to spawn in the segment:
		if (_SegmentSpawnCountCurrent >= _SegmentSpawnCountMax) Rotate();
	}

	private void Rotate()
	{
		// Disable ability to spawn new objects:
		_canSpawnObjects = false;

		// Rotate spawner:
		StartCoroutine(_rotator.Rotate(-1, 2)); // Currently always turning left.

		// Clear current wall segment and generate new segment:
		ClearAndGenerateNewSegment();

		int temp = _spawnedObjects.Count - 1;


		for (int i = 0; i < temp; i++)
		{
			_spawnedObjects.RemoveAt(0);
			Debug.Log(_spawnedObjects.Count);
		}
	}

	private void ClearAndGenerateNewSegment()
	{
		_SegmentSpawnCountCurrent = 0;
		_SegmentSpawnCountMax = Random.Range(wallSegmentMinLength, wallSegmentMaxLength);

		Debug.Log(_SegmentSpawnCountCurrent + " | " + _SegmentSpawnCountMax);
	}
}
