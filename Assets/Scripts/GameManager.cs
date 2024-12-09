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
                PhotonNetwork.Instantiate("Dora", player1SpawnerPosition.position, Quaternion.identity);
            }
            else
            {
                PhotonNetwork.Instantiate("Don", player2SpawnerPosition.position, Quaternion.identity);
            }
            
        }

    }

    // public override void OnPlayerLeftRoom(Player otherPlayer)
    // {
    //     base.OnPlayerLeftRoom(otherPlayer);
    // }
}