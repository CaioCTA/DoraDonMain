using System;
using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using TMPro;
using UnityEngine;

public class Dora : PlayerController
{
    #region Variables

    public typeBtn typeBtn;
    
    private GameObject _isNearby;
    private bool _isFlying;
    // [SerializeField] private float _flyForce = 3f;
    [SerializeField] private float maxFlyTime = 2f; // Tempo máximo de voo em segundos
    [SerializeField] private float _flySpeed = 6.0f;
    private float currentFlyTime = 0f; // Variável para controlar o tempo de voo restante
    [SerializeField] private int _flyQuant = 1;

    [SerializeField] private TMP_Text _flyTextTimer;
    
    #endregion
    
    #region Unity Methods

    protected override void Start()
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
            }
            else
            {
                float moveX = Input.GetAxis("Horizontal");
                float moveY = Input.GetAxis("Vertical");
                _rb.velocity = new Vector2(moveX * _flySpeed, moveY * _flySpeed);
            }
            
            
            // Alterna entre voar e andar
            if (Input.GetKeyDown(KeyCode.UpArrow) && !_isFlying && !isGrounded && _flyQuant == 1)
            {
                StartFlying();
            }
            // else if (Input.GetKeyDown(KeyCode.LeftArrow) && _isFlying && !isGrounded)
            // {
            //     StopFlying();
            // }

            // Se está voando, aplicar a força de voo
            if (_isFlying)
            {
                //_rb.velocity = new Vector2(_rb.velocity.x, _flyForce);
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

    #endregion

    #region 2D Methods

    protected override void OnCollisionEnter2D(Collision2D collision)
    {
        base.OnCollisionEnter2D(collision);

        GameObject go = collision.gameObject;

        if (_isFlying)
        {
            isGrounded = false; // Impede que a Dora seja considerada como "no chão" enquanto voando
        }

        if (go.CompareTag("Ground"))
        {
            _flyQuant = 1;
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

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Water"))
        {
            // photonView.RPC("Morte", RpcTarget.AllBuffered);
            MorteOffline();
        }
    }

    #endregion
}
