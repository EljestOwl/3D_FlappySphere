using System.Collections.Generic;
using UnityEngine;

public class MauntainSpawner : MonoBehaviour
{
	[SerializeField] GameObject wallSegment;
	[SerializeField] Transform wallSegmentParent;
	private Rotator _rotator;

	private List<GameObject> _wallSegmentsList = new List<GameObject>();

	public int wallMinLength;
	public int wallMaxLength;
	public int _segmentCountMax;
	public float spawnOffsetSegment; // The offset between wall segments.
	public float spawnOffsetSpawner; // The offset from the spawner.

	[SerializeField] private int _wallLenghtMax;
	[SerializeField] private int _wallLenghtCurrent;

	private Vector3 _spawnLocation;
	private bool _canSpawn;

	private void Awake()
	{
		_rotator = GetComponent<Rotator>();
	}

	private void Start()
	{
		//Generate first wall segment:
		ClearAndGenerateNewWall();

		// Spawn initial wall segment:
		CalculateSpawnLocation();
		SpawnWallSegment();

		// Insure that objects can start to spawn:
		_canSpawn = true;
	}

	private void FixedUpdate()
	{
		// Clear the list from all null/removed segments, i.e. GameObjects that has been destroyed:
		_wallSegmentsList.RemoveAll(x => x == null);

		// Check if can spawn new segment to the current wall:
		if (_wallSegmentsList.Count < _segmentCountMax && _canSpawn && CheckSpawnDistance())
		{
			// Calculate new spawn location from last spawned segemnt:
			CalculateSpawnLocation();

			// Spawn new segment
			SpawnWallSegment();

			// Check if it is the final object to spawn in the segment:
			if (_wallLenghtCurrent >= _wallLenghtMax) Rotate();
		}

		// If it can't spawn, check when the spawner has finished rotating:
		if (!_canSpawn)
		{
			_canSpawn = !_rotator.isRotating;
		}
	}

	private bool CheckSpawnDistance()
	{
		// Calculate the different distances from the world center to the spawner and last spawned object:
		float objectDisToOrigin = Vector3.Distance(_wallSegmentsList[_wallSegmentsList.Count - 1].transform.position, Vector3.zero);
		float spawnerDisToOrigin = Vector3.Distance(transform.position, Vector3.zero);
		float disBetween = spawnerDisToOrigin - objectDisToOrigin;

		// Check if last spawned object location is closer then spawner, and if so return true:
		if (disBetween > spawnOffsetSegment) return true;
		else return false;
	}

	private void CalculateSpawnLocation()
	{
		// Ceck if there are any wall segments spanwed already:
		if (_wallSegmentsList.Count == 0)
		{
			// If not -> get new spawn loacation based on spawner location with offset and then exit the method:
			_spawnLocation = transform.position + _rotator.RotateVector3(_rotator.worldDiraction, -1) * spawnOffsetSpawner;
			return;
		}
		// Get the position of last spawned object:
		_spawnLocation = _wallSegmentsList[_wallSegmentsList.Count - 1].transform.position;

		// Get the spawn-offset in relation to the spawners location/rotation, and add it to the final spawn location:
		_spawnLocation += _rotator.worldDiraction * spawnOffsetSegment;
	}

	private void SpawnWallSegment(bool addToList = true)
	{
		// Check if there is a spawnable object:
		if (wallSegment == null)
		{
			Debug.LogWarning("ERROR: No Spawnable Object found!");
			return;
		}

		// Spawn new wall segment:
		GameObject temp = Instantiate(wallSegment, _spawnLocation, transform.rotation, wallSegmentParent);

		// Check if the segment should be added to the list:
		if (addToList)
		{
			// Add it to the list:
			_wallSegmentsList.Add(temp);
			// Add 1 to the current segment counter:
			_wallLenghtCurrent++;
		}
	}

	private void SpawnWallEndSegment(int rotationDirection)
	{
		Debug.Log("Generate End Wall Segment:");
		if (rotationDirection >= 0)
		{
			Debug.Log("Turning Right");
			// Spawn a wall segment towards the camera but don't add it to the list:
			_spawnLocation = _wallSegmentsList[_wallSegmentsList.Count - 1].transform.position;
			_spawnLocation += _rotator.RotateVector3(_rotator.worldDiraction, 1) * spawnOffsetSpawner * 2;
			SpawnWallSegment(false);

			// spawn 2 more wall segments:
			for (int i = 0; i < 2; i++)
			{
				CalculateSpawnLocation();
				SpawnWallSegment();
				Debug.Log("Spawned wall at: " + _spawnLocation);
			}
		}
		else
		{
			Debug.Log("Turning Left");
			// Get the spawn location:
			_spawnLocation = _wallSegmentsList[_wallSegmentsList.Count - 1].transform.position;
			_spawnLocation += _rotator.worldDiraction * spawnOffsetSegment * 2;

			for (int i = 0; i < 2; i++)
			{
				// Spawn in two wall segments 
				_spawnLocation += _rotator.RotateVector3(_rotator.worldDiraction, 1) * spawnOffsetSpawner;
				SpawnWallSegment(false);
				Debug.Log("Spawned wall at: " + _spawnLocation);
			}
		}
	}

	private void Rotate()
	{
		// Disable ability to spawn new objects:
		_canSpawn = false;

		// Randomize the rotation direction:
		int rotationDirection = Random.Range(-1, 1); // left < 0 < right
		Debug.Log("rotationDirection: " + rotationDirection);

		// Spawn in end segment depending on rotation direction:
		SpawnWallEndSegment(rotationDirection);

		// Rotate spawner:
		StartCoroutine(_rotator.Rotate(rotationDirection, 2));

		// Clear current wall segment length and generate new segment length:
		ClearAndGenerateNewWall();

		// Loop through the list and remove all but the last segment
		for (int i = 0; i < _wallSegmentsList.Count - 1; i++)
		{
			_wallSegmentsList.RemoveAt(0);
		}
	}

	private void ClearAndGenerateNewWall()
	{
		_wallLenghtCurrent = 0;
		_wallLenghtMax = Random.Range(wallMinLength, wallMaxLength);
	}
}