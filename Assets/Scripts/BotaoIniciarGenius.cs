using Photon.Pun;
using UnityEngine;

public class BotaoIniciarGenius : MonoBehaviourPun
{
    [SerializeField] private GeniusGame geniusGame;
    private bool jogadorPerto;
    Animator animator;

    private void Start()
    {
        
        animator = GetComponent<Animator>();
        
        // Verificação segura ao iniciar
        if (geniusGame == null)
        {
            Debug.LogError("GeniusGame não atribuído no Inspector!", this);
            enabled = false; // Desativa o script
            return;
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
            if (geniusGame != null && geniusGame.photonView != null)
            {
                if (PhotonNetwork.IsMasterClient)
                {
                    geniusGame.photonView.RPC("IniciarJogo", RpcTarget.AllBuffered);
                }
                else
                {
                    Debug.Log("Apenas o mestre pode iniciar o jogo");
                }
            }
            else
            {
                Debug.LogError("Referências faltando: " +
                               $"GeniusGame: {geniusGame != null}, " +
                               $"PhotonView: {geniusGame?.photonView != null}");
            }
        }
    }
}