// using System.Collections;
// using System.Collections.Generic;
// using Photon.Pun;
// using UnityEngine;
// using UnityEngine.UI;
//
// public class GeniusGame : MonoBehaviourPunCallbacks
// {
//     [Header("Progresso")]
//     [SerializeField] private int sequenciasAcertadas = 0;
//     [SerializeField] private int sequenciasParaVencer = 3; 
//     
//     [Header("Fases")]
//     [SerializeField] private int faseAtual = 1;
//     [SerializeField] private int[] tamanhoPorFase = { 3, 5, 8 };
//     
//     [Header("Botão de Reinício")]
//     [SerializeField] private Button botaoReiniciar;
//     
//     [Header("Cores quando pisca")]
//     [SerializeField] private Color[] coresPiscar;
//     
//     [Header("Objetos da Sequência")]
//     [SerializeField] private GameObject[] sequenciaObjects;
//     [SerializeField] private Color[] sequenciaColors;
//
//     // [Header("Botões do Jogador")]
//     // [SerializeField] private Button[] playerButtons;
//
//     [Header("Configurações")]
//     [SerializeField] private float delayBetweenSteps = 1f;
//     [SerializeField] private AudioClip correctSound, wrongSound, completeSound;
//     private AudioSource audioSource;
//
//     private List<int> sequenciaAtual = new List<int>();
//     private int playerStep = 0;
//     private bool inputEnabled = false;
//     private bool esperandoReinicio = false;
//     
//     [Header("Botão de Início")]
//     [SerializeField] private GameObject botaoIniciar; // Objeto físico com Collider2D
//     private bool jogoIniciado = false;
//
//     private void Start()
//     {
//         audioSource = GetComponent<AudioSource>();
//     }
//
//     [PunRPC]
//     public void IniciarJogo()
//     {
//         
//         // Verifique se os arrays estão atribuídos
//         if (sequenciaObjects == null || sequenciaObjects.Length == 0)
//         {
//             Debug.LogError("Sequencia Objects não configurado!");
//             return;
//         }
//         
//         
//         // Configuração inicial do jogo
//         faseAtual = 1;
//         sequenciasAcertadas = 0;
//         playerStep = 0;
//         sequenciaAtual.Clear();
//         inputEnabled = false;
//         esperandoReinicio = false;
//
//         GerarSequencia();
//     }
//
//     private void GerarSequencia()
//     {
//         sequenciaAtual.Clear();
//         int tamanhoSequencia = tamanhoPorFase[faseAtual - 1];
//         
//         for (int i = 0; i < tamanhoSequencia; i++)
//         {
//             sequenciaAtual.Add(Random.Range(0, sequenciaObjects.Length));
//         }
//         
//         StartCoroutine(MostrarSequencia());
//     }
//
//     private IEnumerator MostrarSequencia()
//     {
//         inputEnabled = false;
//         foreach (int index in sequenciaAtual)
//         {
//             yield return new WaitForSeconds(0.5f);
//             yield return StartCoroutine(PiscarObjeto(index));
//             Debug.Log(index);
//         }
//         inputEnabled = true;
//     }
//
//     private IEnumerator PiscarObjeto(int index)
//     {
//         // Pisca o objeto da sequência (como antes)
//         GameObject objPai = sequenciaObjects[index];
//         Animator animSequencia = objPai.transform.Find("Animacao").GetComponent<Animator>();
//         animSequencia.SetTrigger("Piscar");
//         audioSource.PlayOneShot(correctSound);
//     
//         yield return new WaitForSeconds(0.5f);
//     
//         // Volta para idle (opcional)
//         animSequencia.SetTrigger("Idle");;
//     }
//
//     public void OnPlayerButtonClick(int buttonIndex)
//     {
//         if (!inputEnabled || esperandoReinicio) return;
//
//         if (buttonIndex == sequenciaAtual[playerStep])
//         {
//             playerStep++;
//             audioSource.PlayOneShot(correctSound);
//
//             if (playerStep >= sequenciaAtual.Count)
//             {
//                 sequenciasAcertadas++;
//                 audioSource.PlayOneShot(completeSound);
//                 playerStep = 0;
//                 inputEnabled = false;
//
//                 if (sequenciasAcertadas >= sequenciasParaVencer)
//                 {
//                     if (faseAtual < tamanhoPorFase.Length)
//                     {
//                         faseAtual++;
//                         sequenciasAcertadas = 0; // Reseta o contador para a próxima fase
//                         StartCoroutine(ProximaFase());
//                     }
//                     else
//                     {
//                         Debug.Log("Você venceu todas as fases!");
//                         botaoReiniciar.gameObject.SetActive(true);
//                     }
//                 }
//                 else
//                 {
//                     StartCoroutine(ProximaFase());
//                 }
//             }
//         }
//         else
//         {
//             GameOver();
//         }
//     }
//
//     private IEnumerator ProximaFase()
//     {
//         yield return new WaitForSeconds(1f);
//         GerarSequencia();
//     }
//
//     private void GameOver()
//     {
//         Debug.Log("Perdeu");
//         audioSource.PlayOneShot(wrongSound);
//         inputEnabled = false;
//         esperandoReinicio = true;
//         botaoReiniciar.gameObject.SetActive(true);
//     }
//     
// }

using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine;
using UnityEngine.UI;

public class GeniusGame : MonoBehaviourPunCallbacks
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
    
    [Header("Configurações")]
    [SerializeField] private float delayBetweenSteps = 1f;
    [SerializeField] private AudioClip correctSound, wrongSound, completeSound;

    [Header("Porta")] [SerializeField] private GameObject porta;
    
    private AudioSource audioSource;
    private List<int> sequenciaAtual = new List<int>();
    private int playerStep = 0;
    private bool inputEnabled = false;
    private bool esperandoReinicio = false;

    private void Start()
    {
        audioSource = GetComponent<AudioSource>();
        
        // Só o mestre pode iniciar o jogo
        if (PhotonNetwork.IsMasterClient)
        {
            photonView.RPC("IniciarJogo", RpcTarget.AllBuffered);
        }
    }

    [PunRPC]
    public void IniciarJogo()
    {
        if (sequenciaObjects == null || sequenciaObjects.Length == 0)
        {
            Debug.LogError("Sequencia Objects não configurado!");
            return;
        }

        faseAtual = 1;
        sequenciasAcertadas = 0;
        playerStep = 0;
        sequenciaAtual.Clear();
        inputEnabled = false;
        esperandoReinicio = false;

        if (PhotonNetwork.IsMasterClient)
        {
            GerarSequencia();
            photonView.RPC("SincronizarSequencia", RpcTarget.Others, sequenciaAtual.ToArray());
        }
    }

    [PunRPC]
    private void SincronizarSequencia(int[] sequencia)
    {
        sequenciaAtual = new List<int>(sequencia);
        StartCoroutine(MostrarSequencia());
    }

    private void GerarSequencia()
    {
        sequenciaAtual.Clear();
        int tamanhoSequencia = tamanhoPorFase[faseAtual - 1];
        
        for (int i = 0; i < tamanhoSequencia; i++)
        {
            sequenciaAtual.Add(Random.Range(0, sequenciaObjects.Length));
        }
        
        photonView.RPC("SincronizarSequencia", RpcTarget.Others, sequenciaAtual.ToArray());
        StartCoroutine(MostrarSequencia());
    }

    private IEnumerator MostrarSequencia()
    {
        inputEnabled = false;
        
        foreach (int index in sequenciaAtual)
        {
            photonView.RPC("PiscarObjeto", RpcTarget.All, index);
            yield return new WaitForSeconds(delayBetweenSteps);
        }
        
        inputEnabled = true;
    }

    [PunRPC]
    private void PiscarObjeto(int index)
    {
        StartCoroutine(ExecutarPiscar(index));
    }

    private IEnumerator ExecutarPiscar(int index)
    {
        GameObject objPai = sequenciaObjects[index];
        Animator animSequencia = objPai.transform.Find("Animacao").GetComponent<Animator>();
        animSequencia.SetTrigger("Piscar");
        audioSource.PlayOneShot(correctSound);
        
        yield return new WaitForSeconds(0.5f);
        
        animSequencia.SetTrigger("Idle");
    }

    public void OnPlayerButtonClick(int buttonIndex)
    {
        if (!inputEnabled || esperandoReinicio || !photonView.IsMine) return;

        photonView.RPC("ProcessarInput", RpcTarget.MasterClient, buttonIndex, PhotonNetwork.LocalPlayer.ActorNumber);
    }

    [PunRPC]
    private void ProcessarInput(int buttonIndex, int actorNumber)
    {
        if (buttonIndex == sequenciaAtual[playerStep])
        {
            playerStep++;
            photonView.RPC("AtualizarEstado", RpcTarget.All, playerStep, sequenciasAcertadas);
            
            if (playerStep >= sequenciaAtual.Count)
            {
                sequenciasAcertadas++;
                photonView.RPC("SequenciaCompleta", RpcTarget.All, sequenciasAcertadas);
            }
        }
        else
        {
            photonView.RPC("GameOver", RpcTarget.All);
        }
    }

    [PunRPC]
    private void AtualizarEstado(int novoPlayerStep, int novasSequenciasAcertadas)
    {
        playerStep = novoPlayerStep;
        sequenciasAcertadas = novasSequenciasAcertadas;
        audioSource.PlayOneShot(correctSound);
    }

    [PunRPC]
    private void SequenciaCompleta(int novasSequenciasAcertadas)
    {
        sequenciasAcertadas = novasSequenciasAcertadas;
        audioSource.PlayOneShot(completeSound);
        playerStep = 0;
        inputEnabled = false;

        if (sequenciasAcertadas >= sequenciasParaVencer)
        {
            if (faseAtual < tamanhoPorFase.Length)
            {
                faseAtual++;
                sequenciasAcertadas = 0;
                StartCoroutine(ProximaFase());
            }
            else
            {
                Debug.Log("Você venceu todas as fases!");
                botaoReiniciar.gameObject.SetActive(true);
                porta.gameObject.SetActive(false);
            }
        }
        else
        {
            StartCoroutine(ProximaFase());
        }
    }

    [PunRPC]
    private void GameOver()
    {
        Debug.Log("Perdeu");
        audioSource.PlayOneShot(wrongSound);
        inputEnabled = false;
        esperandoReinicio = true;
        botaoReiniciar.gameObject.SetActive(true);
    }

    private IEnumerator ProximaFase()
    {
        yield return new WaitForSeconds(1f);
        
        if (PhotonNetwork.IsMasterClient)
        {
            GerarSequencia();
        }
    }
}