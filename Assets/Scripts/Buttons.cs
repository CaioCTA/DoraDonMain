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

    protected typeBtn typeBtn;

    private GameObject _isNearby;
    [SerializeField] private GameObject Door;
    [SerializeField] private GameObject Button1;
    private bool _botaoAtivado = false;

    #endregion

    #region Unity Methods

    private void Update()
    {
        if (Input.GetKey(KeyCode.E) && _isNearby != null && _botaoAtivado == false)
        {
            AtivarButton();
        }
    }

    #endregion
    
    #region 2D Methods
    private void OnTriggerEnter2D(Collider2D other)
    {
        
        if (other.CompareTag("Player")) // Certifique-se de que os jogadores têm a tag "Player"
        {
            _isNearby = other.gameObject; // Marca o jogador como estando próximo

            if (_botaoAtivado == true)
            {
                Debug.Log("Botao ja ativado!");
            }
            else
            {
                Debug.Log("Pressione 'E' para ativar o botão.");
            }
        }
        
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        
        // Se o jogador sair do trigger, a referência é removida
        if (other.CompareTag("Player"))
        {
            _isNearby = null;
            Debug.Log("O jogador saiu da área.");
        }
        
    }

    // this.gameObject: Refere-se ao objeto no qual o script está anexado.
    // other.gameObject: Refere-se ao objeto que entrou no trigger.
    
    #endregion
    
    #region Public Methods
    //[PunRPC]
    //public void AtivarButton(GameObject btn)
    //{

    //    PhotonView btnPhotonView = btn.GetComponent<PhotonView>();
    //    if (btnPhotonView != null)
    //    {
    //        Debug.Log("Botao Ativado!");
    //        photonView.RPC("DestroyDoor", RpcTarget.MasterClient);
    //        _botaoAtivado = true;
    //    }

    //}

    [PunRPC]
    protected void AtivarButton()
    {

        PhotonView btnPhotonView = this.GetComponent<PhotonView>();
        if (btnPhotonView != null)
        {
            Debug.Log("Botao Ativado!");
            photonView.RPC("DestroyDoor", RpcTarget.MasterClient);
            _botaoAtivado = true;
        }

    }


    [PunRPC]
    public void DestroyDoor()
    {
        Door.SetActive(false);
    }
    
    #endregion
    
    
    
}
