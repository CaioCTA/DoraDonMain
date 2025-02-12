using System;
using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using Photon.Pun.UtilityScripts;
using UnityEngine;
using UnityEngine.EventSystems;

public class DonTeste : MonoBehaviour, IPunObservable
{
    [SerializeField] private float _movSpeed = 5f;
    private Rigidbody2D _rb;
    
    
    //Sync
    private float lastUpdate;
    private Vector2 latestPosition;

    private void Start()
    {
        lastUpdate = Time.time;
    }

    private void Update()
    {
        if (GetComponent<PhotonView>().IsMine)
        {
            MovePlayer();
        }
        else
        {
            //Suavizar o mov remoto
            SmoothMove();
        }
    }

    void MovePlayer()
    {
        float moveX = Input.GetAxis("Horizontal") * _movSpeed * Time.deltaTime;
        float moveY = Input.GetAxis("Vertical") * _movSpeed * Time.deltaTime;

        _rb.velocity = new Vector2(moveX * _movSpeed, moveY * _movSpeed);
    }

    private void SmoothMove()
    {
        transform.position = Vector2.Lerp(transform.position, latestPosition, Time.deltaTime * 5f);
    }

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.IsWriting)
        {
            stream.SendNext(transform.position);
        }
        else
        {
            latestPosition = (Vector2)stream.ReceiveNext();
            
            //Calcula o tempo desde a ultima atualizacao
            lastUpdate = Time.time;
        }
    }
}
