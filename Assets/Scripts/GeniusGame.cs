using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GeniusGame : MonoBehaviour
{
    // Lista de cores (ou botões) disponíveis
    public List<GameObject> colorButtons; // GameObjects que representam as cores
    public List<int> sequence = new List<int>(); // Armazena a sequência gerada
    private int playerStep = 0; // Contador para o passo atual do jogador

    void Start()
    {
        StartGame();
    }

    void StartGame()
    {
        sequence.Clear(); // Limpa a sequência anterior
        playerStep = 0; // Reinicia o contador do jogador
        AddNewStep(); // Adiciona o primeiro passo à sequência
        StartCoroutine(PlaySequence()); // Mostra a sequência ao jogador
    }

    void AddNewStep()
    {
        int randomColor = Random.Range(0, colorButtons.Count); // Gera um número aleatório entre 0 e o número de cores
        sequence.Add(randomColor); // Adiciona o número à sequência
    }

    IEnumerator PlaySequence()
    {
        foreach (int colorIndex in sequence)
        {
            yield return new WaitForSeconds(0.5f); // Espera meio segundo antes de mostrar a próxima cor
            HighlightButton(colorIndex); // Destaca o botão correspondente
            yield return new WaitForSeconds(0.5f); // Mantém o botão destacado por meio segundo
            DimButton(colorIndex); // Volta o botão ao normal
        }
    }

    void HighlightButton(int index)
    {
        // Aqui você pode mudar a cor, escala ou qualquer efeito visual para destacar o botão
        colorButtons[index].GetComponent<Renderer>().material.color = Color.white; // Exemplo: muda a cor para branco
    }

    void DimButton(int index)
    {
        // Volta o botão ao estado normal
        colorButtons[index].GetComponent<Renderer>().material.color = GetDefaultColor(index); // Restaura a cor original
    }

    Color GetDefaultColor(int index)
    {
        // Define as cores padrão para cada botão
        switch (index)
        {
            case 0: return Color.green; // Verde
            case 1: return Color.red;    // Vermelho
            case 2: return Color.yellow; // Amarelo
            case 3: return Color.blue;  // Azul
            default: return Color.black; // Cor padrão (não deve acontecer)
        }
    }

    public void PlayerInput(int colorIndex)
    {
        Debug.Log("Jogador clicou na cor: " + colorIndex);

        if (colorIndex == sequence[playerStep])
        {
            playerStep++; // Avança para o próximo passo

            if (playerStep == sequence.Count) // Verifica se o jogador completou a sequência
            {
                Debug.Log("Sequência correta! Próximo nível.");
                playerStep = 0; // Reinicia o contador do jogador
                AddNewStep(); // Adiciona um novo passo à sequência
                StartCoroutine(PlaySequence()); // Mostra a nova sequência
            }
        }
        else
        {
            Debug.Log("Errou! Fim de jogo.");
            StartGame(); // Reinicia o jogo
        }
    }
}