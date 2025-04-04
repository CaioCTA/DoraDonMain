// using System;
// using System.Collections;
// using System.Collections.Generic;
// using UnityEngine;
//
// public class GeniusButtons : MonoBehaviour
// {
//     public GeniusGame geniusGame;
//     public int indexCor; // 0=Vermelho, 1=Azul, 2=Roxo, 3=Amarelo
//     private bool jogadorPerto;
//     private Animator anim;
//     
//
//
//
//     private void Start()
//     {
//         anim = GetComponent<Animator>();
//     }
//
//     private void OnTriggerEnter2D(Collider2D other)
//     {
//         if (other.CompareTag("Player"))
//         {
//             jogadorPerto = true;
//         }
//     }
//
//     private void OnTriggerExit2D(Collider2D other)
//     {
//         if (other.CompareTag("Player"))
//         {
//             jogadorPerto = false;
//         }
//     }
//
//     private void Update()
//     {
//         if (jogadorPerto && Input.GetKeyDown(KeyCode.E))
//         {
//             anim.Play("Apertar"); // Nome da animação de clique
//             geniusGame.OnPlayerButtonClick(indexCor);
//         }
//     }
// }

using UnityEngine;
using Photon.Pun;

public class GeniusButtons : MonoBehaviourPun
{
    public GeniusGame geniusGame;
    public int indexCor; // 0=Vermelho, 1=Azul, 2=Roxo, 3=Amarelo
    private bool jogadorPerto;
    private Animator anim;

    private void Start()
    {
        anim = GetComponent<Animator>();
        
        // Garante que o componente PhotonView está configurado
        if (GetComponent<PhotonView>() == null)
        {
            gameObject.AddComponent<PhotonView>();
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player") && photonView.IsMine)
        {
            jogadorPerto = true;
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player") && photonView.IsMine)
        {
            jogadorPerto = false;
        }
    }

    private void Update()
    {
        if (jogadorPerto && Input.GetKeyDown(KeyCode.E) && photonView.IsMine)
        {
            // Sincroniza a animação para todos os jogadores
            photonView.RPC("PlayButtonAnimation", RpcTarget.All);
            
            // Envia o input para o Master Client validar
            if (geniusGame != null)
            {
                geniusGame.OnPlayerButtonClick(indexCor);
            }
        }
    }

    [PunRPC]
    private void PlayButtonAnimation()
    {
        anim.Play("Apertar"); // Força reinício da animação
        
        // Opcional: tocar som localmente
        // GetComponent<AudioSource>()?.PlayOneShot(clickSound);
    }
}
