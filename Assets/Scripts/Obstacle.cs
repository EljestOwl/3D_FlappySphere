using UnityEngine;

public class Obstacle : MonoBehaviour
{
	public float movementSpeed = 2;
	public int despawnDistance = 15;
	public Vector3 moveDirection;

	private GameObject player;

	private void Start()
	{
		player = GameObject.FindGameObjectWithTag("Player");
		moveDirection = movementSpeed * -1 * player.GetComponent<Rotator>().worldDiraction;
	}

	private void Update()
	{
		MoveObject(Time.deltaTime);
	}

	private void FixedUpdate()
	{
		if (Vector3.Distance(transform.position, Vector3.zero) < -despawnDistance)
		{
			Destroy(gameObject);
		}
	}

	private void MoveObject(float timeDiff)
	{
		transform.position = new Vector3(transform.position.x + moveDirection.x * timeDiff, transform.position.y, transform.position.z + moveDirection.z * timeDiff);
	}
}
