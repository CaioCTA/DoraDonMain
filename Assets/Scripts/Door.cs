using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;

public class Door : MonoBehaviourPunCallbacks
{

    public GameObject door;

    private void OnTriggerStay2D(Collider2D other)
    {
        PhotonView view = other.gameObject.GetComponent<PhotonView>();
        
        if (view != null && other.gameObject.tag == "Player" && Input.GetKeyDown(KeyCode.E))
        {
            Destroy(door.gameObject);
        }
    }
    
    
}
