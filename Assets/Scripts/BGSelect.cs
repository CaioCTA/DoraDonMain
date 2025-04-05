using UnityEngine;
using UnityEngine.UI;
using PlayFab;
using PlayFab.ClientModels;
using System.Collections.Generic;

public class BackgroundSelector : MonoBehaviour
{
    [System.Serializable]
    public class BackgroundItem
    {
        public string itemId;
        public GameObject backgroundObject;
    }

    [Header("Configuração")]
    [SerializeField] private Button switchButton;
    [SerializeField] private BackgroundItem[] backgrounds;
    [SerializeField] private GameObject defaultBackground;

    private List<GameObject> _ownedBackgrounds = new List<GameObject>();
    private int _currentIndex = 0;
    
    private void Start()
    {
        switchButton.onClick.AddListener(SwitchBackground);
        InitializeBackgrounds();
    }

    private void InitializeBackgrounds()
    {
        // Garante que todos estão desativados inicialmente
        defaultBackground.SetActive(true);
        foreach (var bg in backgrounds)
        {
            bg.backgroundObject.SetActive(false);
        }

        // Verifica itens comprados (substitua pelo seu método PlayFab se necessário)
        _ownedBackgrounds.Add(backgrounds[0].backgroundObject); // Exemplo: bg1
        _ownedBackgrounds.Add(backgrounds[1].backgroundObject); // Exemplo: bg2

        if (_ownedBackgrounds.Count >= 0)
        {
            defaultBackground.SetActive(false);
            _ownedBackgrounds[0].SetActive(true);
        }
    }

    private void SwitchBackground()
    {
        if (_ownedBackgrounds.Count <= 1) return;

        // Desativa o atual
        _ownedBackgrounds[_currentIndex].SetActive(false);

        // Calcula próximo índice
        _currentIndex = (_currentIndex + 1) % _ownedBackgrounds.Count;

        // Ativa o novo
        _ownedBackgrounds[_currentIndex].SetActive(true);
        Debug.Log($"Background ativo: Índice {_currentIndex}");
    }
}