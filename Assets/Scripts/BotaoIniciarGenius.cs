using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BotaoIniciarGenius : MonoBehaviour
{
    public GeniusGame geniusGame;
    private bool jogadorPerto;
    
    
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            jogadorPerto = true;
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            jogadorPerto = false;
        }
    }

    void Update()
    {
        if (jogadorPerto && Input.GetKeyDown(KeyCode.E))
        {
            if (geniusGame != null) // Adicione esta verificação
            {
                geniusGame.IniciarJogo();
            }
            else
            {
                Debug.LogError("GeniusGame não atribuído no BotaoIniciarGenius!");
            }
        }
    }
}
