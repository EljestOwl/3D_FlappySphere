using UnityEngine;

public class Background : MonoBehaviour
{
	public float movementSpeed = 2;
	public int despawnDistance = 15;
	public Vector3 moveDirection;

	private GameObject player;
	[SerializeField] private float distanceToWorldOrigin;

	private void Start()
	{
		player = GameObject.FindGameObjectWithTag("Player");
	}

	private void Update()
	{
		// Check player move direction and reverse it:
		moveDirection = movementSpeed * -1 * player.GetComponent<Rotator>().worldDiraction;

		MoveObject(Time.deltaTime);
	}

	private void FixedUpdate()
	{
		distanceToWorldOrigin = Vector3.Distance(transform.position, Vector3.zero);

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
