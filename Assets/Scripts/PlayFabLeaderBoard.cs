using System.Collections;
using System.Collections.Generic;
using PlayFab;
using PlayFab.ClientModels;
using TMPro;
using UnityEngine;

public class PlayFabLeaderBoard : MonoBehaviour
{

    public Transform _LBTransform;
    public GameObject _LBRow;
    public GameObject[] _LBEntries;
    
    public void RecuperarLeaderBoard()
    {
        GetLeaderboardRequest request = new GetLeaderboardRequest
        {
            StartPosition = 1,
            StatisticName = "Deaths",
            MaxResultsCount = 10
        };
        
        
        PlayFabClientAPI.GetLeaderboard(
            request,
            result =>
            {
                for (int i = 0; i < _LBEntries.Length; i++)
                {
                    _LBEntries[i] = Instantiate(_LBRow, _LBTransform);
                    TMP_Text[] colunas = _LBEntries[i].GetComponentsInChildren<TMP_Text>();
                    colunas[0].text = result.Leaderboard[i].DisplayName.ToString();
                    colunas[1].text = result.Leaderboard[i].Position.ToString();
                    colunas[2].text = result.Leaderboard[i].StatValue.ToString();
                }
            },
            error =>
            {
                Debug.Log("[PlayFabLeaderBoard] Update error: " + error.GenerateErrorReport());
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
