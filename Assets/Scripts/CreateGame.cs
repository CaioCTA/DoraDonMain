using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Photon.Pun;
using UnityEngine.UI;
using Photon.Realtime;
using UnityEngine.UIElements;
using System;
using Random = UnityEngine.Random;

public class CreateGame : MonoBehaviourPunCallbacks
{
    
    #region Variables
    
    [SerializeField] private TMP_InputField _nickName;
    [SerializeField] private TMP_InputField _roomID;
    private RoomOptions _roomOptions = new RoomOptions();
    
    #endregion
    
    #region Unity Methods

    private void Awake()
    {
        PhotonNetwork.AutomaticallySyncScene = true;
    }

    private void Start()
    {
        _roomOptions.MaxPlayers = 2;
        _roomOptions.IsVisible = true;
        _roomOptions.IsOpen = true;
    }

    #endregion
    
    
    #region Public Methods

    public string GeraID()
    {
        const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
        string code = "";
        int digitCount = 6;

        for (int i = 0; i < digitCount; i++)
        {
            code += chars[Random.Range(0, chars.Length)];
        }
        Debug.Log(code);
        return code;
    }

    public void CriarSala()
    {

        if (_nickName.text == "")
        {
            Debug.Log("Obrigatorio um nickname.");
        }
        else
        {
            string roomName = GeraID();
            Debug.Log("Sala Criada: " + roomName);
            PhotonNetwork.CreateRoom(roomName, _roomOptions);
        }
        
        
    }

    public void JoinRoom()
    {
        
        if (_nickName.text == "")
        {
            Debug.Log("Obrigatorio um nickname.");
        }
        else
        {
            if (_roomID == null)
            {
                return;
            }

            PhotonNetwork.JoinRoom(_roomID.text);
        }

    }

    public void JoinRandomRoom()
    {
        
        if (_nickName.text == "")
        {
            Debug.Log("Obrigatorio um nickname.");
        }
        else
        {
            PhotonNetwork.JoinRandomRoom();
        }
        
        
    }

    public void MudaNome()
    {
        PhotonNetwork.LocalPlayer.NickName = _nickName.text;
        Debug.Log(_nickName.text);
    }
    
    #endregion

    #region Photon Callbacks

    public override void OnJoinedRoom()
    {
        Debug.Log("Entrou na sala: " + PhotonNetwork.CurrentRoom.Name);
        PhotonNetwork.LoadLevel("GameScene");
    }

    #endregion
    
    
}
