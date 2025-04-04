using UnityEngine;
using Photon.Pun;

public class PlayerRespawn : MonoBehaviour
{
    private Vector2 respawnPosition;
    
    private void Start()
    {
        // Usa a posição inicial do jogador como primeiro respawn
        respawnPosition = transform.position; 
    }

    public void SetRespawnPoint(Vector2 newPosition)
    {
        // Só atualiza se for o jogador local
        if (GetComponent<PhotonView>().IsMine)
        {
            respawnPosition = newPosition;
        }
    }

    public void Respawn()
    {
        if (GetComponent<PhotonView>().IsMine)
        {
            transform.position = respawnPosition;
            // Adicione aqui efeitos de respawn se quiser
        }
    }
}