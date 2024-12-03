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
    [SerializeField] private float _moveSpeed = 10f;
    [SerializeField] private float _jumpForce = 6f;
    [SerializeField] private TMP_Text _namePlayer;

    private bool isGrounded = false;

    private Vector2 _networkingPosition;
    private Rigidbody2D _rb;
    private string _nickName;
    private Vector2 _playerMovement;

    private GameObject _itemNearby;

    private int _localScore; //Pontuacao
    
    #endregion

    #region Unity Methods
    void Start()
    {
        _rb = GetComponent<Rigidbody2D>();
        if (photonView.IsMine)
        {
            if (LocalPlayerInstance == null)
            {
                LocalPlayerInstance = this.gameObject;

            }
            _nickName = PhotonNetwork.LocalPlayer.NickName;
            var score = PhotonNetwork.LocalPlayer.CustomProperties["Score"];
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


    private void Update()
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
        
        
        if (Input.GetKeyDown(KeyCode.E) && _itemNearby != null)
        {
            PickUpItem(_itemNearby);
        }
        

    }
    #endregion

    #region 2D Methods

    public void OnCollisionEnter2D(Collision2D collision)
    {
        
        GameObject go = collision.gameObject;
        
        if (go.CompareTag("Ground"))
        {
            isGrounded = true;
            //Debug.Log("chao");
        }

        if (go.CompareTag("Door"))
        {
            Debug.Log("Precisa apertar os botões para abrir a porta");
        }
        
    }
    
    private void OnCollisionExit2D(Collision2D collision)
    {
        
        GameObject go = collision.gameObject;
        
        if (go.CompareTag("Ground"))
        {
            isGrounded = false;
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        GameObject go = collision.gameObject;

        if (go.CompareTag("Button"))
        {
            _itemNearby = go.gameObject;
            Debug.Log("Aperte 'E' para pressionar o Botao!");
        }
        
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        
        GameObject go = collision.gameObject;
        
        if (go.CompareTag("Button"))
        {
            _itemNearby = null;
        }
    }
    

    #endregion
    
    #region Player Methods
    
    private void Pular()
    {
        _rb.velocity = new Vector2(_rb.velocity.x, _jumpForce);
    }
    
    private void PickUpItem(GameObject item)
    {
        // Garantir que o item tenha um PhotonView
        PhotonView itemPhotonView = item.GetComponent<PhotonView>();
        if (itemPhotonView != null)
        {
            // Envia o ViewID do item para todos os jogadores via RPC
            photonView.RPC("DestroyItem", RpcTarget.AllBuffered, itemPhotonView.ViewID);
        }
    }
    
    [PunRPC]
    private void DestroyItem(int itemViewID)
    {
        // Encontrar o PhotonView do item pelo ViewID
        PhotonView itemPhotonView = PhotonView.Find(itemViewID);
        if (itemPhotonView != null)
        {
            // Destroy(itemPhotonView.gameObject);
            GameObject button = GameObject.FindWithTag("Button");
            button.GetComponent<SpriteRenderer>().color = Color.green;
            Debug.Log("Botao pressionado!");
            
            
            // Destrói o item em todos os clientes
        }
    }

    // private void ButtonDoor()
    // {
    //     
    // }
    //
    // private void OpenDoor(int itemViewID)
    // {
    //     PhotonView itemPhotonView = PhotonView.Find(itemViewID);
    //     if (itemPhotonView != null)
    //     {
    //         Destroy(itemPhotonView.gameObject);
    //     }
    // }

    #endregion

    #region Public Methods
    
    //public void UpdateScore(int quantidade)
    //{
    //    int scoreAtual = 0;
    //    if (PhotonNetwork.LocalPlayer.CustomProperties.ContainsKey("Score"))
    //    {
    //        scoreAtual = (int)PhotonNetwork.LocalPlayer.CustomProperties["Score"];
    //    }

    //    scoreAtual += quantidade;

    //    // Atualizar essa pontuacao nas propriedades customizaveis do jogador.

    //    var tabela = new ExitGames.Client.Photon.Hashtable();
    //    tabela.TryAdd("Score", scoreAtual);
    //    PhotonNetwork.LocalPlayer.SetCustomProperties(tabela);

    //}

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