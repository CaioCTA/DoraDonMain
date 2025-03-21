using System;
using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using Photon.Pun.UtilityScripts;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
public class Dora : MonoBehaviourPunCallbacks, IPunObservable
{
    public static Dora DoraInstance;
    public static GameObject LocalDoraInstance;
    
    [SerializeField] private float _movSpeed = 3500f;
    [SerializeField] private float _jumpForce = 3000f;
    private bool isGrounded;
    //Voo
    private bool _isFlying;
    [SerializeField] private float maxFlyTime = 2f; // Tempo máximo de voo em segundos
    [SerializeField] private float _flySpeed = 3500f;
    private float currentFlyTime = 0f; // Variável para controlar o tempo de voo restante
    [SerializeField] private int _flyQuant = 1;
    public TMP_Text flyTime_Text;

    private Rigidbody2D _rb;
    private Animator _anim;
    
    //BoxColliderFlying
    private CapsuleCollider2D _capsuleCollider2D;
    
    private Vector2 _originalBoxColliderSize;
    private Vector2 _originalBoxColliderOffset;

    //Sync
    private float lastUpdate;
    private Vector2 latestPosition;
    private Quaternion latestRotation;



    private void Start()
    {

        _rb = GetComponent<Rigidbody2D>();
        _anim = GetComponent<Animator>();
        _capsuleCollider2D = GetComponent<CapsuleCollider2D>();
        
        _originalBoxColliderSize = _capsuleCollider2D.size;
        _originalBoxColliderOffset = _capsuleCollider2D.offset;
        
        lastUpdate = Time.time;
        latestPosition = transform.position;
        latestRotation = transform.rotation;
        
        
        if (!GetComponent<PhotonView>().IsMine)
        {
            _rb.simulated = false; // Desativa a física para jogadores remotos
        }
        else
        {
            Debug.Log(photonView.ViewID);
        }
    }

    private void Update()
    {
        if (PhotonNetwork.IsMasterClient)
        {
            if (GameManager.Instance.player1 && GameManager.Instance.player2)
            {
                photonView.RPC("CheckWinner", RpcTarget.AllBuffered);
            }
        }
        else
        {
            if (GameManager.Instance.player1 && GameManager.Instance.player2)
            {
                photonView.RPC("CheckWinner", RpcTarget.AllBuffered);
            }
        }
        
        MovePlayer();
        
    }

    private void FixedUpdate()
    {
        if (photonView.IsMine)
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
        if (GetComponent<PhotonView>().IsMine)
        {
            float moveH = Input.GetAxis("Horizontal");

            if (!_isFlying)
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
                _rb.velocity = new Vector2(moveX * _flySpeed, moveY * _flySpeed) * Time.deltaTime;
                

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
             _anim.SetBool("isWalking", isWalking); // Atualiza o parâmetro "isWalking" no Animator

             _anim.SetBool("isFlying", _isFlying);
           
             if (moveH > 0)
             {
                 transform.localScale = new Vector3(Mathf.Abs(transform.localScale.x), transform.localScale.y, transform.localScale.z); // Direita
             }
             else if (moveH < 0)
             {
                 transform.localScale = new Vector3(-Mathf.Abs(transform.localScale.x), transform.localScale.y, transform.localScale.z); // Esquerda
             }
            
            //Alterna entre voar e andar
            if (Input.GetKeyDown(KeyCode.Space) && !_isFlying && !isGrounded && _flyQuant == 1)
            {
                StartFlying();
                ChangeCollider();
            }

            if (_isFlying)
            {
                currentFlyTime -= Time.deltaTime;
                string v = currentFlyTime.ToString("00:00");
                flyTime_Text.text = $"Timer: {v}";

                if (currentFlyTime <= 0)
                {
                    StopFlying();
                    ChangeColliderOut();
                    flyTime_Text.text = "";
                }
                
            }
            
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
    
    [PunRPC]
    private void Death()
    {
        //PhotonNetwork.Destroy(gameObject);

        PhotonNetwork.Destroy(GameManager.player1Obj);
        GameManager.PlayerDeath();

        
        
        
    }
    
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Ground"))
        {
            isGrounded = true;
            _flyQuant = 1; // Restaura a quantidade de voo
            _anim.SetBool("isJumping", false);
        }

        if (collision.gameObject.CompareTag("Espinhos"))
        {
            photonView.RPC("Death", RpcTarget.AllBuffered);
        }
            
    }
    private void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Ground"))
        {
            isGrounded = false;
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Water"))
        {
            //Courotine de morte
            photonView.RPC("Death", RpcTarget.AllBuffered);
        }

        if (collision.gameObject.CompareTag("Final"))
        {
            if (PhotonNetwork.IsMasterClient)
            {
                GameManager.Instance.player1 = true;
                Debug.Log("Sou o player1");
            }
            else
            {
                GameManager.Instance.player2 = true;
                Debug.Log("Sou o player2");
            }
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("Final"))
        {
            if (PhotonNetwork.IsMasterClient)
            {
                GameManager.Instance.player1 = true;
                Debug.Log("Saiu o player1");
            }
            else
            {
                GameManager.Instance.player2 = true;
                Debug.Log("Saiu o player2");
            }
        }
    }
    

    //para fazer com que o outro player reaja a morte do outro, usar o if(photonView.IsMine) e ter um bool public(?) para verificar a morte.

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
             

             // // Ativa o texto do timer
             // if (_flyTextTimer != null)
             // {
             //     _flyTextTimer.text = $"Fly Time: {currentFlyTime.ToString("F1")}";
             // }
             //
             // // Ajusta o Collider para a orientação de voo
             // if (_collider != null)
             // {
             //     _collider.size = _flyingColliderSize;
             //     _collider.offset = _flyingColliderOffset;
             // }
             //
             // // Sincroniza o estado de voo
             // SetAnimationState("isFlying", true);
         }
     }

     private void StopFlying()
     {
         
         if (_isFlying)
         {
             _isFlying = false;
             _rb.gravityScale = 1f; // Reativa a gravidade
             _anim.SetBool("isFlying", false);
             


             // // Oculta o texto do timer
             // if (_flyTextTimer != null)
             // {
             //     _flyTextTimer.text = "";
             // }
             //
             // // Restaura o Collider para a orientação original
             // if (_collider != null)
             // {
             //     _collider.size = _originalColliderSize;
             //     _collider.offset = _originalColliderOffset;
             // }
             //
             // // Sincroniza o estado de voo
             // SetAnimationState("isFlying", false);
         }
     }
     #endregion
     
     //IEnumerator ChangeCollider()
     //{
     //    yield return new WaitForSeconds(0.2f);
     //    _boxCollider2D.size = new Vector2(0.6485293f, 0.4182276f);
     //    _boxCollider2D.offset = new Vector2(-0.05495566f, -0.02544083f);
     //}

     //IEnumerator ChangeColliderOut()
     //{
     //    yield return new WaitForSeconds(0.1f);
     //    _boxCollider2D.size = _originalBoxColliderSize;
     //    _boxCollider2D.offset = _originalBoxColliderOffset;
     //}

    void ChangeCollider()
    {
        _capsuleCollider2D.size = new Vector2(0.6485293f, 0.4182276f);
        _capsuleCollider2D.offset = new Vector2(-0.05495566f, -0.02544083f);
        _capsuleCollider2D.direction = CapsuleDirection2D.Horizontal;
    }

    void ChangeColliderOut()
    {
        _capsuleCollider2D.size = _originalBoxColliderSize;
        _capsuleCollider2D.offset = _originalBoxColliderOffset;
        _capsuleCollider2D.direction = CapsuleDirection2D.Vertical;
    }

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
     {
         if (stream.IsWriting)
         {
            stream.SendNext(transform.position);
            stream.SendNext(transform.rotation);
            // stream.SendNext(_rb.velocity.y); //Teste pulo
            stream.SendNext(transform.localScale.x > 0 ? 1 : -1);
         }
         else
         {
            latestPosition = (Vector3)stream.ReceiveNext();
            latestRotation = (Quaternion)stream.ReceiveNext();
            // float receivedVelocityY = (float)stream.ReceiveNext();
            int direction = (int)stream.ReceiveNext();


            transform.localScale = new Vector3(
                 direction * Mathf.Abs(transform.localScale.x),
                 transform.localScale.y,
                 transform.localScale.z);

            // _rb.velocity = new Vector2(_rb.velocity.x, receivedVelocityY);
            //Calcula o tempo desde a ultima atualizacao
            lastUpdate = Time.time;
         }
     }
    
}
