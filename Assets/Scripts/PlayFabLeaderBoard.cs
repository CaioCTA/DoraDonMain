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
                        
        
                        // fazer um laço para destruir os registros, SE HOUVER registros
                        
        
                        // limpar a lista/array _LBEntries
                        
        
                        // inicializar o array de linhas da tabela
                        _LBEntries = new GameObject[result.Leaderboard.Count];
        
                        // popula as linhas da tabela com as informações do playfab
                        for (int i = 0; i < _LBEntries.Length; i++)
                        {
                            _LBEntries[i] = Instantiate(_LBRow, _LBTransform);
                            TMP_Text[] colunas = _LBEntries[i].GetComponentsInChildren<TMP_Text>();
                            colunas[0].text = result.Leaderboard[i].Position.ToString(); // valor da posição do ranking
                            colunas[1].text = result.Leaderboard[i].DisplayName; // nome do player ou player id
                            colunas[2].text = result.Leaderboard[i].StatValue.ToString(); // valor do estatística
                        }
                    },
                    error => 
                    {
                        Debug.LogError($"[PlayFab] {error.GenerateErrorReport()}");
                    }
                );
            }

    public void UpdateLeaderBoard()
    {
        UpdatePlayerStatisticsRequest request = new UpdatePlayerStatisticsRequest
        {
            Statistics = new List<StatisticUpdate>
            {
                new StatisticUpdate
                {
                    StatisticName = "Deaths",
                    Value = UnityEngine.Random.Range(0, 100)
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
