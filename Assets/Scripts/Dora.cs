using System;
using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using TMPro;
using UnityEngine;

public class Dora : PlayerController, IPunObservable
{
    #region Variables
    public typeBtn typeBtn;

    private GameObject _isNearby;
    private bool _isFlying;
    [SerializeField] private float maxFlyTime = 2f; // Tempo máximo de voo em segundos
    [SerializeField] private float _flySpeed = 6.0f;
    private float currentFlyTime = 0f; // Variável para controlar o tempo de voo restante
    [SerializeField] private int _flyQuant = 1;
    [SerializeField] private TMP_Text _flyTextTimer;

    private Animator _animDora;
    private bool isGrounded = true; // Variável para verificar se o jogador está no chão
    
    private bool _syncIsWalking;
    private bool _syncIsJumping;
    private bool _syncIsFlying;
    
    private BoxCollider2D _collider; // Referência ao BoxCollider2D
    private Vector2 _originalColliderSize; // Tamanho original do Collider
    private Vector2 _originalColliderOffset; // Posição original do Collider
    private Vector2 _flyingColliderSize = new Vector2(0.5054458f, 0.3472958f); // Tamanho do Collider ao voar
    private Vector2 _flyingColliderOffset = new Vector2(-0.05403954f, -0.04566178f); // Posição do Collider ao voar
    
    #endregion

    #region Unity Methods
    protected override void Start()
    {
        _rb = GetComponent<Rigidbody2D>();
        _animDora = GetComponent<Animator>();
        _collider = GetComponent<BoxCollider2D>();
        
        if (_collider != null)
        {
            _originalColliderSize = _collider.size;
            _originalColliderOffset = _collider.offset;
        }

        if (photonView.IsMine)
        {
            if (LocalPlayerInstance == null)
            {
                LocalPlayerInstance = this.gameObject;
            }
            _nickName = PhotonNetwork.LocalPlayer.NickName;
            _namePlayer.text = _nickName;

            FindFlyTimerText();
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
            // Para outros jogadores, apenas atualiza o Animator com base nos estados sincronizados
            UpdateAnimator();
        }
        
    }

    private void HandleInput()
    {
        if (photonView.IsMine)
        {
            float moveH = 0f;

            if (!_isFlying)
            {
                if (Input.GetKey(KeyCode.RightArrow))
                {
                    moveH = 1 * _moveSpeed;
                }
                if (Input.GetKey(KeyCode.LeftArrow))
                {
                    moveH = -1 * _moveSpeed;
                }

                _rb.velocity = new Vector2(moveH * _moveSpeed, _rb.velocity.y);

                // Verifica se o jogador está pulando
                if (Input.GetKeyDown(KeyCode.UpArrow) && isGrounded)
                {
                    Pular();
                    _animDora.SetBool("isJumping", true);
                }
            }
            else
            {
                float moveX = Input.GetAxis("Horizontal");
                float moveY = Input.GetAxis("Vertical");
                _rb.velocity = new Vector2(moveX * _flySpeed, moveY * _flySpeed);
                
                if (moveX > 0)
                {
                    transform.localScale = new Vector3(2.951194f, 2.951194f, 2.951194f); // Olha para a direita
                }
                else if (moveX < 0)
                {
                    transform.localScale = new Vector3(-2.951194f, 2.951194f, 2.951194f); // Olha para a esquerda
                }
            }

            // Atualiza o estado da animação de andar
            bool isWalking = Mathf.Abs(moveH) > 0 && !_isFlying; // Verifica se o jogador está andando fora do voo
            _animDora.SetBool("isWalking", isWalking); // Atualiza o parâmetro "isWalking" no Animator

            // Atualiza o estado da animação de voar
            _animDora.SetBool("isFlying", _isFlying);

            // Inverte a direção do jogador com base na direção do movimento
            if (moveH > 0)
            {
                transform.localScale = new Vector3(2.951194f, 2.951194f, 2.951194f); // Olha para a direita
            }
            else if (moveH < 0)
            {
                transform.localScale = new Vector3(-2.951194f, 2.951194f, 2.951194f); // Olha para a esquerda
            }

            // Alterna entre voar e andar
            if (Input.GetKeyDown(KeyCode.UpArrow) && !_isFlying && !isGrounded && _flyQuant == 1)
            {
                StartFlying();
            }

            // Se está voando, aplicar a força de voo
            if (_isFlying)
            {
                currentFlyTime -= Time.deltaTime; // Reduz o tempo de voo a cada frame

                if (_flyTextTimer != null)
                {
                    _flyTextTimer.text = $"Fly Time: {currentFlyTime.ToString("F1")}";
                }

                // Se o tempo de voo terminar, parar o voo
                if (currentFlyTime <= 0)
                {
                    StopFlying();
                }
            }
        }
    }

    
    private void UpdateAnimator()
    {
        // Atualiza o Animator com base nos estados sincronizados
        _animDora.SetBool("isWalking", _syncIsWalking);
        _animDora.SetBool("isFlying", _syncIsFlying);
        _animDora.SetBool("isJumping", _syncIsJumping);
    }
    
    private void SetAnimationState(string parameter, bool value)
    {
        _animDora.SetBool(parameter, value);

        // Envia o estado da animação para outros jogadores
        photonView.RPC("SyncAnimationState", RpcTarget.Others, parameter, value);
    }

    [PunRPC]
    private void SyncAnimationState(string parameter, bool value)
    {
        // Atualiza o estado da animação para outros jogadores
        _animDora.SetBool(parameter, value);
    }
    
    #endregion

    #region 2D Methods
    protected override void OnCollisionEnter2D(Collision2D collision)
    {
        base.OnCollisionEnter2D(collision);
        GameObject go = collision.gameObject;

        if (go.CompareTag("Ground"))
        {
            isGrounded = true;
            _flyQuant = 1; // Restaura a quantidade de voos ao tocar o chão
            SetAnimationState("isJumping", false); // Desativa a animação de pular
        }
    }

    protected override void OnCollisionExit2D(Collision2D collision)
    {
        base.OnCollisionExit2D(collision);
        if (collision.gameObject.CompareTag("Ground"))
        {
            isGrounded = false; // Indica que o jogador não está mais no chão
        }
    }
    #endregion

    #region Fly Methods
    private void StartFlying()
    {
        if (!_isFlying)
        {
            _isFlying = true;
            currentFlyTime = maxFlyTime;
            _rb.gravityScale = 0f; // Desativa a gravidade
            _rb.velocity = Vector2.zero; // Reseta a velocidade
            _flyQuant--;
            
            
            // Ativa o texto do timer
            if (_flyTextTimer != null)
            {
                _flyTextTimer.text = $"Fly Time: {currentFlyTime.ToString("F1")}";
            }
            
            // Ajusta o Collider para a orientação de voo
            if (_collider != null)
            {
                _collider.size = _flyingColliderSize;
                _collider.offset = _flyingColliderOffset;
            }

            // Sincroniza o estado de voo
            SetAnimationState("isFlying", true);
        }
    }

    private void StopFlying()
    {
        if (_isFlying)
        {
            _isFlying = false;
            _rb.gravityScale = 1f; // Reativa a gravidade
            

            // Oculta o texto do timer
            if (_flyTextTimer != null)
            {
                _flyTextTimer.text = "";
            }
            
            // Restaura o Collider para a orientação original
            if (_collider != null)
            {
                _collider.size = _originalColliderSize;
                _collider.offset = _originalColliderOffset;
            }

            // Sincroniza o estado de voo
            SetAnimationState("isFlying", false);
        }
    }

    private void FindFlyTimerText()
    {
        GameObject timerTextObj = GameObject.Find("FlyTimerText"); // Procura o objeto pelo nome
        if (timerTextObj != null)
        {
            _flyTextTimer = timerTextObj.GetComponent<TextMeshProUGUI>();
            if (_flyTextTimer == null)
            {
                Debug.LogError("FlyTimerText não possui um componente TextMeshProUGUI!");
            }
        }
        else
        {
            Debug.LogError("FlyTimerText não encontrado na cena!");
        }
    }

    protected void Pular()
    {
        base.Pular();
        _animDora.SetBool("isJumping", true); // Ativa a animação de pular
    }
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Water"))
        {
            photonView.RPC("Morte", RpcTarget.AllBuffered);
        }
    }
    #endregion

    #region Photon Serialize

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.IsWriting)
        {
            // Envia os estados locais para outros jogadores
            stream.SendNext(_syncIsWalking);
            stream.SendNext(_syncIsJumping);
            stream.SendNext(_syncIsFlying);
        }
        else
        {
            // Recebe os estados de outros jogadores
            _syncIsWalking = (bool)stream.ReceiveNext();
            _syncIsJumping = (bool)stream.ReceiveNext();
            _syncIsFlying = (bool)stream.ReceiveNext();
        }
    }

    #endregion
    
}
