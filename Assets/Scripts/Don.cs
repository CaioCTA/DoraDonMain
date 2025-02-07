using System;
using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class Don : PlayerController
{
    #region Variables

    public typeBtn typeBtn;
    

    private bool _isInWater =  false;
    private GameObject _isNearby;
    private float _waterSpeed = 4f;
    private float _normalGravityScale = 1f; // Gravidade normal fora da água
    private float _waterGravityScale = 7f; // Gravidade reduzida dentro da água
    private float _gravityTransitionSpeed = 6f; // Velocidade da transição de gravidade
    private float _jumpOutWater = 7.5f;
    private float _enterWaterSpeedReduction = 5f; // Fator de redução de velocidade ao entrar na ág
    
    private Animator _animDon;
    
    #endregion
    
    #region Unity Methods

    protected override void Start()
    {
        
        _rb = GetComponent<Rigidbody2D>();
        _animDon = GetComponent<Animator>();
        
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

    protected override void Update()
    {
        base.Update();


        if (photonView.IsMine)
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

                _rb.velocity = new Vector2(moveH * _moveSpeed, _rb.velocity.y);
                _rb.gravityScale = 1f;
            }
            else
            {
                Nadar();
            }
            
            
            bool isWalking = Mathf.Abs(moveH) > 0 && !_isInWater; // Verifica se o jogador está se movendo horizontalmente
            _animDon.SetBool("isWalking", isWalking); // Atualiza o parâmetro "isWalking" no Animator
            _animDon.SetBool("isSwimming", _isInWater);
            // Inverte a direção do sprite com base na direção do movimento
            if (moveH > 0)
            {
                // Olha para a direita
                transform.localScale = new Vector3(2.951194f, 2.951194f, 2.951194f);
            }
            else if (moveH < 0)
            {
                // Olha para a esquerda
                transform.localScale = new Vector3(-2.951194f, 2.951194f, 2.951194f);
            }
            
            float targetGravityScale = _isInWater ? _waterGravityScale : _normalGravityScale;
            _rb.gravityScale = Mathf.Lerp(_rb.gravityScale, targetGravityScale, _gravityTransitionSpeed);
            
            
        }
    }

    #endregion

    #region 2D Methods

    protected override void OnCollisionEnter2D(Collision2D collision)
    {
        GameObject don = collision.gameObject;
        
        base.OnCollisionEnter2D(collision);
    }
    
    protected override void OnCollisionExit2D(Collision2D collision)
    {
        GameObject don = collision.gameObject;
        
        base.OnCollisionExit2D(collision);
        
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        GameObject don = other.gameObject;

        if (don.CompareTag("Water"))
        {
            _isInWater = true;
            
            // Reduz a velocidade vertical ao entrar na água para evitar impacto
            _rb.velocity = new Vector2(_rb.velocity.x, _rb.velocity.y * _enterWaterSpeedReduction);

            // Aplica uma pequena força para cima para simular flutuação inicial
            _rb.AddForce(Vector2.up * 2f, ForceMode2D.Impulse);
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        GameObject don = other.gameObject;

        if (don.CompareTag("Water"))
        {
            _isInWater = false;
            _rb.velocity = new Vector2(_rb.velocity.x, _jumpOutWater);
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
    }
    
}
