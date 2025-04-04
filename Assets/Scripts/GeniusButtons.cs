using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GeniusButtons : MonoBehaviour
{
    public GeniusGame geniusGame;
    public int indexCor; // 0=Vermelho, 1=Azul, 2=Roxo, 3=Amarelo
    private bool jogadorPerto;
    private Animator anim;
    
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

    private void Update()
    {
        if (jogadorPerto && Input.GetKeyDown(KeyCode.E))
        {
            anim.Play("Apertar"); // Nome da animação de clique
            geniusGame.OnPlayerButtonClick(indexCor);
        }
    }
}
