using System;
using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Menu : MonoBehaviourPunCallbacks
{
    
    public void Play()
    {
        PhotonNetwork.LoadLevel("CreateGame");
        Debug.Log("Load Level");
    }
    
    public void Credits()
    {
        PhotonNetwork.LoadLevel("Credits");
    }
    
    public void Sair()
    {
        Application.Quit();
    }

    public void BackMenu()
    {
        PhotonNetwork.LoadLevel("Menu");
    }
    public void Loja()
    {
        PhotonNetwork.LoadLevel("Loja");
    }
    

}
