using UnityEngine;

public class Player : MonoBehaviour
{
	private Rigidbody _rb;
	private Rotator cameraRotator;
	private Rotator playerRotator;

	public float flapStrength = 2f;
	public float turnDurationSec = 2f;

	private bool _inputEnabled = true;
	private bool _isFlapping;

	private void Awake()
	{
		_rb = GetComponent<Rigidbody>();
		cameraRotator = Camera.main.gameObject.GetComponentInParent<Rotator>();
		playerRotator = GetComponent<Rotator>();

	}

	private void Update()
	{
		if (_inputEnabled)
		{
			// Flapp input:
			if (Input.GetKeyDown(KeyCode.Space) || Input.GetKeyDown(KeyCode.W) || Input.GetKeyDown(KeyCode.UpArrow))
			{
				_isFlapping = true;
			}

			// Change direction input:
			if (!playerRotator.isRotating)
			{
				if (Input.GetKeyDown(KeyCode.A) || Input.GetKeyDown(KeyCode.LeftArrow))
				{
					Rotate(-1, turnDurationSec);
				}
				if (Input.GetKeyDown(KeyCode.D) || Input.GetKeyDown(KeyCode.RightArrow))
				{
					Rotate(1, turnDurationSec);
				}
			}
		}
	}

	private void FixedUpdate()
	{
		if (_isFlapping)
		{
			_rb.velocity = new Vector3(0, flapStrength, 0);
			_isFlapping = false;
		}
	}

	private void Rotate(int diraction, float duration)
	{
		StartCoroutine(cameraRotator.Rotate(diraction, duration));
		StartCoroutine(playerRotator.Rotate(diraction, duration));
	}
}
