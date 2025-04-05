using System;
using System.Collections.Generic;
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
    public void SpendCoins(int amount, string itemId, System.Action<bool> onComplete = null)
    {
        if (coinText != null && int.TryParse(coinText.text, out int currentCoins))
        {
            if (currentCoins < amount)
            {
                onComplete?.Invoke(false);
                return;
            }

            // Atualização local imediata
            coinText.text = (currentCoins - amount).ToString();

            // Primeiro subtrai as moedas
            var subtractRequest = new SubtractUserVirtualCurrencyRequest
            {
                VirtualCurrency = "GC",
                Amount = amount
            };

            PlayFabClientAPI.SubtractUserVirtualCurrency(subtractRequest,
                subtractResult => {
                    // Depois registra a compra do item
                    var purchaseRequest = new PurchaseItemRequest
                    {
                        ItemId = itemId,
                        VirtualCurrency = "GC",
                        Price = amount
                    };

                    PlayFabClientAPI.PurchaseItem(purchaseRequest,
                        purchaseResult => {
                            Debug.Log($"Item {itemId} comprado com sucesso!");
                            onComplete?.Invoke(true);
                        },
                        purchaseError => {
                            Debug.LogError($"Erro ao registrar compra: {purchaseError.GenerateErrorReport()}");
                            // Reverte a subtração de moedas em caso de erro
                            AddCoinsToPlayer(amount);
                            onComplete?.Invoke(false);
                        });
                },
                subtractError => {
                    Debug.LogError($"Erro ao subtrair moedas: {subtractError.GenerateErrorReport()}");
                    coinText.text = currentCoins.ToString();
                    onComplete?.Invoke(false);
                });
        }
    }
    
    public void CheckPlayerInventory()
    {
        PlayFabClientAPI.GetUserInventory(new GetUserInventoryRequest(),
            result => {
                foreach (var item in result.Inventory)
                {
                    Debug.Log($"Item possuído: {item.ItemId}");
                    // Ative os itens comprados no seu jogo
                    EnablePurchasedItem(item.ItemId);
                }
            },
            error => {
                Debug.LogError($"Erro ao carregar inventário: {error.GenerateErrorReport()}");
            });
    }

    public void EnablePurchasedItem(string itemId)
    {
        // Implemente a lógica para ativar o item no seu jogo
        switch (itemId)
        {
            case "skin_01":
                // Desbloqueia a skin 01
                break;
            case "powerup_01":
                // Desbloqueia o powerup
                break;
        }
    }
    
}