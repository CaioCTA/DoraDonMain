using System;
using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class Don : PlayerController
{
    #region Variables

    private bool _isInWater =  false;
    private GameObject _isNearby;

    #endregion
    
    #region Unity Methods

    protected override void Start()
    {
        
        _rb = GetComponent<Rigidbody2D>();
        if (photonView.IsMine)
        {
            if (LocalPlayerInstance == null)
            {
                LocalPlayerInstance = this.gameObject;

            }
            _nickName = PhotonNetwork.LocalPlayer.NickName;
            // var score = PhotonNetwork.LocalPlayer.CustomProperties["Score"];
            _namePlayer.text = _nickName;
        }
        else
        {
            _namePlayer.text = _nickName;
        }
        
    }

    protected override void Update()
    {
        base.Update();
        if (photonView.IsMine)
        {
            if (_isInWater)
            {
                //Nadar();
            }
        }
    }

    #endregion

    #region 2D Methods

    protected override void OnCollisionEnter2D(Collision2D collision)
    {
        GameObject don = collision.gameObject;
        
        base.OnCollisionEnter2D(collision);
        if (don.CompareTag("Water"))
        {
            _isInWater = true;
        }
    }
    
    protected override void OnCollisionExit2D(Collision2D collision)
    {
        GameObject don = collision.gameObject;
        
        base.OnCollisionExit2D(collision);
        if (don.CompareTag("Water"))
        {
            _isInWater = false;
        }
        
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        // GameObject don = other.gameObject;
        //
        // if (photonView.IsMine)
        // {
        //     if (don.CompareTag("Bola"))
        //     {
        //         _isNearby = don.gameObject;
        //         Debug.Log("AEEEE");
        //     }
        // }
    }
    
    private void OnTriggerExit2D(Collider2D other)
    {
        // GameObject don = other.gameObject;
        //
        // if (photonView.IsMine)
        // {
        //     if (don.CompareTag("Bola"))
        //     {
        //         _isNearby = null;
        //         Debug.Log("Sai do range");
        //     }
        // }
    }


    #endregion
    
}
