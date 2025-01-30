using System;
using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using System.Linq;
using Photon.Realtime;

public class PlayerController : MonoBehaviourPunCallbacks, IPunObservable
{

    #region Variables
    public static PlayerController Instance;
    public static GameObject LocalPlayerInstance;
    
    [SerializeField] protected float _moveSpeed = 2.8f;
    [SerializeField] protected float _jumpForce = 3.8f;
    [SerializeField] protected TMP_Text _namePlayer;

    protected bool isGrounded = false;
    protected bool isDead = false;

    private Vector2 _networkingPosition;
    protected Rigidbody2D _rb;
    protected string _nickName;
    private Vector2 _playerMovement;

    public bool PodeMover { get; private set; }

    private bool _winners;

    #endregion


    #region Properties

    public Vector2 Movement { get; set; }
    public float JumpForce => _jumpForce;
    public float MoveSpeed
    {
        get => _moveSpeed;
        set => _moveSpeed = value;
    }

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
            var win = PhotonNetwork.LocalPlayer.CustomProperties["Win"];
            HabilitaMovimentacao(true);
        }
        else
        {
            _namePlayer.text = _nickName;
        }

    }
    
    protected virtual void Update()
    {
        if (photonView.IsMine)
        {
            
            if (ChatManager.Instance.ChatAtivo())
            {
                Movement = Vector2.zero;
                return;
            }
            
            // float moveH = Input.GetAxisRaw("Horizontal");
            float moveH = 0f;

            if (Input.GetKey(KeyCode.RightArrow))
            {
                moveH = 1 * _moveSpeed;
            }

            if (Input.GetKey(KeyCode.LeftArrow))
            {
                moveH = -1 * _moveSpeed;
            }
            
            _rb.velocity = new Vector2(moveH * _moveSpeed, _rb.velocity.y);
            bool jump = Input.GetKeyDown(KeyCode.UpArrow);
            
            if (jump && isGrounded)
            {
                Pular();
            }
            
            Movement = new Vector2(moveH * _moveSpeed, _rb.velocity.y);
            
        }
        
        
        
    }
    
    public void FixedUpdate()
    {
        if (photonView.IsMine)
        {
            // local
            if (PodeMover)
            {
                _rb.velocity = Movement;
            }

        }
        else
        {
            //network
            transform.position = Vector2.Lerp(transform.position, _networkingPosition, Time.deltaTime * 10);
        }
    }
    #endregion

    #region 2D Methods

    protected virtual void OnCollisionEnter2D(Collision2D collision)
    {
        
        GameObject go = collision.gameObject;
        
        if (go.CompareTag("Ground") || go.CompareTag("Player"))
        {
            isGrounded = true;
            Debug.Log("chao");
        }


        if (go.CompareTag("Door"))
        {
            Debug.Log("Precisa apertar os botões para abrir a porta");
        }
        
    }
    
    protected virtual void OnCollisionExit2D(Collision2D collision)
    {
        
        GameObject go = collision.gameObject;
        
        if (go.CompareTag("Ground") || go.CompareTag("Player"))
        {
            isGrounded = false;
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        GameObject go = other.gameObject;

        if (photonView.IsMine)
        {

            if (go.CompareTag("Morte"))
            {
                photonView.RPC("Morte", RpcTarget.AllBuffered);
            }

            if (go.CompareTag("Final"))
            {
                GameManager.Instance.player1 = true;
                GameManager.Instance.CheckWinner();
                
            }

        }
        else
        {
            if (go.CompareTag("Final"))
            {
                GameManager.Instance.player2 = true;
                GameManager.Instance.CheckWinner();
            }
        }
    }
    private void OnTriggerExit2D(Collider2D collision)
    {
        GameObject go = collision.gameObject;

        if (photonView.IsMine)
        {
            if (go.CompareTag("Final"))
            {
                GameManager.Instance.player1 = false;
            }
            else
            {
                GameManager.Instance.player2 = false;
            }
        }

        

    }

    #endregion

    #region Player Methods
    public void Pular()
    {
        _rb.velocity = new Vector2(_rb.velocity.x, _jumpForce);
    }

    [PunRPC]
    public void Morte()
    {
        PhotonNetwork.LoadLevel("GameScene");
        isGrounded = true;
    }

    #endregion


    #region Public Methods

    public void HabilitaMovimentacao(bool mover)
    {
        PodeMover = mover;
    }

    #endregion


    #region Photon callbacks

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if(stream.IsWriting)
        {
            // Enviar dados
            stream.SendNext((Vector2)transform.position);
            stream.SendNext(_nickName);
            stream.SendNext(isGrounded);
        }
        else
        {
            //Receber dados
            _networkingPosition = (Vector2)stream.ReceiveNext();
            _nickName = (string)stream.ReceiveNext();
            isGrounded = (bool)stream.ReceiveNext();


            if (photonView.IsMine)
            {
                _nickName = PhotonNetwork.LocalPlayer.NickName;
                _namePlayer.text = _nickName;
            }
            else
            {
                _namePlayer.text = _nickName;
            }
        }


    }


    #endregion
    

}