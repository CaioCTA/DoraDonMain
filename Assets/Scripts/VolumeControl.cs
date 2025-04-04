using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Slider))]
public class VolumeController : MonoBehaviour
{
    [Header("Configurações")]
    [Tooltip("Controla se o volume deve ser salvo entre sessões")]
    [SerializeField] private bool saveVolumeBetweenSessions = true;
    
    private Slider volumeSlider;
    private const string VolumePrefKey = "GameVolume";

    private void Awake()
    {
        volumeSlider = GetComponent<Slider>();
        
        // Configura os limites padrão do slider (0 a 1)
        volumeSlider.minValue = 0f;
        volumeSlider.maxValue = 1f;
        
        // Carrega o volume salvo ou usa 0.8 como padrão
        float savedVolume = saveVolumeBetweenSessions ? 
            PlayerPrefs.GetFloat(VolumePrefKey, 0.8f) : 0.8f;
        
        // Aplica o volume carregado
        SetVolume(savedVolume);
        volumeSlider.value = savedVolume;
        
        // Adiciona o listener para mudanças no slider
        volumeSlider.onValueChanged.AddListener(OnVolumeChanged);
    }

    private void OnVolumeChanged(float newVolume)
    {
        SetVolume(newVolume);
        
        // Salva o volume se configurado
        if (saveVolumeBetweenSessions)
        {
            PlayerPrefs.SetFloat(VolumePrefKey, newVolume);
            PlayerPrefs.Save();
        }
    }

    private void SetVolume(float volume)
    {
        // Limita o volume entre 0 e 1
        volume = Mathf.Clamp01(volume);
        AudioListener.volume = volume;
    }

    private void OnDestroy()
    {
        // Remove o listener quando o objeto for destruído
        if (volumeSlider != null)
        {
            volumeSlider.onValueChanged.RemoveListener(OnVolumeChanged);
        }
    }
}