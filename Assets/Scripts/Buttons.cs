using System;
using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;


public enum typeBtn
{
    Dora,Don
}


public class Buttons : MonoBehaviourPunCallbacks
{
    #region Variables

    public typeBtn typeBtn;
    
    private GameObject _isNearby;
    [SerializeField] private GameObject Door;
    private bool _isDoorOpen = false;
    public static bool _botaoAtivado = false;

    #endregion

    #region Unity Methods
    
    private void Update()
    {
        if (_isNearby != null && !_botaoAtivado)
        {
            PhotonView playerPhotonView = _isNearby.GetComponent<PhotonView>();
        
            if (playerPhotonView != null)
            {
                if (typeBtn == typeBtn.Dora && _isNearby.GetComponent<Dora>() != null)
                {
                    if (Input.GetKey(KeyCode.E))
                    {
                        photonView.RPC("AtivarButton", RpcTarget.AllBuffered);

                    }
                }
                else if (typeBtn == typeBtn.Dora && _isNearby.GetComponent<Don>() != null && Input.GetKey(KeyCode.E))
                {
                    Debug.Log("Você nao pode ativar esse botão.");
                }
                
                if (typeBtn == typeBtn.Don && _isNearby.GetComponent<Don>() != null)
                {
                    if (Input.GetKey(KeyCode.E))
                    {
                        photonView.RPC("AtivarButton", RpcTarget.AllBuffered);

                    }
                }
                else if (typeBtn == typeBtn.Don && _isNearby.GetComponent<Dora>() != null && Input.GetKey(KeyCode.E))
                {
                    Debug.Log("Você nao pode ativar esse botão.");
                }
            }
            else
            {
                Debug.LogError("O jogador próximo não tem um PhotonView!");
            }
        }
    }

    #endregion
    
    #region 2D Methods
    
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player")) // Certifique-se de que o jogador tenha a tag correta
        {
            _isNearby = other.gameObject; // Marca o jogador como próximo

            // if (_botaoAtivado)
            // {
            //     Debug.Log("Botão já ativado!");
            // }
            // else
            // {
            //     Debug.Log("Pressione 'E' para ativar o botão.");
            // }

            // Valida se o jogador tem o componente esperado
            if (_isNearby.GetComponent<PhotonView>() == null)
            {
                Debug.LogError("O jogador não possui um PhotonView!");
            }
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        
        // Se o jogador sair do trigger, a referência é removida
        if (other.CompareTag("Player"))
        {
            _isNearby = null;
            // Debug.Log("O jogador saiu da área.");
        }
        
    }

    // this.gameObject: Refere-se ao objeto no qual o script está anexado.
    // other.gameObject: Refere-se ao objeto que entrou no trigger.
    
    #endregion
    
    #region Public Methods
    
    [PunRPC]
    public void AtivarButton()
    {

        PhotonView btnPhotonView = this.GetComponent<PhotonView>();
        if (btnPhotonView != null)
        {
            Debug.Log("Botao Ativado!");
            DestroyDoor();
            _botaoAtivado = true;
        }

    }

    
    public void DestroyDoor()
    {
        _isDoorOpen = true;
        Door.SetActive(false);
    }
    
    #endregion
    
    
}
