using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Win : MonoBehaviourPunCallbacks
{

    #region Methods
    
    public void OnWinButtonPressed()
    {
        if (PhotonNetwork.InRoom)
        {
            PhotonNetwork.LeaveRoom(); // Sai da sala (assíncrono)
        }
        else
        {
            LoadMenu(); // Caso já esteja fora, carrega o menu diretamente
        }
    }

    // Este método é chamado automaticamente pelo Photon quando o jogador sai da sala
    public override void OnLeftRoom()
    {
        LoadMenu(); // Carrega o menu ao sair da sala
    }

    private void LoadMenu()
    {
        SceneManager.LoadScene("Menu"); // Substitua "MenuScene" pelo nome exato da sua cena do menu
    }
    
    #endregion
    
    
}
