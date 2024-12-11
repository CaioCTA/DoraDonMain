using System;
using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using TMPro;
using UnityEngine;

public class ChatManager : MonoBehaviourPunCallbacks
{

    #region Private Var
    
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
                _photonView.RPC("ReceiveMessage", RpcTarget.All, PhotonNetwork.LocalPlayer.NickName + ": " + _inputMessage.text);
                _inputMessage.text = ""; // Limpa o campo após enviar
                
            }
        }
    }
    
    #endregion

    #region Public Methods


    [PunRPC]
    public void ReceiveMessage(string messageReceived)
    {
        
        if (_filaMessage.Count <= _numeroMaxMessage)
        {
            _filaMessage.Enqueue(CriaMessage(messageReceived));
        }
        else
        {
            GameObject smsRemoved = _filaMessage.Dequeue();
            Destroy(smsRemoved);
            _filaMessage.Enqueue(CriaMessage(messageReceived));
        }
        
    }

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
    
    #endregion
    
}