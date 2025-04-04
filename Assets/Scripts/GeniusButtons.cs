// using UnityEngine;
// using Photon.Pun;
//
// public class GeniusButtons : MonoBehaviourPun
// {
//     public GeniusGame geniusGame;
//     public int indexCor; // 0=Vermelho, 1=Azul, 2=Roxo, 3=Amarelo
//     private bool jogadorPerto;
//     private Animator anim;
//
//     private void Start()
//     {
//         anim = GetComponent<Animator>();
//         
//         // Garante que o componente PhotonView está configurado
//         if (GetComponent<PhotonView>() == null)
//         {
//             gameObject.AddComponent<PhotonView>();
//         }
//     }
//
//     private void OnTriggerEnter2D(Collider2D other)
//     {
//         if (other.CompareTag("Player") && photonView.IsMine)
//         {
//             jogadorPerto = true;
//         }
//     }
//
//     private void OnTriggerExit2D(Collider2D other)
//     {
//         if (other.CompareTag("Player") && photonView.IsMine)
//         {
//             jogadorPerto = false;
//         }
//     }
//
//     private void Update()
//     {
//         if (jogadorPerto && Input.GetKeyDown(KeyCode.E) && photonView.IsMine)
//         {
//             // Sincroniza a animação para todos os jogadores
//             photonView.RPC("PlayButtonAnimation", RpcTarget.All);
//             
//             // Envia o input para o Master Client validar
//             if (geniusGame != null)
//             {
//                 geniusGame.OnPlayerButtonClick(indexCor);
//             }
//         }
//     }
//
//     [PunRPC]
//     private void PlayButtonAnimation()
//     {
//         anim.Play("Apertar"); // Força reinício da animação
//         
//         // Opcional: tocar som localmente
//         // GetComponent<AudioSource>()?.PlayOneShot(clickSound);
//     }
// }

using System.Collections;
using UnityEngine;
using Photon.Pun;

public class GeniusButtons : MonoBehaviourPun
{
    [SerializeField] private GeniusGame geniusGame;
    [SerializeField] private int indexCor; // 0=Vermelho, 1=Azul, 2=Roxo, 3=Amarelo
    [SerializeField] private AudioClip clickSound;
    
    private bool jogadorPerto;
    private Animator anim;
    private AudioSource audioSource;

    [SerializeField] private GameObject image;

    private void Start()
    {
        anim = GetComponent<Animator>();
        audioSource = GetComponent<AudioSource>();
        
        // Verificação segura do PhotonView
        if (photonView == null)
        {
            Debug.LogError("PhotonView não encontrado! Adicione manualmente no Inspector.", this);
            return;
        }
        
        // Configuração recomendada
        photonView.ObservedComponents ??= new System.Collections.Generic.List<Component>();
        if (!photonView.ObservedComponents.Contains(this))
        {
            photonView.ObservedComponents.Add(this);
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            photonView.RPC("SetPlayerProximity", RpcTarget.All, true);
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            photonView.RPC("SetPlayerProximity", RpcTarget.All, false);
        }
    }

    [PunRPC]
    private void SetPlayerProximity(bool state)
    {
        jogadorPerto = state;
        Debug.Log($"Jogador {(state ? "entrou" : "saiu")} do trigger");
        
        if (image != null)
        {
            image.gameObject.SetActive(state);
        }
        else
        {
            Debug.LogWarning("Objeto filho 'Feedback' não encontrado!", this);
        }
    }

    private void Update()
    {
        if (jogadorPerto && Input.GetKeyDown(KeyCode.E))
        {
            photonView.RPC("PressButton", RpcTarget.All);
        }
    }

    [PunRPC]
    private void PressButton()
    {
        PlayButtonAnimation();
        
        if (PhotonNetwork.IsMasterClient && geniusGame != null)
        {
            geniusGame.OnPlayerButtonClick(indexCor);
        }
    }

    private void PlayButtonAnimation()
    {
        anim.Play("Apertar");
        
        if (audioSource != null && clickSound != null)
        {
            audioSource.PlayOneShot(clickSound);
        }
        
        anim.Play("Idle");
    }
    
}