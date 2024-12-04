using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;
using Photon.Pun;

public class RoomManager : MonoBehaviourPunCallbacks
{

    public GameObject virtualCam;
    [SerializeField] private Transform _player1;
    [SerializeField] private Transform _player2;


    private void Start()
    {
        foreach (var playerObject in PhotonNetwork.PlayerList)
        {
            if (playerObject == PhotonNetwork.LocalPlayer)
            {
                AssignFollowToLocalPlayer();
                break;
            }
        }
    }
    
    void AssignFollowToLocalPlayer()
    {
        if (PhotonNetwork.LocalPlayer == null)
        {
            Debug.LogError("LocalPlayer não está disponível.");
            return;
        }

        // Tente encontrar o PhotonView do jogador local
        PhotonView localPhotonView = PhotonView.Find(PhotonNetwork.LocalPlayer.ActorNumber);
        
        if (localPhotonView == null)
        {
            Debug.LogError("Não foi possível encontrar o PhotonView do jogador local.");
            return;
        }

        GameObject localPlayerObject = localPhotonView.gameObject;

        if (localPlayerObject != null)
        {
            // Acesse a Cinemachine Virtual Camera
            CinemachineVirtualCamera cinemachineCamera = virtualCam.GetComponent<CinemachineVirtualCamera>();

            if (cinemachineCamera != null)
            {
                // Defina o Follow como o transform do jogador local
                cinemachineCamera.Follow = localPlayerObject.transform;
                // Debug.Log("Virtual Camera agora segue: " + PhotonNetwork.LocalPlayer.NickName);
            }
            else
            {
                Debug.LogError("Componente CinemachineVirtualCamera não encontrado no objeto da câmera virtual.");
            }
        }
        else
        {
            Debug.LogError("Objeto do jogador local não encontrado.");
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        // Verifica se é o jogador, não é um trigger, e pertence ao cliente local
        PhotonView photonView = other.GetComponent<PhotonView>();
        if (other.CompareTag("Player") && !other.isTrigger && photonView != null && photonView.IsMine)
        {
            virtualCam.SetActive(true);
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        // Verifica se é o jogador, não é um trigger, e pertence ao cliente local
        PhotonView photonView = other.GetComponent<PhotonView>();
        if (other.CompareTag("Player") && !other.isTrigger && photonView != null && photonView.IsMine)
        {
            virtualCam.SetActive(false);
        }
    }
}
