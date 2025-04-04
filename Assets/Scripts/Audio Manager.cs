using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;

public class AudioManager : MonoBehaviour
{
    [Header ("---- Audio Source ----")]
    [SerializeField] AudioSource menuMusicSource;
    [SerializeField] AudioSource gameplayMusicSource;
    [SerializeField] AudioSource SFXSource;

    [Header("---- Audio Clip ----")]
    public AudioClip menuBackground;
    public AudioClip gameplayBackground;


    public static AudioManager instance;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        PlayMenuMusic();
    }

    private void Update()
    {
        if (SceneManager.GetActiveScene().name == "GameScene" && !gameplayMusicSource.isPlaying)
        {
            PlayGameplayMusic();
        }
        else if (SceneManager.GetActiveScene().name != "GameScene" && !menuMusicSource.isPlaying)
        {
            PlayMenuMusic();
        }
    }

    void PlayMenuMusic()
    {
        gameplayMusicSource.Stop();
        menuMusicSource.clip = menuBackground;
        menuMusicSource.Play();
    }

    void PlayGameplayMusic()
    {
        menuMusicSource.Stop();
        gameplayMusicSource.clip = gameplayBackground;
        gameplayMusicSource.Play();
    }

    public void PlaySFX(AudioClip clip)
    {
        SFXSource.clip = clip; 
    }

}

