using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuPause : MonoBehaviour
{

    public GameObject pauseMenuPanel;
    public bool _IsPause = false;
    
    private Dora _Dora;

    void Start()
    {
        // Obtém a referência ao script Dora (do player)
        _Dora = FindObjectOfType<Dora>();
        
        if (_Dora == null)
        {
            Debug.LogError("Dora script not found in the scene!");
        }
        
        pauseMenuPanel.SetActive(false);
        
    }
    void Update()
    {
        // Abre/fecha a roda com T
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            _IsPause = !_IsPause;
            _Dora._canMove = _IsPause;
            pauseMenuPanel.SetActive(_IsPause);
            Debug.Log($"Pause Menu: {_IsPause}");
        }
    }
    
    public void OnResumeButtonClicked()
    {
        // Garante que vai sair do estado de pausa
        _IsPause = false;
    
        // Habilita o movimento do player (inverso do estado de pausa)
        _Dora._canMove = true;
    
        // Desativa o painel de pausa
        pauseMenuPanel.SetActive(false);
        
    
        Debug.Log("Jogo despausado - Player pode se mover");
    }
}
