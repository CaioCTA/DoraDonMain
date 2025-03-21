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

    public static GameObject player1Obj;
    public static GameObject player2Obj;

    public bool player1 = false;
    public bool player2 = false;

    public void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
    }

    private void Update()
    {
        CheckWinner();
    }


    void Start()
    {

        if (player1Obj == null && player2Obj == null)
        {
            if (PhotonNetwork.IsMasterClient)
            {
                player1Obj = PhotonNetwork.Instantiate("Dora", player1SpawnerPosition.position, Quaternion.identity);
            }
            else
            {
                player2Obj = PhotonNetwork.Instantiate("Don", player2SpawnerPosition.position, Quaternion.identity);
            }
            
        }

    }

    public static void PlayerDeath()
    {
        if (player1Obj == null || player2Obj == null)
        {
            PhotonNetwork.LoadLevel("GameScene");
        }
    }


    [PunRPC]
    public virtual void CheckWinner()
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