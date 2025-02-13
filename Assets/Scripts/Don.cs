using System;
using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using Photon.Pun.UtilityScripts;
using UnityEngine;
using UnityEngine.EventSystems;

public class Don : MonoBehaviour, IPunObservable
{
    [SerializeField] private float _movSpeed = 3500f;
    [SerializeField] private float _jumpForce = 2700f;
    private bool isGrounded;
    
    //Nado
    private bool _isSwimming;
    [SerializeField] private float _waterSpeed = 3200f;
    private float _normalGravityScale = 1f; // Gravidade normal fora da água
    private float _waterGravityScale = 7f; // Gravidade reduzida dentro da água
    private float _gravityTransitionSpeed = 6f; // Velocidade da transição de gravidade
    private float _jumpOutWater = 7.5f;
    private float _enterWaterSpeedReduction = 5f; // Fator de redução de velocidade ao entrar na água
    
    
    private Rigidbody2D _rb;
    private Animator _anim;
    
    //Sync
    private float lastUpdate;
    private Vector2 latestPosition;
    private Quaternion latestRotation;

    private void Start()
    {
        _rb = GetComponent<Rigidbody2D>();
        _anim = GetComponent<Animator>();
        
        
        lastUpdate = Time.time;
        latestRotation = transform.rotation;
        latestPosition = transform.position;
        
        if (!GetComponent<PhotonView>().IsMine)
        {
            _rb.simulated = false; // Desativa a física para jogadores remotos
        }
    }

    private void Update()
    {
        if (GetComponent<PhotonView>().IsMine)
        {
            MovePlayer();
        }
        else
        {
            //Suavizar o mov remoto
            SmoothMove();
        }
        
        PhotonNetwork.SerializationRate = 30;
        
    }
    

    void MovePlayer()
    {
        if (GetComponent<PhotonView>())
        {
            float moveH = Input.GetAxis("Horizontal");

            if (!_isSwimming)
            {
                // Movimento horizontal no chão
                _rb.velocity = new Vector2(moveH * _movSpeed * Time.deltaTime, _rb.velocity.y);
                _anim.SetBool("isWalking", true);
                
                //Verifica se o Jogador esta pulando
                if (Input.GetKeyDown(KeyCode.Space) && isGrounded)
                {
                    //_rb.velocity = new Vector2(_rb.velocity.x, _jumpForce * Time.deltaTime);
                    _rb.velocity = new Vector2(_rb.velocity.x, _jumpForce);
                    _anim.SetBool("isJumping", true);
                }
                
            }
            else
            {
                float moveX = Input.GetAxis("Horizontal");
                float moveY = Input.GetAxis("Vertical");
                _rb.velocity = new Vector2(moveX * _waterSpeed, moveY * _waterSpeed) * Time.deltaTime;

                if (moveX > 0)
                {
                    transform.localScale = new Vector3(2.951194f, 2.951194f, 2.951194f); // Olha para a direita
                }
                else if (moveX < 0)
                {
                    transform.localScale = new Vector3(-2.951194f, 2.951194f, 2.951194f); // Olha para a esquerda
                }
            }
            
            bool isWalking = Mathf.Abs(moveH) > 0 && !_isSwimming; // Verifica se o jogador está andando fora do voo
            _anim.SetBool("isWalking", isWalking); // Atualiza o parâmetro "isWalking" no Animator
            
            _anim.SetBool("isSwimming", _isSwimming);
            
            //Inverte a direção do sprite com base na direção do movimento
            if (moveH > 0)
            {
                transform.localScale = new Vector3(2.951194f, 2.951194f, 2.951194f); // Olha para a direita
            }
            else if (moveH < 0)
            {
                transform.localScale = new Vector3(-2.951194f, 2.951194f, 2.951194f); // Olha para a esquerda
            }

            float targetGravityScale = _isSwimming ? _waterGravityScale : _normalGravityScale;
            _rb.gravityScale = Mathf.Lerp(_rb.gravityScale, targetGravityScale, _gravityTransitionSpeed);
            
        }
    }
    
    private void SmoothMove()
    {
        if (latestPosition != null)
        {
            transform.position = Vector2.Lerp(transform.position, latestPosition, Time.deltaTime * 5f);
        }

        if (latestRotation != null)
        {
            transform.rotation = Quaternion.Lerp(transform.rotation, latestRotation, Time.deltaTime * 5f);
        }
        
    }
    
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Ground"))
        {
            isGrounded = true;
            _anim.SetBool("isJumping", false);
        }
    }
    
    private void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Ground"))
        {
            isGrounded = false;
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
     {
         if (other.CompareTag("Water"))
         {
             _isSwimming = true;
             // StartCoroutine(ChangeCollider());
             // Reduz a velocidade vertical ao entrar na água para evitar impacto
             _rb.velocity = new Vector2(_rb.velocity.x, _rb.velocity.y * _enterWaterSpeedReduction);
             // Aplica uma pequena força para cima para simular flutuação inicial
             _rb.AddForce(Vector2.up * 2f, ForceMode2D.Impulse);

             Debug.Log("To na agua");

         }
     }

     private void OnTriggerExit2D(Collider2D other)
     {
         if (other.CompareTag("Water"))
         {

             _isSwimming = false;
             _rb.velocity = new Vector2(_rb.velocity.x, _jumpOutWater);
             // StartCoroutine(ChangeColliderOut());


             Debug.Log("Sai da agua");


         }
     }

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.IsWriting)
        {
            stream.SendNext(transform.position);
            stream.SendNext(transform.rotation);
            stream.SendNext(_rb.velocity.y); //Teste pulo
            stream.SendNext(transform.localScale.x > 0 ? 1 : -1);
        }
        else
        {
            latestPosition = (Vector3)stream.ReceiveNext();
            latestRotation = (Quaternion)stream.ReceiveNext();
            float receivedVelocityY = (float)stream.ReceiveNext();
            int direction = (int)stream.ReceiveNext();
            

            transform.localScale = new Vector3(
                direction * Mathf.Abs(transform.localScale.x),
                transform.localScale.y,
                transform.localScale.z);

            _rb.velocity = new Vector2(_rb.velocity.x, receivedVelocityY);
            //Calcula o tempo desde a ultima atualizacao
            lastUpdate = Time.time;
        }
    }
}
