using System.Collections;
using System.Collections.Generic;
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
    
    
    private void Update()
    {
        if (Input.GetKeyUp(KeyCode.Tab))
        {
            UpdateLeaderBoard();
            RecuperarLeaderBoard();
        }
    }
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

                    // limpar a tabela antes de fazer a rotinha de mostrar os novos resultados

                    if (_LBEntries != null && _LBEntries.Length > 0)
                    {
                        foreach (GameObject entry in _LBEntries)
                        {
                            if (entry != null) Destroy(entry); // Destrói cada linha
                        }
                        _LBEntries = new GameObject[0]; // "Limpa" o array (cria um vazio)
                    }

                    // fazer um laço para destruir os registros, SE HOUVER registros

                    if (_LBEntries != null && _LBEntries.Length > 0) // Verifica se o array não é nulo e tem elementos
                    {
                        for (int i = 0; i < _LBEntries.Length; i++)
                        {
                            if (_LBEntries[i] != null) // Verifica se o objeto não é nulo
                            {
                                Destroy(_LBEntries[i]); // Destrói cada linha da UI
                            }
                        }
                        _LBEntries = new GameObject[0]; // Reseta o array para vazio
                    }

                    // limpar a lista/array _LBEntries
                    _LBEntries = new GameObject[0];


                    // inicializar o array de linhas da tabela
                    _LBEntries = new GameObject[result.Leaderboard.Count];

                    // popula as linhas da tabela com as informações do playfab
                    for (int i = 0; i < _LBEntries.Length; i++)
                    {
                        _LBEntries[i] = Instantiate(_LBRow, _LBTransform);
                        TMP_Text[] colunas = _LBEntries[i].GetComponentsInChildren<TMP_Text>();
                        colunas[0].text = (result.Leaderboard[i].Position + 1).ToString();
                        colunas[1].text = result.Leaderboard[i].DisplayName;
                        colunas[2].text = result.Leaderboard[i].StatValue.ToString(); 
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
    
}
