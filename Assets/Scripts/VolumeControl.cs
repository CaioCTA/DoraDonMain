using UnityEngine;
using UnityEngine.UI;

public class SimpleVolumeControl : MonoBehaviour
{
    [SerializeField] private Slider volumeSlider;
    
    void Start()
    {
        volumeSlider.value = AudioListener.volume;
        volumeSlider.onValueChanged.AddListener(v => AudioListener.volume = v);
    }
}