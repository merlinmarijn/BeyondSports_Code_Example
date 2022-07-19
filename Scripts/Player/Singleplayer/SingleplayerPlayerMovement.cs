using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SingleplayerPlayerMovement : MonoBehaviour
{
	[Header("Components")]
	private Rigidbody2D _rb;

	[Header("Layer Masks")]
	[SerializeField]
	private LayerMask _groundLayer;

	[Header("Movement Variables")]
	[SerializeField]
	private float _movementAcceleration;

	[SerializeField]
	private float _maxMoveSpeed;

	[SerializeField]
	public float _GroundLinearDrag;

	private float _horizontalDirection;

	[Header("Jump Variables")]
	[SerializeField]
	private float _jumpForce = 12f;

	[SerializeField]
	private float _airLinearDrag = 2.5f;

	[SerializeField]
	private float _fallMultiplier = 8f;

	[SerializeField]
	private float _lowJumpFallMultiplier = 5f;

	[Header("Ground Collision Variables")]
	[SerializeField]
	private float _groundRaycastLength;

	[SerializeField]
	private Vector3 _groundRayCastOffset;
	[SerializeField]
	private Vector3 _groundRayCastOffset1;

	private bool _onGround;

	[Header("Knockback Variables")]
	[SerializeField]
	private float _upKnockbackForce = 12f;

	[SerializeField]
	private float _sideWaysKnockbackForce = 24f;

	private float _resetDistance = -10f;

	private int _hitCount = 0;

	//Knockback base multiplier per hit (10%)
	[SerializeField]
	private float _KnockbackMult = 0.10f;

	private bool _canPlay = true;

	private bool _resetKnockback = true;

	public bool _hasKnockback = false;

	private AudioHandler audio;


	private bool _changingDirection => (_rb.velocity.x > 0f && _horizontalDirection < 0f) || (_rb.velocity.x < 0f && _horizontalDirection > 0f);

	private bool _canJump => Input.GetButtonDown("Jump") && _onGround;

	private Singleplayer_animationHadler AH;

	private void Awake()
	{
		// GetComponent<SpriteRenderer>().color = Color.green;
		audio = GameObject.FindGameObjectWithTag("NetworkController").GetComponent<AudioHandler>();
        //GetComponent<Rigidbody2D>().gravityScale = 0f;
        //GetComponent<PlayerMovement2D>().enabled = false;
        //GetComponent<Rigidbody2D>().bodyType = RigidbodyType2D.Static;
    }

	private void Start()
	{
		_rb = GetComponent<Rigidbody2D>();
		AH = GetComponent<Singleplayer_animationHadler>();
		Debug.Log(AH);
	}

	private void Update()
	{
			_horizontalDirection = GetInput().x;
			if (_resetKnockback)
			{
				CheckCollisions();
			}
			if (_canJump)
			{
				Jump();
			}
			if (transform.position.y <= _resetDistance || transform.position.y >= 0f - _resetDistance)
			{
				_hitCount = 0;
			}
	}

	private void FixedUpdate()
	{
			MoveCharacter();
			if (_onGround)
			{
				ApplyGroundLinearDrag();
				return;
			}
			ApplyAirLinearDrag();
			FallMultiplier();
	}

	private Vector2 GetInput()
	{
		return new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));
	}

	private void MoveCharacter()
	{
		_rb.AddForce(new Vector2(_horizontalDirection, 0f) * _movementAcceleration);
		if (Mathf.Abs(_rb.velocity.x) > _maxMoveSpeed && !_hasKnockback)
		{
			_rb.velocity = new Vector2(Mathf.Sign(_rb.velocity.x) * _maxMoveSpeed, _rb.velocity.y);
		}
	}

	private void ApplyGroundLinearDrag()
	{
		if (Mathf.Abs(_horizontalDirection) < 0.4f || _changingDirection)
		{
			_rb.drag = _GroundLinearDrag;
		}
		else
		{
			_rb.drag = 0f;
		}
	}

	private void ApplyAirLinearDrag()
	{
		_rb.drag = _airLinearDrag;
	}

	public void Jump(float boost = 1)
	{
		_rb.velocity = new Vector2(_rb.velocity.x, 0f);
		_rb.AddForce((Vector2.up * _jumpForce) * boost, ForceMode2D.Impulse);
		//audio here
		audio.PlaySound(2, gameObject, 0.0375f, 0.575f, true);
	}

	public void Knockback(Vector2 EnemyPos, float ItemMult = 1f)
	{
		_hasKnockback = true;
		StartCoroutine(DisableCollisionCheck());
		Vector2 normalized = ((Vector2)transform.position - EnemyPos).normalized;
		Debug.DrawRay(transform.position, normalized * 5f, Color.red);
		Vector2 a = normalized * _sideWaysKnockbackForce;
		Vector2 b = Vector2.up * _upKnockbackForce;
		float d = 1f + _KnockbackMult * (float)_hitCount;
		Vector2 vector = (a + b) * d * ItemMult;
		Debug.DrawRay(transform.position, vector, Color.yellow);
		_hitCount++;
		_rb.AddForce(vector, ForceMode2D.Impulse);
	}

	private void FallMultiplier()
	{
		if (_rb.velocity.y < 0f)
		{
			_rb.gravityScale = _fallMultiplier;
		}
		else if (_rb.velocity.y > 0f && !Input.GetButton("Jump"))
		{
			_rb.gravityScale = _lowJumpFallMultiplier;
		}
		else
		{
			_rb.gravityScale = 1f;
		}
	}

	private IEnumerator DisableCollisionCheck()
	{
		_resetKnockback = false;
		yield return new WaitForSeconds(0.1f);
		_resetKnockback = true;
	}

	private void CheckCollisions()
	{
		_onGround = (Physics2D.Raycast(transform.position + _groundRayCastOffset, Vector2.down, _groundRaycastLength, _groundLayer)
					||
					Physics2D.Raycast(transform.position + _groundRayCastOffset1, Vector2.down, _groundRaycastLength, _groundLayer));
		if(_onGround){
			AH.isGrounded = true;
		}
		else{
			AH.isGrounded = false;
		}
		if (_hasKnockback)
		{
			Debug.Log("SETKNOCKBACK TO FALSE");
			_hasKnockback = false;
		}
	}

	public void SetPlayable(bool Allowed)
	{
		_canPlay = Allowed;
	}

	private void OnDrawGizmos()
	{
		Gizmos.color = Color.green;
		Gizmos.DrawLine(transform.position + _groundRayCastOffset, transform.position + _groundRayCastOffset + Vector3.down * _groundRaycastLength);
		Gizmos.DrawLine(transform.position + _groundRayCastOffset1, transform.position + _groundRayCastOffset1 + Vector3.down * _groundRaycastLength);
	}


}
