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

    public typeBtn typeBtn;
    

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
        // if (don.CompareTag("Water"))
        // {
        //     _isInWater = true;
        // }
    }
    
    protected override void OnCollisionExit2D(Collision2D collision)
    {
        GameObject don = collision.gameObject;
        
        base.OnCollisionExit2D(collision);
        // if (don.CompareTag("Water"))
        // {
        //     _isInWater = false;
        // }
        
    }



    #endregion

}
