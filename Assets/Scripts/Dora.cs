using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;

public class Dora : PlayerController
{
    #region Variables

    public typeBtn doraBtn = typeBtn.Dora;
    
    private GameObject _isNearby;
    private bool _isFlying;
    [SerializeField] private float _flyForce = 3f;
    [SerializeField] private float maxFlyTime = 1.5f; // Tempo máximo de voo em segundos
    private float currentFlyTime = 0f; // Variável para controlar o tempo de voo restante

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
            // Alterna entre voar e andar
            if (Input.GetKeyDown(KeyCode.F) && !_isFlying && isGrounded)
            {
                StartFlying();
            }
            else if (Input.GetKeyDown(KeyCode.F) && _isFlying && !isGrounded)
            {
                StopFlying();
            }

            // Se está voando, aplicar a força de voo
            if (_isFlying)
            {
                _rb.velocity = new Vector2(_rb.velocity.x, _flyForce);
                currentFlyTime -= Time.deltaTime;  // Reduz o tempo de voo a cada frame

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

        if (_isFlying)
        {
            isGrounded = false; // Impede que a Dora seja considerada como "no chão" enquanto voando
        }
    }
    
    protected override void OnCollisionExit2D(Collision2D collision)
    {
        base.OnCollisionExit2D(collision);
    }

    #endregion

    #region Fly Methods

    private void StartFlying()
    {
        _isFlying = true;
        currentFlyTime = maxFlyTime; // Inicia o contador de tempo de voo
    }

    private void StopFlying()
    {
        _isFlying = false;
        currentFlyTime = 0f; // Reseta o tempo de voo
    }

    #endregion
}
