using UnityEngine;
using PlayFab;
using PlayFab.ClientModels;

public class BackgroundManager : MonoBehaviour
{
    [System.Serializable]
    public class BackgroundItem
    {
        public string itemId;  // ID no PlayFab (ex: "bg_space", "bg_forest")
        public GameObject backgroundObject; // Referência ao GameObject do background
    }

    [Header("Backgrounds Disponíveis")]
    [SerializeField] private BackgroundItem[] backgrounds;
    [SerializeField] private GameObject defaultBackground; // Background padrão (se nenhum estiver comprado)

    private void Start()
    {
        // Garante que apenas um background fique ativo por vez
        foreach (var bg in backgrounds)
        {
            bg.backgroundObject.SetActive(false);
        }
        defaultBackground.SetActive(true);

        // Verifica os itens ao logar
        CheckOwnedBackgrounds();
    }

    private void CheckOwnedBackgrounds()
    {
        PlayFabClientAPI.GetUserInventory(new GetUserInventoryRequest(),
            result =>
            {
                bool hasAnyBackground = false;

                // Verifica cada background
                foreach (var bg in backgrounds)
                {
                    bool hasItem = false;
                    foreach (var item in result.Inventory)
                    {
                        if (item.ItemId == bg.itemId)
                        {
                            hasItem = true;
                            break;
                        }
                    }

                    // Ativa o background se o jogador tiver comprado
                    if (hasItem)
                    {
                        bg.backgroundObject.SetActive(true);
                        defaultBackground.SetActive(false);
                        hasAnyBackground = true;
                        Debug.Log($"Background {bg.itemId} ativado!");
                    }
                }

                // Se não tiver nenhum, mantém o padrão
                if (!hasAnyBackground)
                {
                    defaultBackground.SetActive(true);
                }
            },
            error =>
            {
                Debug.LogError("Erro ao verificar backgrounds: " + error.GenerateErrorReport());
                // Fallback: mantém o background padrão
                defaultBackground.SetActive(true);
            });
    }
}