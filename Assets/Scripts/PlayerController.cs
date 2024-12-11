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
    
    [SerializeField] protected float _moveSpeed = 6.5f;
    [SerializeField] protected float _jumpForce = 3f;
    [SerializeField] protected TMP_Text _namePlayer;

    protected bool isGrounded = false;
    protected bool isDead = false;

    private Vector2 _networkingPosition;
    protected Rigidbody2D _rb;
    protected string _nickName;
    private Vector2 _playerMovement;

    public bool PodeMover { get; private set; }

    private bool _win;
    private int _winners = 0;

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
            var win = PhotonNetwork.LocalPlayer.CustomProperties["Win"];
            _namePlayer.text = _nickName;
            HabilitaMovimentacao(true);
        }
        else
        {
            _namePlayer.text = _nickName;
        }

    }

    public void FixedUpdate()
    {
        if (photonView.IsMine)
        {
            // local
            _rb.velocity = _playerMovement;

        }
        else
        {
            //network
            transform.position = Vector2.Lerp(transform.position, _networkingPosition, Time.deltaTime * 10);
        }
    }


    protected virtual void Update()
    {
        if (photonView.IsMine)
        {
            float moveH = Input.GetAxisRaw("Horizontal");
            float moveV = Input.GetAxisRaw("Vertical");
            _playerMovement = new Vector2(moveH * _moveSpeed, _rb.velocity.y);

            bool jump = Input.GetKeyDown(KeyCode.Space);
            
            if (jump && isGrounded)
            {
                photonView.RPC("Pular", RpcTarget.All);
            }
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
        if (photonView.IsMine)
        {
            GameObject go = other.gameObject;

            if (go.CompareTag("Morte"))
            {
                photonView.RPC("Morte", RpcTarget.All);
            }

            if (go.CompareTag("Final"))
            {
                if (_winners == 2)
                {
                    photonView.RPC("Win", RpcTarget.All);
                }
                else
                {
                    Debug.Log("Precisa de 2 players");
                }
                
            }

        }
    }

    #endregion
    
    #region Player Methods
    [PunRPC]
    public void Pular()
    {
        _rb.velocity = new Vector2(_rb.velocity.x, _jumpForce);
    }

    [PunRPC]
    public void Morte()
    {
        isGrounded = true;
        PhotonNetwork.LoadLevel("GameScene");
    }

    public void Win()
    {


        if ((bool)PhotonNetwork.LocalPlayer.CustomProperties["Win"])
        {
            _winners = (int)PhotonNetwork.LocalPlayer.CustomProperties["Win"];
        }

        var jaGanhou = new ExitGames.Client.Photon.Hashtable();
        jaGanhou.TryAdd("Win", _winners);
        PhotonNetwork.LocalPlayer.SetCustomProperties(jaGanhou);

    }

    public override void OnPlayerPropertiesUpdate(Player targetPlayer, ExitGames.Client.Photon.Hashtable changedProps)
    {
        Debug.Log((bool)PhotonNetwork.LocalPlayer.CustomProperties["Win"]);
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