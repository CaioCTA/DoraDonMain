using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using System;
using TMPro;
using JetBrains.Annotations;
using UnityEngine.UI;

public class LobbyManager : MonoBehaviourPunCallbacks
{
    #region Variables
    [SerializeField] private List<GameObject> _playersPanels;
    [SerializeField] private TMP_Text _textPlayerCount;
    private int _playersCount;
    private string _masterPlayer = PhotonNetwork.MasterClient.NickName;
    #endregion

    #region Unity Methods
    private void Awake()
    {
        
        ChecaJogadores();
        
    }

    private void Update()
    {
        ChecaJogadores();
    }
    #endregion

    #region Private Methods
    private void ChecaJogadores()
    {
        _playersCount = PhotonNetwork.CurrentRoom.PlayerCount;
        Player[] playersList = PhotonNetwork.PlayerList;

        if( _playersCount <= 0)
        {
            return;
        }
        
        foreach (var panel in _playersPanels)
        {
            panel.SetActive(false);
        }

        _textPlayerCount.text = "Jogadores na sala: " + _playersCount.ToString();

        for(int i = 0; i < _playersCount; i++)
        {
            _playersPanels[i].SetActive(true);
            _playersPanels[i].GetComponentInChildren<TMP_Text>().text = playersList[i].NickName;
            
            
        }
        
    }
    
    
    #endregion
    
    #region Public Methods
    
    public void StartGame()
    {

        if (!PhotonNetwork.IsMasterClient)
        {
            Debug.Log($"Apenas o dono da sala {_masterPlayer} pode inciar o jogo!");
        }
        
        if (_playersCount != 2)
        {
            Debug.Log("Precisa de dois players para iniciar.");
            return;
        }
        else if (PhotonNetwork.IsConnected && PhotonNetwork.IsMasterClient)
        {
            PhotonNetwork.LoadLevel("GameScene");
        }
        
    }    
    
    #endregion
    
    #region Callbacks Photon
    
    public override void OnPlayerLeftRoom(Player otherPlayer)
    {
        // Recheca jogadores quando alguém sai da sala
        ChecaJogadores();
    }
    
    #endregion
    
}