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
using static UnityEditor.Progress;
using WebSocketSharp;

public class CreateGame : MonoBehaviourPunCallbacks
{
    
    #region Variables
    
    public float displayTime = 3f;
    [SerializeField] private TMP_InputField _nickName;
    [SerializeField] private TMP_InputField _roomID;
    public string _roomName;
    [SerializeField] private TMP_Text _noNickname;
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

        _nickName.text = PlayFabLogin.PFL.Nickname;
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

    //public void CriarSala()
    //{

    //    if (_nickName.text == "")
    //    {
    //        Debug.Log("Obrigatorio um nickname.");
    //        StartCoroutine(DisplayTextNoNickname());
    //    }
    //    else
    //    {
    //        string roomName = GeraID();
    //        Debug.Log("Sala Criada: " + roomName);
    //        PhotonNetwork.CreateRoom(roomName, _roomOptions);
    //    }


    //}

    public void CriarSala(string roomName = "")
    {
        roomName = !roomName.IsNullOrEmpty() ? roomName : GeraID();

        Debug.Log("SALA CRIADA");

        PhotonNetwork.CreateRoom(roomName, _roomOptions);
    }


    public void CriarOuEntrarSala(bool isHost, string nomeSala)
    {
        if (isHost)
        {
            PhotonNetwork.CreateRoom(nomeSala, _roomOptions);
        }
        else
        {
            PhotonNetwork.JoinRoom(nomeSala);
        }

    }

    public void JoinRoom()
    {


        if (_roomID == null)
        {
            return;
        }

        PhotonNetwork.JoinRoom(_roomID.text);
       

    }

    public void JoinRandomRoom()
    {

        PhotonNetwork.JoinRandomRoom();

    }

    public void MudaNome()
    {
        PhotonNetwork.LocalPlayer.NickName = _nickName.text;
        Debug.Log(_nickName.text);
    }
    public void SairMenu()
    {
        PhotonNetwork.LoadLevel("Menu");
    }


    #endregion

    #region Photon Callbacks

    public override void OnJoinedRoom()
    {
        MudaNome();

        Debug.Log("Entrou na sala: " + PhotonNetwork.CurrentRoom.Name);
        PhotonNetwork.LoadLevel("Lobby");
    }

    public override void OnJoinRoomFailed(short returnCode, string message)
    {
        Debug.Log($"[PhotonNetwork] Falha ao entrar na sala, erro {returnCode}: {message}, vamos tentar novamente em 2s.");

        // coroutine para ficar tentando entrar na sala
        StartCoroutine(TentaEntrarSala(_roomName));
    }

    #endregion

    #region IEnumerator

    private IEnumerator TentaEntrarSala(string nomeSala)
    {
        yield return new WaitForSeconds(2f);
        Debug.Log($"[PhotonNetwork] Tentando entrar na sala {nomeSala}");
        PhotonNetwork.JoinRoom(nomeSala);
    }


    #endregion


}
