using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro;


public class PlayerMovement2D : MonoBehaviourPunCallbacks
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
	private float _jumpForce = 18f;

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
	private Vector3 _groundRayCastOffset1;
	[SerializeField]
	private Vector3 _groundRayCastOffset;

	[SerializeField]
	private bool _onGround;

	[Header("Knockback Variables")]
	[SerializeField]
	private float _upKnockbackForce = 1.5f;

	[SerializeField]
	private float _sideWaysKnockbackForce = 3f;

	private float _resetDistance = -10f;

	private int _hitCount = 0;

	//Knockback base multiplier per hit (10%)
	[SerializeField]
	private float _KnockbackMult = 0.10f;

	private bool _canPlay = true;

    public float direction;

	private bool _resetKnockback = true;

	public bool _hasKnockback = false;

	private AudioHandler audio;

	[SerializeField]
	private TextMeshPro Name;
	[SerializeField]

    private bool _changingDirection => (_rb.velocity.x > 0f && _horizontalDirection < 0f) || (_rb.velocity.x < 0f && _horizontalDirection > 0f);

	private bool _canJump => inputManager.GetButtonDown("Jump") && _onGround;

	InputManger inputManager;
	
	private AnimationHandler AH;

	public int inputX;

	UIManager UIM;

	private float holdForce;
	private float holdSpeed;

	private void Awake()
    {
        if (photonView.IsMine)
        {
            Name.color = Color.green;
			photonView.RPC("RPC_SetName", RpcTarget.All, PhotonNetwork.NickName);
        }
        audio = GameObject.FindGameObjectWithTag("NetworkController").GetComponent<AudioHandler>();
		UIM = GameObject.FindGameObjectWithTag("NetworkController").GetComponent<UIManager>();
		if (!photonView.IsMine)
        {
            Name.color = Color.red;
            GetComponent<Rigidbody2D>().gravityScale = 0f;
			//GetComponent<PlayerMovement2D>().enabled = false;
			GetComponent<Rigidbody2D>().bodyType = RigidbodyType2D.Static;
			gameObject.layer = 8;
        }
		inputManager = FindObjectOfType<InputManger>();
		//NamePos.DetachChildren();
		Name.transform.eulerAngles = new Vector3(0, 0, 0);

		holdSpeed = _maxMoveSpeed;
		holdForce = _jumpForce;
    }

    [PunRPC]
	void RPC_SetName(string name)
    {
		Name.text = name;
    }

    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
		photonView.RPC("RPC_SetName", newPlayer, PhotonNetwork.NickName);
		photonView.RPC("RPC_SyncParent", newPlayer, GetComponent<PhotonView>().ViewID, transform.parent.GetComponent<PhotonView>().ViewID);
    }

    private void Start()
	{
		_rb = GetComponent<Rigidbody2D>();
		AH = GetComponent<AnimationHandler>();
		Debug.Log(AH);
	}


    private void Update()
	{
		//Debug.Log(_onGround);
		//if (_onGround && transform.parent.gameObject.layer != _groundLayer)
		//{
		//	GameObject platform = Physics2D.Raycast(transform.position + _groundRayCastOffset, Vector2.down, _groundRaycastLength, _groundLayer).transform.gameObject;
		//	photonView.RPC("RPC_ParentToPlatform", RpcTarget.All, platform.GetComponent<PhotonView>().ViewID);
		//	//transform.parent = platform.transform;
		//}
		//else if (!_onGround && transform.parent.gameObject.layer == _groundLayer)
  //      {
		//	photonView.RPC("RPC_UnParentToPlatform", RpcTarget.All);
		//	//transform.parent = null;
  //      }
  
		//Name.transform.position = NamePos.transform.position;
		Name.transform.eulerAngles = new Vector3(0, Mathf.Abs(transform.rotation.y), 0);

		if (photonView.IsMine && _canPlay)
		{
			_horizontalDirection = GetInput().x;
			if (_resetKnockback)
			{
				CheckCollisions();
			}
			if (_canJump)
			{
				//Jump();
				AH.GetComponent<Animator>().SetBool("Jumping", true);
			}
			if (transform.position.y <= _resetDistance || transform.position.y >= 0f - _resetDistance)
			{
				_hitCount = 0;
			}
		}
	}

	private void FixedUpdate()
	{
		if (photonView.IsMine)
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
	}

	private Vector2 GetInput()
	{

		if (inputManager.GetWalkButtonDown("Left"))
		{
			inputX = -1;
		}
		else if (inputManager.GetWalkButtonDown("Right"))
		{
			inputX = 1;
		}
		else
		{
			inputX = 0;
		}
		return new Vector2(inputX, Input.GetAxisRaw("Vertical"));
	}

	private void MoveCharacter()
	{
        _rb.AddForce(new Vector2(inputX, 0f) * _movementAcceleration);
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
		//GetComponent<AnimationHandler>().RPC_Animetrigger("jump"); // GetComponent<AnimationHandler>().photonView.RPC("RPC_Animetrigger", RpcTarget.All, "jump");
		_rb.velocity = new Vector2(_rb.velocity.x, 0f);
		_rb.AddForce((Vector2.up * _jumpForce)*boost, ForceMode2D.Impulse);
		//audio here
		audio.PlaySound(2, gameObject, 0.0375f, 0.575f, true);
	}

	public void Heal(int HealAmt){
		_hitCount -= (_hitCount -= HealAmt !< 0 ? HealAmt : _hitCount);
	}


	[PunRPC]
	public void StartSlow(float time)
	{
		StartCoroutine(SlowDown(time));
	}
	    IEnumerator SlowDown(float time)
    {
        _jumpForce = (_jumpForce / 100 * 65);
		_maxMoveSpeed = (_maxMoveSpeed / 100 * 85);
        yield return new WaitForSeconds(time);
        _jumpForce = holdForce;
		_maxMoveSpeed = holdSpeed;
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
		UIM.UpdateKnockbackUI(_KnockbackMult * _hitCount);
		_rb.AddForce(vector, ForceMode2D.Impulse);
	}

	private void FallMultiplier()
	{
		if (_rb.velocity.y < 0f)
		{
			_rb.gravityScale = _fallMultiplier;
		}
		else if (_rb.velocity.y > 0f && !inputManager.GetWalkButtonDown("Jump"))
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
		_onGround = 
			(Physics2D.Raycast(transform.position + _groundRayCastOffset, Vector2.down, _groundRaycastLength, _groundLayer)
			||
			Physics2D.Raycast(transform.position + _groundRayCastOffset1, Vector2.down, _groundRaycastLength, _groundLayer));
		if(_onGround){
			AH.isGrounded = true;

            if (!gameObject.transform.parent)
            {
				if ( Physics2D.Raycast(transform.position + _groundRayCastOffset, Vector2.down, _groundRaycastLength, _groundLayer).transform.GetComponent<PhotonView>().ViewID != null)
                photonView.RPC("RPC_ParentToPlatform", RpcTarget.All, Physics2D.Raycast(transform.position + _groundRayCastOffset, Vector2.down, _groundRaycastLength, _groundLayer).transform.GetComponent<PhotonView>().ViewID);
            	AH.GetComponent<Animator>().SetBool("Jumping", false);

			}
		}
		else{
			AH.isGrounded = false;
            if (gameObject.transform.parent)
            {
                photonView.RPC("RPC_UnParentToPlatform", RpcTarget.All);
            }
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

	public void resetPlayerPosition(Vector3? pos = default(Vector3?))
	{
		if (!pos.HasValue)
		{
			photonView.RPC("RPC_ResetPosition", RpcTarget.All, new Vector3(0f, 0f, 0f));
		}
		else
		{
			photonView.RPC("RPC_ResetPosition", RpcTarget.All, pos);
		}
	}

	[PunRPC]
	private void RPC_ResetPosition(Vector3 resetPos)
	{
		transform.position = resetPos;
	}

	[PunRPC]
	void RPC_GetJailed()
    {
		Debug.Log("Playermovement: get jailed");
		//transform.position = new Vector3(0, 100, 0);
		//transform.position = GameObject.FindGameObjectWithTag("Jail").GetComponent<JailSystem>().JailObj.transform.position;
		GameObject.FindGameObjectWithTag("Jail").GetComponent<JailSystem>().GetJailed(GetComponent<PhotonView>().ViewID, (int)GetComponent<PlayerData>().team);

	}

	[PunRPC]
	void RPC_ParentToPlatform(int id)
	{
		//PhotonView.Find(id).gameObject.transform.parent = gameObject.transform;
		transform.parent = PhotonView.Find(id).gameObject.transform;
	}

	[PunRPC]
	void RPC_UnParentToPlatform()
	{
		//PhotonView.Find(id).gameObject.transform.parent = null;
		transform.parent = null;
	}

	[PunRPC]
	void RPC_SyncParent(int playerID, int platformID)
    {
        PhotonView.Find(playerID).transform.parent = PhotonView.Find(platformID).transform;
    }
}
