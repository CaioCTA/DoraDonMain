using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using TMPro;

public class Loading : MonoBehaviourPunCallbacks
{
    #region Variables
    
    [SerializeField] private TMP_Text loadingText;

    #endregion

    #region Unity Methods

    private void Start()
    {
        loadingText.text = "Carregando...";
        PhotonNetwork.ConnectUsingSettings();
    }

    #endregion
    
    #region Callbacks Photon

    public override void OnConnectedToMaster()
    {
        loadingText.text = "Conectado ao server photon...";
        PhotonNetwork.JoinLobby();
    }

    public override void OnJoinedLobby()
    {
        loadingText.text = ("Conectado ao Lobby...");
        PhotonNetwork.LoadLevel("Menu");
    }

    #endregion
    
    
}
