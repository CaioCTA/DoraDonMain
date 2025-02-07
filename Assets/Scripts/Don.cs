using System;
using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class Don : PlayerController, IPunObservable
{
    #region Variables
    public typeBtn typeBtn;
    //X - 0.3432861
    //Y - 0.6082628

    private bool _isInWater = false;
    private GameObject _isNearby;
    private float _waterSpeed = 4f;
    private float _normalGravityScale = 1f; // Gravidade normal fora da água
    private float _waterGravityScale = 7f; // Gravidade reduzida dentro da água
    private float _gravityTransitionSpeed = 6f; // Velocidade da transição de gravidade
    private float _jumpOutWater = 7.5f;
    private float _enterWaterSpeedReduction = 5f; // Fator de redução de velocidade ao entrar na água

    private Animator _animDon;

    // Estados para sincronização
    private bool _syncIsWalking;
    private bool _syncIsJumping;
    private bool _syncIsSwimming;

    [SerializeField] private BoxCollider2D _boxCollider;
    [SerializeField] private CapsuleCollider2D _capsuleCollider;


    //private BoxCollider2D _collider; // Referência ao BoxCollider2D
    //private Vector2 _originalColliderSize; // Tamanho original do Collider
    //private Vector2 _originalColliderOffset; // Posição original do Collider
    //private Vector2 _swimmingColliderSize = new Vector2(0.5f, 0.5f); // Tamanho do Collider ao nadar
    //private Vector2 _swimmingColliderOffset = new Vector2(0.5f, 0.5f); // Posição do Collider ao nadar

    #endregion

    #region Unity Methods
    protected override void Start()
    {
        _boxCollider = GetComponent<BoxCollider2D>();
        _capsuleCollider = GetComponent<CapsuleCollider2D>();

        _capsuleCollider.enabled = false;
        _boxCollider.enabled = true;

        _rb = GetComponent<Rigidbody2D>();
        _animDon = GetComponent<Animator>();
       

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
            _namePlayer.text = _nickName;
        }
   }


    protected override void Update()
    {
        base.Update();

        if (photonView.IsMine)
        {
            HandleInput();
        }
        else
        {
            // Para outros jogadores, atualiza o Animator com base nos estados sincronizados
            UpdateAnimator();
        }
    }

    private void HandleInput()
    {
        float moveH = 0f;

        if (!_isInWater)
        {
            if (Input.GetKey(KeyCode.RightArrow))
            {
                moveH = 1 * _moveSpeed;
            }
            if (Input.GetKey(KeyCode.LeftArrow))
            {
                moveH = -1 * _moveSpeed;
            }

            if (Input.GetKeyDown(KeyCode.UpArrow) && isGrounded)
            {
                Pular();
                _syncIsJumping = true;
            }

            _rb.velocity = new Vector2(moveH * _moveSpeed, _rb.velocity.y);
            
            _rb.gravityScale = 1f;
        }
        else
        {
            Nadar();
        }

        // Atualiza os estados locais
        _syncIsWalking = Mathf.Abs(moveH) > 0 && !_isInWater; // Verifica se o jogador está andando
        SetAnimationState("isWalking", _syncIsWalking);
        SetAnimationState("isSwimming", _syncIsSwimming);
        _syncIsSwimming = _isInWater;

        // Inverte a direção do sprite com base na direção do movimento
        if (moveH > 0)
        {
            transform.localScale = new Vector3(2.951194f, 2.951194f, 2.951194f); // Olha para a direita
        }
        else if (moveH < 0)
        {
            transform.localScale = new Vector3(-2.951194f, 2.951194f, 2.951194f); // Olha para a esquerda
        }

        float targetGravityScale = _isInWater ? _waterGravityScale : _normalGravityScale;
        _rb.gravityScale = Mathf.Lerp(_rb.gravityScale, targetGravityScale, _gravityTransitionSpeed);
    }

    private void UpdateAnimator()
    {
        // Atualiza o Animator com base nos estados sincronizados
        _animDon.SetBool("isWalking", _syncIsWalking);
        _animDon.SetBool("isJumping", _syncIsJumping);
        _animDon.SetBool("isSwimming", _syncIsSwimming);
    }
    
    private void SetAnimationState(string parameter, bool value)
    {
        _animDon.SetBool(parameter, value);

        // Envia o estado da animação para outros jogadores
        photonView.RPC("SyncAnimationState", RpcTarget.Others, parameter, value);
    }

    [PunRPC]
    private void SyncAnimationState(string parameter, bool value)
    {
        // Atualiza o estado da animação para outros jogadores
        _animDon.SetBool(parameter, value);
    }

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.IsWriting)
        {
            // Envia os estados locais para outros jogadores
            stream.SendNext(_syncIsWalking);
            stream.SendNext(_syncIsJumping);
            stream.SendNext(_syncIsSwimming);
        }
        else
        {
            // Recebe os estados de outros jogadores
            _syncIsWalking = (bool)stream.ReceiveNext();
            _syncIsJumping = (bool)stream.ReceiveNext();
            _syncIsSwimming = (bool)stream.ReceiveNext();
        }
    }
    #endregion

    #region 2D Methods
    protected void Pular()
    {
        base.Pular();
        SetAnimationState("isJumping", true);
    }

    protected override void OnCollisionEnter2D(Collision2D collision)
    {
        base.OnCollisionEnter2D(collision);

        if (collision.gameObject.CompareTag("Ground"))
        {
            isGrounded = true;
            _syncIsJumping = false; // Desativa a animação de pular ao tocar o chão
            SetAnimationState("isJumping", false);
            SetAnimationState("isSwimming", false);
        }
    }

    protected override void OnCollisionExit2D(Collision2D collision)
    {
        base.OnCollisionExit2D(collision);

        if (collision.gameObject.CompareTag("Ground"))
        {
            isGrounded = false;
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Water"))
        {
            _isInWater = true;
            StartCoroutine("ChangeCollider");


            // Reduz a velocidade vertical ao entrar na água para evitar impacto
            _rb.velocity = new Vector2(_rb.velocity.x, _rb.velocity.y * _enterWaterSpeedReduction);

            // // Aplica uma pequena força para cima para simular flutuação inicial
            _rb.AddForce(Vector2.up * 2f, ForceMode2D.Impulse);

          

        }
    }


    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Water"))
        {
            
            
            _isInWater = false;
            _rb.velocity = new Vector2(_rb.velocity.x, _jumpOutWater);
            StartCoroutine("ChangeColliderOut");


        }
    }
    #endregion

    void Nadar()
    {
        float moveX = Input.GetAxis("Horizontal");
        float moveY = Input.GetAxis("Vertical");

        // Aplica amortecimento ao movimento
        _rb.velocity = new Vector2(moveX * _waterSpeed, moveY * _waterSpeed);

        // Inverte a escala do Transform com base na direção do movimento na água
        if (moveX > 0)
        {
            transform.localScale = new Vector3(2.951194f, 2.951194f, 2.951194f); // Olha para a direita
        }
        else if (moveX < 0)
        {
            transform.localScale = new Vector3(-2.951194f, 2.951194f, 2.951194f); // Olha para a esquerda
        }
        
        SetAnimationState("isSwimming", true);
        
    }

    IEnumerable ChangeCollider()
    {
        Debug.Log("Aloo");
        yield return new WaitForSeconds(1);
        _boxCollider.enabled = false;
        _capsuleCollider.enabled = true;

    }

    IEnumerable ChangeColliderOut()
    {
        yield return new WaitForSeconds(1);
        _boxCollider.enabled = true;       
        _capsuleCollider.enabled = false;

    }

}

