using System;
using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using Photon.Realtime;
using TMPro;
using UnityEngine;

public class ChatManager : MonoBehaviourPunCallbacks
{

    #region Private Var

    public static ChatManager Instance;
    
    [SerializeField] private TMP_InputField _inputMessage;
    [SerializeField] private GameObject _content;
    [SerializeField] private GameObject _message;
    private PhotonView _photonView;
    [SerializeField] private int _numeroMaxMessage = 10;

    private Queue<GameObject> _filaMessage = new Queue<GameObject>();

    public delegate void BloqueioMovimento(bool move);
    public BloqueioMovimento bloqueioMovimento;

    #endregion

    #region Unity Methods
    
    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
    }
    
    void Start()
    {
        _photonView = GetComponent<PhotonView>();
        bloqueioMovimento = PlayerController.Instance.HabilitaMovimentacao;

    }
    
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown(KeyCode.KeypadEnter))
        {
            if (!string.IsNullOrEmpty(_inputMessage.text))
            {
                Debug.Log("Texto enviado: " + _inputMessage.text);
                _photonView.RPC("RecebeMensagem", RpcTarget.All, PhotonNetwork.LocalPlayer.NickName + ": " + _inputMessage.text);
                _inputMessage.text = ""; // Limpa o campo após enviar
                
            }
        }
    }
    
    #endregion

    #region Public Methods

    public void BloqueiaMovimento(bool estado)
    {
        bloqueioMovimento.Invoke(estado);
    }

    public GameObject CriaMessage(string texto)
    {
        //Cria o objryo mensagem apartir do prefab de texto.
        GameObject message = Instantiate(_message, _content.transform);

        //Edita o texto da mensagem.
        message.GetComponent<TMP_Text>().text = texto;
        //Ultimas mensagens virão primeiro.
        message.GetComponent<RectTransform>().SetAsLastSibling();

        return message;
    }
    
    public void EnviaMensagem()
    {
        _photonView.RPC(
            "RecebeMensagem",
            RpcTarget.All,
            PhotonNetwork.LocalPlayer.NickName + ": " + _inputMessage.text
        );

        _inputMessage.text = "";

    }

    public bool ChatAtivo()
    {
        return _inputMessage.isFocused;
    }
    
    // [PunRPC]
    // public void ReceiveMessage(string messageReceived)
    // {
    //     
    //     if (_filaMessage.Count <= _numeroMaxMessage)
    //     {
    //         _filaMessage.Enqueue(CriaMessage(messageReceived));
    //     }
    //     else
    //     {
    //         GameObject smsRemoved = _filaMessage.Dequeue();
    //         Destroy(smsRemoved);
    //         _filaMessage.Enqueue(CriaMessage(messageReceived));
    //     }
    //     
    // }
    
    [PunRPC]
    public void RecebeMensagem(string mensagemRecebida)
    {
        if (_filaMessage.Count <= _numeroMaxMessage)
        {
            var mensagem = CriaMessage(mensagemRecebida);
            _filaMessage.Enqueue(mensagem);
        }
        else
        {
            var tempMsg = _filaMessage.Dequeue();
            Destroy(tempMsg);
            var mensagem = CriaMessage(mensagemRecebida);
            _filaMessage.Enqueue(mensagem);
        }

    }
    
    #endregion
    
}