using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapBuilderNew : MonoBehaviour
{
	[SerializeField] GameObject dubbleWallSegment;
	[SerializeField] GameObject CornerWallSegmentLeft;
	[SerializeField] GameObject CornerWallSegmentRight;
	[SerializeField] Transform wallSegmentParent;
	private Rotator _rotator;

	private List<GameObject> _mapSegmentsList = new List<GameObject>();

	public int wallMinLength;
	public int wallMaxLength;
	public float spawnOffsetSegment; // The offset between wall segments.
	public float newMapDistance;
	public float despawnDistance;

	private Quaternion _spawnRotation;
	private Vector3 _spawnLocation;
	private Vector3 _mapEndLocation;

	private int rotationDirection;
	private int _wallLenghtMax;
	private bool _canSpawn;

	private void Awake()
	{
		_rotator = GetComponent<Rotator>();
	}

	private void Start()
	{
		_canSpawn = true;
		InitialMapGeneration();
	}

	private void FixedUpdate()
	{
		if (Vector3.Distance(_mapEndLocation, Vector3.zero) < newMapDistance && _canSpawn)
		{
			//StartCoroutine(GenerateNewMap());
			// Clear the list from all segments:
			StartCoroutine(ClearAndDestroyList());
		}
		else
		{
			_mapEndLocation = _mapSegmentsList[_mapSegmentsList.Count - 1].transform.position;
		}
	}

	private void CalculateSpawnLocation()
	{
		// Get the position of last spawned object:
		_spawnLocation = _mapSegmentsList[_mapSegmentsList.Count - 1].transform.position;

		// Get the spawn-offset in relation to the spawners location/rotation, and add it to the final spawn location:
		_spawnLocation += _rotator.worldDiraction * spawnOffsetSegment;
	}

	private void CalculateSpawnRotation()
	{
		// Get the rotation based on the world rotation:
		if (_rotator.worldDiraction == Vector3.right) _spawnRotation = Quaternion.Euler(0, 0, 0);
		else if (_rotator.worldDiraction == Vector3.left) _spawnRotation = Quaternion.Euler(0, 180, 0);
		else if (_rotator.worldDiraction == Vector3.forward) _spawnRotation = Quaternion.Euler(0, -90, 0);
		else if (_rotator.worldDiraction == Vector3.back) _spawnRotation = Quaternion.Euler(0, 90, 0);
		else Debug.LogWarning("ERROR: World direction is wierd? worldDirection: " + _rotator.worldDiraction + ", _spawnRotation:" + _spawnRotation);
	}

	private void SpawnWallSegment()
	{
		// Check if there is a spawnable object:
		if (dubbleWallSegment == null)
		{
			Debug.LogWarning("ERROR: No Spawnable Object found!");
			return;
		}

		// Spawn new wall segment:
		GameObject temp = Instantiate(dubbleWallSegment, _spawnLocation, _spawnRotation, wallSegmentParent);

		// Check if the segment should be added to the list:

		// Add it to the list:
		_mapSegmentsList.Add(temp);
	}

	private void SpawnWallCornerSegment(int rotationDirTemp)
	{
		GameObject temp;

		CalculateSpawnLocation();

		// Check direction, true = right:
		if (rotationDirTemp >= 0) temp = Instantiate(CornerWallSegmentRight, _spawnLocation, _spawnRotation, wallSegmentParent);
		else temp = Instantiate(CornerWallSegmentLeft, _spawnLocation, _spawnRotation, wallSegmentParent);

		// Add segment to the list:
		_mapSegmentsList.Add(temp);
	}

	private void GenerateNewMapLength()
	{
		_wallLenghtMax = Random.Range(wallMinLength, wallMaxLength);
	}

	private void InitialMapGeneration()
	{
		// Spawn first wall segment:
		_spawnLocation = new Vector3(-15, 0, 0);
		SpawnWallSegment();

		// Generate initial wall:
		for (int i = 0; i < 6; i++)
		{
			CalculateSpawnLocation();
			SpawnWallSegment();
		}

		// Start first map generation:
		StartCoroutine(ClearAndDestroyList());
	}

	private IEnumerator GenerateNewMap()
	{
		// Calc the new rotation for the map:
		CalculateSpawnRotation();

		// Generate new max length for the map segment:
		GenerateNewMapLength();

		// Generate the full map length:
		for (int i = 0; i < _wallLenghtMax; i++)
		{
			CalculateSpawnLocation();
			SpawnWallSegment();
			yield return null;
		}

		// Randomize the rotation direction:
		rotationDirection = Random.Range(-1, 1); // left < 0 < right
		if (rotationDirection == 0) rotationDirection = 1;

		// Spawn conrner segment depending on rotation direction:
		SpawnWallCornerSegment(rotationDirection);
		yield return null;

		// Rotate spawner to update world direction:
		StartCoroutine(_rotator.Rotate(rotationDirection));

		_canSpawn = true;
	}

	private IEnumerator ClearAndDestroyList()
	{
		// Make sure that no new segments can spawn in while the map gets updated:
		_canSpawn = false;

		float temp;

		Debug.Log("World Direction: " + _rotator.RotateVector3(_rotator.worldDiraction, -rotationDirection));

		// Loop through the list and remove all segments that are behind the player:
		for (int i = 0; i < _mapSegmentsList.Count - 1; i++)
		{
			// Multiply the segments position with the previous world direction to remove irrelevent data
			// from the other axis and get its position in relation to world origin that behind the player:
			Vector3 distance = Vector3.Scale(_mapSegmentsList[i].transform.position, _rotator.RotateVector3(_rotator.worldDiraction, -rotationDirection));

			// Check which value of the Vector3 is not 0 -> store the value:
			// If the value of the remaining axis is negative -> it is behind the player:
			if (distance.x != 0) temp = distance.x;
			else temp = distance.z;

			// If the segment is futher away the the despawn distance -> destroy it
			if (temp < -despawnDistance)
			{
				Destroy(_mapSegmentsList[i]);
			}

			yield return null;
		}

		// Remove all destroyed segments form the list:
		_mapSegmentsList.RemoveAll(x => x == null);

		// Start generating a new map.
		StartCoroutine(GenerateNewMap());
	}

	private void OnDrawGizmos()
	{
		Gizmos.DrawWireSphere(_mapEndLocation, newMapDistance);
	}
}