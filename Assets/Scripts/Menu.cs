using System;
using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;

public class Menu : MonoBehaviourPunCallbacks
{
    #region Unity Methods

    private void Awake()
    {
        PhotonNetwork.AutomaticallySyncScene = true;
    }
    
    #endregion
    
    #region Public Methods

    public void Play()
    {
        PhotonNetwork.LoadLevel("CreateGame");
        Debug.Log("Load Level");
    }

    #endregion

}
