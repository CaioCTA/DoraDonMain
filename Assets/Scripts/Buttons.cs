using System;
using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;

public class Buttons : MonoBehaviourPunCallbacks
{
    
    [SerializeField] private GameObject Door;
    
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.gameObject.GetPhotonView().IsMine)
        {
            return;
        }
        
        other.GetComponent<PlayerController>();
        photonView.RPC("AtivarButton", RpcTarget.MasterClient);
        
    }
    
    [PunRPC]
    public void AtivarButton()
    {
        Color button = GetComponent<SpriteRenderer>().color;
        button = Color.green;
        // _ButtonAtivado.Add();
        PhotonNetwork.Destroy(Door);
    }
    
    
}
