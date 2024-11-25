using UnityEngine;

public class WallSegmentMovement : MonoBehaviour
{
	private Rotator playerRotator;

	public float movementSpeed = 2;
	public int despawnDistance = 15;

	private Vector3 moveDirection;
	private float distanceToWorldOrigin;

	private void Start()
	{
		// Find the player's "Rotator"-script:
		playerRotator = GameObject.FindGameObjectWithTag("Player").GetComponent<Rotator>();
	}

	private void Update()
	{
		// Check player world direction and reverse it to create the illusion of player movement:
		moveDirection = movementSpeed * -1 * playerRotator.worldDiraction;

		MoveObject(Time.deltaTime);
	}

	private void FixedUpdate()
	{
		// Destroy self logic:
		// Check its own distance to 0,0,0:
		distanceToWorldOrigin = Vector3.Distance(transform.position, Vector3.zero);

		// If the distance is more then the despawn distance, then destroy GameObject:
		if (distanceToWorldOrigin > despawnDistance)
		{
			Destroy(gameObject);
		}
	}

	private void MoveObject(float timeDiff)
	{
		transform.position = new Vector3(transform.position.x + moveDirection.x * timeDiff, transform.position.y, transform.position.z + moveDirection.z * timeDiff);
	}
}
