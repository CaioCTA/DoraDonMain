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

    public float displayTime = 3f;
    [SerializeField] private List<GameObject> _playersPanels;
    [SerializeField] private TMP_Text _textPlayerCount;
    [SerializeField] private TMP_Text _NoPlay;
    [SerializeField] private TMP_Text _NoStart;
    [SerializeField] private TMP_Text _IDRoom;
    private int _playersCount;
    #endregion

    #region Unity Methods
    private void Awake()
    {
        
        ChecaJogadores();
        _IDRoom.text = $"Código da sala: {PhotonNetwork.CurrentRoom.Name}.";
        
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

        if (_playersCount < 2)
        {
            Debug.Log("Precisa de dois players para iniciar.");
            StartCoroutine(DisplayTextNoPLay());
        }


        if (!PhotonNetwork.IsMasterClient)
        {
            Debug.Log($"Apenas o dono da sala {PhotonNetwork.MasterClient.NickName} pode inciar o jogo!");
            StartCoroutine(DisplayTextNoStart());

        }

        if (PhotonNetwork.IsMasterClient)
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

    #region IEnumerator

    IEnumerator DisplayTextNoStart()
    {
        _NoStart.text = $"Apenas o dono da sala {PhotonNetwork.MasterClient.NickName} pode inciar o jogo!";
        yield return new WaitForSeconds(displayTime);
        _NoStart.text = string.Empty;
    }
    
    IEnumerator DisplayTextNoPLay()
    {
        _NoPlay.text = $"Não pode iniciar o jogo com apenas {PhotonNetwork.CurrentRoom.PlayerCount} jogadores.";
        yield return new WaitForSeconds(displayTime);
        _NoPlay.text = string.Empty;
    }

    #endregion
    
}