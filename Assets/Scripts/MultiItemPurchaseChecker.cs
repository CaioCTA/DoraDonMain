using UnityEngine;
using PlayFab;
using PlayFab.ClientModels;
using System.Collections.Generic;

public class MultiItemPurchaseChecker : MonoBehaviour
{
    
    public static MultiItemPurchaseChecker Instance;
    
    [System.Serializable]
    public class ItemPanelPair
    {
        public string itemId;       // ID do item no PlayFab (ex: "skin_01")
        public GameObject panel;    // Painel que será desativado se o item já foi comprado
    }

    [Header("Itens e Painéis")]
    [SerializeField] private List<ItemPanelPair> itemsToCheck = new List<ItemPanelPair>();

    private void Awake()
    {
        // Verifica os itens ao iniciar
        CheckOwnedItems();
    }

    public void CheckOwnedItems()
    {
        PlayFabClientAPI.GetUserInventory(new GetUserInventoryRequest(),
            result =>
            {
                foreach (var itemPair in itemsToCheck)
                {
                    bool hasItem = false;
                    foreach (var item in result.Inventory)
                    {
                        if (item.ItemId == itemPair.itemId)
                        {
                            hasItem = true;
                            break;
                        }
                    }

                    // Desativa o painel se o jogador já tem o item
                    if (hasItem && itemPair.panel != null)
                    {
                        itemPair.panel.SetActive(false);
                        Debug.Log($"Item {itemPair.itemId} já comprado - Painel desativado.");
                    }
                }
            },
            error =>
            {
                Debug.LogError("Erro ao verificar inventário: " + error.GenerateErrorReport());
            });
    }
}