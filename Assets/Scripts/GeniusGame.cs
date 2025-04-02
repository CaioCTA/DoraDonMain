using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GeniusGame : MonoBehaviour
{
    [Header("Progresso")]
    [SerializeField] private int sequenciasAcertadas = 0;
    [SerializeField] private int sequenciasParaVencer = 3; 
    
    [Header("Fases")]
    [SerializeField] private int faseAtual = 1;
    [SerializeField] private int[] tamanhoPorFase = { 3, 5, 8 };
    
    [Header("Botão de Reinício")]
    [SerializeField] private Button botaoReiniciar;
    
    [Header("Cores quando pisca")]
    [SerializeField] private Color[] coresPiscar;
    
    [Header("Objetos da Sequência")]
    [SerializeField] private GameObject[] sequenciaObjects;
    [SerializeField] private Color[] sequenciaColors;

    [Header("Botões do Jogador")]
    [SerializeField] private Button[] playerButtons;

    [Header("Configurações")]
    [SerializeField] private float delayBetweenSteps = 1f;
    [SerializeField] private AudioClip correctSound, wrongSound, completeSound;
    private AudioSource audioSource;

    private List<int> sequenciaAtual = new List<int>();
    private int playerStep = 0;
    private bool inputEnabled = false;
    private bool esperandoReinicio = false;

    private void Start()
    {
        audioSource = GetComponent<AudioSource>();
        IniciarJogo();
    }

    private void IniciarJogo()
    {
        // Reinicia todos os valores para o estado inicial
        faseAtual = 1;
        sequenciasAcertadas = 0;
        playerStep = 0;
        sequenciaAtual.Clear();
        inputEnabled = false;
        esperandoReinicio = false;
        botaoReiniciar.gameObject.SetActive(false);

        // Configura os botões
        foreach (Button btn in playerButtons)
        {
            btn.onClick.RemoveAllListeners();
        }
        
        for (int i = 0; i < playerButtons.Length; i++)
        {
            int index = i;
            playerButtons[i].onClick.AddListener(() => OnPlayerButtonClick(index));
        }

        botaoReiniciar.onClick.RemoveAllListeners();
        botaoReiniciar.onClick.AddListener(ReiniciarJogoCompleto);

        GerarSequencia();
    }

    private void GerarSequencia()
    {
        sequenciaAtual.Clear();
        int tamanhoSequencia = tamanhoPorFase[faseAtual - 1];
        
        for (int i = 0; i < tamanhoSequencia; i++)
        {
            sequenciaAtual.Add(Random.Range(0, sequenciaObjects.Length));
        }
        
        StartCoroutine(MostrarSequencia());
    }

    private IEnumerator MostrarSequencia()
    {
        inputEnabled = false;
        foreach (int index in sequenciaAtual)
        {
            yield return new WaitForSeconds(0.5f);
            yield return StartCoroutine(PiscarObjeto(index));
            Debug.Log(index);
        }
        inputEnabled = true;
    }

    private IEnumerator PiscarObjeto(int index)
    {
        GameObject obj = sequenciaObjects[index];
        SpriteRenderer sprite = obj.GetComponent<SpriteRenderer>();
        sprite.enabled = true;

        Color originalColor = sprite.color;
        originalColor.a = 1f;

        Color corPiscar = coresPiscar[index];
        corPiscar.a = 1f;

        sprite.color = corPiscar;
        audioSource.PlayOneShot(correctSound);
    
        yield return new WaitForSeconds(0.5f);
    
        sprite.color = originalColor;
    }

    private void OnPlayerButtonClick(int buttonIndex)
    {
        if (!inputEnabled || esperandoReinicio) return;

        if (buttonIndex == sequenciaAtual[playerStep])
        {
            playerStep++;
            audioSource.PlayOneShot(correctSound);

            if (playerStep >= sequenciaAtual.Count)
            {
                sequenciasAcertadas++;
                audioSource.PlayOneShot(completeSound);
                playerStep = 0;
                inputEnabled = false;

                if (sequenciasAcertadas >= sequenciasParaVencer)
                {
                    if (faseAtual < tamanhoPorFase.Length)
                    {
                        faseAtual++;
                        sequenciasAcertadas = 0; // Reseta o contador para a próxima fase
                        StartCoroutine(ProximaFase());
                    }
                    else
                    {
                        Debug.Log("Você venceu todas as fases!");
                        botaoReiniciar.gameObject.SetActive(true);
                    }
                }
                else
                {
                    StartCoroutine(ProximaFase());
                }
            }
        }
        else
        {
            GameOver();
        }
    }

    private IEnumerator ProximaFase()
    {
        yield return new WaitForSeconds(1f);
        GerarSequencia();
    }

    private void GameOver()
    {
        audioSource.PlayOneShot(wrongSound);
        inputEnabled = false;
        esperandoReinicio = true;
        botaoReiniciar.gameObject.SetActive(true);
    }

    private void ReiniciarJogoCompleto()
    {
        // Chama o método que reinicia completamente o jogo
        IniciarJogo();
    }
}