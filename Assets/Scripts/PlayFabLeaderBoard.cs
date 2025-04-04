using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Photon.Pun;
using PlayFab;
using PlayFab.ClientModels;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;

public class PlayFabLeaderBoard : MonoBehaviour
{

    public Transform _LBTransform;
    public GameObject _LBRow;
    public GameObject[] _LBEntries;
    
    
    // private void Update()
    // {
    //     if (Input.GetKeyUp(KeyCode.Tab))
    //     {
    //         UpdateLeaderBoard(30);
    //         RecuperarLeaderBoard();
    //     }
    // }

    private void Start()
    {
        UpdateAndRefreshLeaderboard();
    }
    
    private void UpdateAndRefreshLeaderboard()
    {
        UpdateLeaderBoard(30); // Atualiza estatísticas
        RecuperarLeaderBoard(); // Atualiza a exibição
    }
    // public void RecuperarLeaderBoard()
    // {
    //     GetLeaderboardRequest request = new GetLeaderboardRequest
    //     {
    //         StartPosition = 0,
    //         StatisticName = "Deaths",
    //         MaxResultsCount = 10
    //     };
    //
    //
    //     PlayFabClientAPI.GetLeaderboard(
    //                 request,
    //                 result =>
    //                 {
    //
    //                 // limpar a tabela antes de fazer a rotinha de mostrar os novos resultados
    //
    //                 if (_LBEntries != null && _LBEntries.Length > 0)
    //                 {
    //                     foreach (GameObject entry in _LBEntries)
    //                     {
    //                         if (entry != null) Destroy(entry); // Destrói cada linha
    //                     }
    //                     _LBEntries = new GameObject[0]; // "Limpa" o array (cria um vazio)
    //                 }
    //
    //                 // fazer um laço para destruir os registros, SE HOUVER registros
    //
    //                 if (_LBEntries != null && _LBEntries.Length > 0) // Verifica se o array não é nulo e tem elementos
    //                 {
    //                     for (int i = 0; i < _LBEntries.Length; i++)
    //                     {
    //                         if (_LBEntries[i] != null) // Verifica se o objeto não é nulo
    //                         {
    //                             Destroy(_LBEntries[i]); // Destrói cada linha da UI
    //                         }
    //                     }
    //                     _LBEntries = new GameObject[0]; // Reseta o array para vazio
    //                 }
    //
    //                 // limpar a lista/array _LBEntries
    //                 _LBEntries = new GameObject[0];
    //
    //
    //                 // inicializar o array de linhas da tabela
    //                 _LBEntries = new GameObject[result.Leaderboard.Count];
    //
    //                 
    //                 var leaderboardOrdenado = result.Leaderboard.OrderBy(jogador => jogador.StatValue).ToList();
    //                 
    //                 // popula as linhas da tabela com as informações do playfab
    //                 for (int i = 0; i < _LBEntries.Length; i++)
    //                 {
    //                     _LBEntries[i] = Instantiate(_LBRow, _LBTransform);
    //                     TMP_Text[] colunas = _LBEntries[i].GetComponentsInChildren<TMP_Text>();
    //                     colunas[0].text = (i + 1).ToString();
    //                     colunas[1].text = result.Leaderboard[i].DisplayName;
    //                     colunas[2].text = result.Leaderboard[i].StatValue.ToString(); 
    //                     }
    //                 }
    //                 //TODO: ARRAY SORT
    //                 ,
    //                 error => 
    //                 {
    //                     Debug.LogError($"[PlayFab] {error.GenerateErrorReport()}");
    //                 }
    //             );
    //         }
    
    public void RecuperarLeaderBoard()
    {
        GetLeaderboardRequest request = new GetLeaderboardRequest
        {
            StartPosition = 0,
            StatisticName = "Deaths",
            MaxResultsCount = 10
        };

        PlayFabClientAPI.GetLeaderboard(
            request,
            result =>
            {
                // Limpa a tabela antes de mostrar novos resultados
                if (_LBEntries != null && _LBEntries.Length > 0)
                {
                    foreach (GameObject entry in _LBEntries)
                    {
                        if (entry != null) Destroy(entry);
                    }
                    _LBEntries = new GameObject[0];
                }

                // Ordena o leaderboard do menor para o maior número de mortes
                var leaderboardOrdenado = result.Leaderboard.OrderBy(jogador => jogador.StatValue).ToList();

                // Inicializa o array de linhas da tabela
                _LBEntries = new GameObject[leaderboardOrdenado.Count];

                // Popula as linhas da tabela com as informações ordenadas
                for (int i = 0; i < _LBEntries.Length; i++)
                {
                    _LBEntries[i] = Instantiate(_LBRow, _LBTransform);
                    TMP_Text[] colunas = _LBEntries[i].GetComponentsInChildren<TMP_Text>();
                    colunas[0].text = (i + 1).ToString();
                    colunas[1].text = leaderboardOrdenado[i].DisplayName;
                    colunas[2].text = leaderboardOrdenado[i].StatValue.ToString(); 
                }
            },
            error => 
            {
                Debug.LogError($"[PlayFab] {error.GenerateErrorReport()}");
            }
        );
    }

    public void UpdateLeaderBoard(int morte)
    {
        UpdatePlayerStatisticsRequest request = new UpdatePlayerStatisticsRequest
        {
            Statistics = new List<StatisticUpdate>
            {
                new StatisticUpdate
                {
                    StatisticName = "Deaths",
                    Value = morte
                }
            }
        };
        
        
        PlayFabClientAPI.UpdatePlayerStatistics(
            request,
            result =>
            {
                Debug.Log("[PlayFabLeaderBoard] Leaderboard Update");
            },
            error =>
            {
                Debug.Log("[PlayFabLeaderBoard] Update error: " + error.GenerateErrorReport());
            }
            );
        
    }

    public void ShowLeaderBoard()
    {
        
    }
    
    public void SairMenu()
    {
        PhotonNetwork.LoadLevel("Menu");
    }
    
    public void IncrementPlayerDeath()
    {
        // Primeiro recupera o valor atual de mortes
        GetPlayerStatisticsRequest request = new GetPlayerStatisticsRequest
        {
            StatisticNames = new List<string> { "Deaths" }
        };

        PlayFabClientAPI.GetPlayerStatistics(
            request,
            result =>
            {
                int currentDeaths = 0;
            
                // Encontra a estatística de mortes se existir
                foreach (var stat in result.Statistics)
                {
                    if (stat.StatisticName == "Deaths")
                    {
                        currentDeaths = stat.Value;
                        break;
                    }
                }
            
                // Atualiza com +1 morte
                UpdateLeaderBoard(currentDeaths + 1);
            },
            error =>
            {
                Debug.LogError("Erro ao buscar estatísticas: " + error.GenerateErrorReport());
                // Se falhar, começa com 1 morte
                UpdateLeaderBoard(1);
            }
        );
    }
    
}
