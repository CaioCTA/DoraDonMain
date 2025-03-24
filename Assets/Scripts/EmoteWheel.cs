using UnityEngine;

public class EmoteWheelDebug : MonoBehaviour
{
    private bool rodaAberta = false;

    void Update()
    {
        // Abre/fecha a roda com T
        if (Input.GetKeyDown(KeyCode.T))
        {
            rodaAberta = !rodaAberta;
            Debug.Log(rodaAberta ? "Roda ABERTA" : "Roda FECHADA");
        }

        // Se a roda estiver aberta, detecta direcionais
        if (rodaAberta)
        {
            if (Input.GetKeyDown(KeyCode.Alpha1)) // Cima
            {
                Debug.Log("Emote: CIMA (👍)");
                FecharRoda();
            }
            else if (Input.GetKeyDown(KeyCode.Alpha2)) // Baixo
            {
                Debug.Log("Emote: BAIXO (👎)");
                FecharRoda();
            }
            else if (Input.GetKeyDown(KeyCode.Alpha3)) // Esquerda
            {
                Debug.Log("Emote: ESQUERDA (😄)");
                FecharRoda();
            }
            else if (Input.GetKeyDown(KeyCode.Alpha4)) // Direita
            {
                Debug.Log("Emote: DIREITA (😠)");
                FecharRoda();
            }
        }
    }

    void FecharRoda()
    {
        rodaAberta = false;
        Debug.Log("Roda FECHADA após escolha");
    }
}