using System;
using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;

public class PlayerController : MonoBehaviourPunCallbacks, IPunObservable
{
    
    #region Variables
    public static GameObject LocalPlayerInstance;
    [SerializeField] protected float _moveSpeed = 10f;
    [SerializeField] protected float _jumpForce = 6f;
    [SerializeField] protected TMP_Text _namePlayer;

    protected bool isGrounded = false;

    private Vector2 _networkingPosition;
    protected Rigidbody2D _rb;
    protected string _nickName;
    private Vector2 _playerMovement;

    // private GameObject _itemNearby;
    //
    // private int _localScore; //Pontuacao
    
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
            // var score = PhotonNetwork.LocalPlayer.CustomProperties["Score"];
            _namePlayer.text = _nickName;
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

            bool jump = Input.GetButtonDown("Jump");

            if (jump && isGrounded)
            {
                Pular();
            }
        }
        
        
        // if (Input.GetKeyDown(KeyCode.E) && _itemNearby != null)
        // {
        //     PickUpItem(_itemNearby);
        // }
        

    }
    #endregion

    #region 2D Methods

    protected virtual void OnCollisionEnter2D(Collision2D collision)
    {
        
        GameObject go = collision.gameObject;
        
        if (go.CompareTag("Ground") || go.CompareTag("Player"))
        {
            isGrounded = true;
            //Debug.Log("chao");
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

    private void OnTriggerEnter2D(Collider2D collision)
    {
        // GameObject go = collision.gameObject;
        //
        // if (go.CompareTag("Button"))
        // {
        //     _itemNearby = go.gameObject;
        //     Debug.Log("Aperte 'E' para pressionar o Botao!");
        // }
        
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        
        // GameObject go = collision.gameObject;
        //
        // if (go.CompareTag("Button"))
        // {
        //     _itemNearby = null;
        // }
    }
    

    #endregion
    
    #region Player Methods
    
    private void Pular()
    {
        _rb.velocity = new Vector2(_rb.velocity.x, _jumpForce);
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
        }
        else
        {
            //Receber dados
            _networkingPosition = (Vector2)stream.ReceiveNext();
            _nickName = (string)stream.ReceiveNext();


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