using System;
using Photon.Pun;
using TMPro;
using UnityEngine;

public class PlayerController : MonoBehaviourPunCallbacks, IPunObservable
{
    #region Variables
    public static PlayerController Instance;
    public static GameObject LocalPlayerInstance;

    [SerializeField] protected float _moveSpeed = 2.8f;
    [SerializeField] protected float _jumpForce = 3.8f;
    [SerializeField] protected TMP_Text _namePlayer;
    protected bool isGrounded = false;
    private Vector2 _networkingPosition;
    protected Rigidbody2D _rb;
    protected string _nickName;

    private bool _syncIsWalking;
    private bool _syncIsJumping;
    private bool _syncIsFlying;
    #endregion

    #region Unity Methods
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
    }

    protected virtual void Start()
    {
        _rb = GetComponent<Rigidbody2D>();
        if (photonView.IsMine)
        {
            if (LocalPlayerInstance == null)
            {
                LocalPlayerInstance = this.gameObject;
            }
            _nickName = PhotonNetwork.LocalPlayer.NickName;
            _namePlayer.text = _nickName;
        }
        else
        {
            _namePlayer.text = photonView.Owner.NickName;
        }
    }

    protected virtual void Update()
    {
        if (photonView.IsMine)
        {
            float moveH = 0f;
            if (Input.GetKey(KeyCode.RightArrow)) moveH = 1 * _moveSpeed;
            if (Input.GetKey(KeyCode.LeftArrow)) moveH = -1 * _moveSpeed;
            _rb.velocity = new Vector2(moveH * _moveSpeed, _rb.velocity.y);

            if (Input.GetKeyDown(KeyCode.UpArrow) && isGrounded)
            {
                Pular();
            }

            _syncIsWalking = Mathf.Abs(moveH) > 0;
        }
    }

    private void FixedUpdate()
    {
        if (!photonView.IsMine)
        {
            transform.position = Vector2.Lerp(transform.position, _networkingPosition, 0.1f);
        }
    }
    #endregion

    #region Photon Callbacks
    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.IsWriting)
        {
            stream.SendNext(transform.position);
            stream.SendNext(isGrounded);
            stream.SendNext(_rb.gravityScale);
            stream.SendNext(_rb.velocity);
            stream.SendNext(transform.localScale);
            stream.SendNext(_syncIsWalking);
            stream.SendNext(_syncIsJumping);
            stream.SendNext(_syncIsFlying);
        }
        else
        {
            _networkingPosition = (Vector2)stream.ReceiveNext();
            isGrounded = (bool)stream.ReceiveNext();
            _rb.gravityScale = (float)stream.ReceiveNext();
            _rb.velocity = (Vector2)stream.ReceiveNext();
            transform.localScale = (Vector3)stream.ReceiveNext();
            _syncIsWalking = (bool)stream.ReceiveNext();
            _syncIsJumping = (bool)stream.ReceiveNext();
            _syncIsFlying = (bool)stream.ReceiveNext();
        }
    }
    #endregion

    #region Player Methods
    protected void Pular()
    {
        _rb.velocity = new Vector2(_rb.velocity.x, _jumpForce);
    }
    #endregion
}