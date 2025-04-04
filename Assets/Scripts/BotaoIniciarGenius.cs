using Photon.Pun;
using UnityEngine;

public class BotaoIniciarGenius : MonoBehaviourPun
{
    [SerializeField] private GeniusGame geniusGame;
    private bool jogadorPerto;

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
            if (geniusGame != null)
            {
                // Só o mestre pode iniciar o jogo
                if (PhotonNetwork.IsMasterClient)
                {
                    geniusGame.photonView.RPC("IniciarJogo", RpcTarget.AllBuffered);
                }
                else
                {
                    Debug.Log("Aguardando mestre iniciar o jogo...");
                }
            }
            else
            {
                Debug.LogError("GeniusGame não atribuído!", this);
            }
        }
    }
}