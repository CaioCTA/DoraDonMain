using System;
using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviourPunCallbacks
{

    public static GameManager Instance;
    [SerializeField] GameObject player1Prefab;
    [SerializeField] GameObject player2Prefab;
    [SerializeField] Transform player1SpawnerPosition;
    [SerializeField] Transform player2SpawnerPosition;

    public bool player1 = false;
    public bool player2 = false;

    public void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
    }

    
    void Start()
    {

        if (PlayerController.LocalPlayerInstance == null)
        {
            if (PhotonNetwork.IsMasterClient)
            {
                PhotonNetwork.Instantiate("Don", player1SpawnerPosition.position, Quaternion.identity);
            }
            else
            {
                PhotonNetwork.Instantiate("Dora", player2SpawnerPosition.position, Quaternion.identity);
            }
            
        }

    }

    private void Update()
    {

        if (player1)
        {
            Debug.Log("Player 1 is active");
        }

        if (player2)
        {
            Debug.Log("Player 2 is active"); 
        }
        
        photonView.RPC("CheckWinner", RpcTarget.AllBuffered);
    }

    [PunRPC]
    public void CheckWinner()
    {
        
        if (player1 && player2)
        {
            if (PhotonNetwork.IsMasterClient)
            {
                PhotonNetwork.LoadLevel("Win");
            }
        }
    }

}