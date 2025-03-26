using PlayFab;
using PlayFab.ClientModels;
using UnityEngine;

public class CoinRewardSystem : MonoBehaviour
{
    [SerializeField] private int coinsPerLevel = 100; // Moedas por fase

    // Chamar quando o jogador COMPLETAR uma fase
    public void CompleteLevel()
    {
        AddCoins(coinsPerLevel);
    }

    // Método para adicionar moedas
    private void AddCoins(int amount)
    {
        var request = new AddUserVirtualCurrencyRequest
        {
            VirtualCurrency = "GC", // Código da moeda criada
            Amount = amount
        };

        PlayFabClientAPI.AddUserVirtualCurrency(
            request,
            result => Debug.Log($"Moedas adicionadas! Total: {result.Balance}"),
            error => Debug.LogError($"Erro: {error.GenerateErrorReport()}")
        );
    }
}