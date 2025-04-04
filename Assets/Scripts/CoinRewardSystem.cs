using PlayFab;
using PlayFab.ClientModels;
using TMPro;
using UnityEngine;

public class CoinRewardSystem : MonoBehaviour
{
    [SerializeField] private TMP_Text coinText;
    public static CoinRewardSystem Instance;
    

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
        
        LoadCoins();
    }
    
    // Chame este método APÓS o login bem-sucedido
    public void LoadCoins()
    {
        PlayFabClientAPI.GetUserInventory(new GetUserInventoryRequest(),
            result => {
                if(result.VirtualCurrency.TryGetValue("GC", out int coins))
                {
                    UpdateCoinUI(coins);
                }
                else
                {
                    Debug.LogWarning("Moeda 'GC' não encontrada - Criando saldo zero...");
                    UpdateCoinUI(0);
                }
            },
            error => {
                Debug.LogError("Erro ao carregar moedas: " + error.GenerateErrorReport());
            });
    }
    
    private void UpdateCoinUI(int coins)
    {
        if (coinText != null)
        {
            coinText.text = coins.ToString();
        }
        else
        {
            Debug.LogWarning("Componente de texto não atribuído no Inspector");
        }
    }
    
    public void AddCoinsToPlayer(int amount)
    {
        // Atualização local imediata para feedback visual
        if (coinText != null && int.TryParse(coinText.text, out int currentCoins))
        {
            coinText.text = (currentCoins + amount).ToString();
        }

        // Atualização no PlayFab
        var request = new AddUserVirtualCurrencyRequest
        {
            VirtualCurrency = "GC",
            Amount = amount
        };

        PlayFabClientAPI.AddUserVirtualCurrency(request,
            result => {
                Debug.Log($"Moedas adicionadas com sucesso! Novo total: {result.Balance}");
                // Atualiza com o valor confirmado pelo servidor
                coinText.text = result.Balance.ToString();
            },
            error => {
                Debug.LogError($"Erro ao adicionar moedas: {error.GenerateErrorReport()}");
                // Reverte a mudança local em caso de erro
                if (coinText != null && int.TryParse(coinText.text, out currentCoins))
                {
                    coinText.text = (currentCoins - amount).ToString();
                }
            });
    }
}