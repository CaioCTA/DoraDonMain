using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class btnLoja : MonoBehaviour
{
   [Header("Configurações")]
    [SerializeField] private int amountToSpend = 1; // Quantidade de moedas a gastar
    [SerializeField] private TMP_Text feedbackText; // Texto para mostrar feedback ao jogador
    [SerializeField] private Button spendButton; // Referência ao botão
    [SerializeField] private GameObject panelBG;

    [Header("Feedback Messages")]
    [SerializeField] private string successMessage = "Compra realizada com sucesso!";
    [SerializeField] private string failMessage = "Saldo insuficiente!";
    [SerializeField] private string errorMessage = "Erro na compra!";
    
    [SerializeField] private string itemIdToPurchase; // ID do item no PlayFab
    

    private void Start()
    {
        // Configura o botão para chamar o método SpendCoins quando clicado
        spendButton.onClick.AddListener(OnSpendButtonClick);
        
    }
    
    private void OnSpendButtonClick()
    {
        CoinRewardSystem.Instance.SpendCoins(amountToSpend, itemIdToPurchase, (success) => 
        {
            if (success)
            {
                panelBG.SetActive(false);
                // Ativa o item localmente também
                CoinRewardSystem.Instance.EnablePurchasedItem(itemIdToPurchase);
            }
        });
    }
    
    private void ShowFeedback(string message, Color color)
    {
        if (feedbackText != null)
        {
            feedbackText.text = message;
            feedbackText.color = color;
            
            // Opcional: Fazer o feedback desaparecer após alguns segundos
            CancelInvoke(nameof(ClearFeedback));
            Invoke(nameof(ClearFeedback), 3f);
        }
    }

    private void ClearFeedback()
    {
        if (feedbackText != null)
        {
            feedbackText.text = "";
        }
    }

    // Método para mudar a quantidade de moedas a gastar (opcional)
    public void SetAmountToSpend(int amount)
    {
        amountToSpend = amount;
        
        // Atualiza o texto do botão se existir
        if (spendButton.GetComponentInChildren<TMP_Text>() != null)
        {
            spendButton.GetComponentInChildren<TMP_Text>().text = $"Comprar ({amountToSpend} moedas)";
        }
    }

}
