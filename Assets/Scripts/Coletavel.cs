using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Coletavel : MonoBehaviourPun
{

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(!collision.gameObject.GetPhotonView().IsMine)
        {
            return;
        }


        photonView.RPC("DestroyItem", RpcTarget.MasterClient);


    }

    [PunRPC]
    public void DestroyItem()
    {
        PhotonNetwork.Destroy(this.gameObject);
    }

}
