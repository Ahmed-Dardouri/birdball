using UnityEngine;

public class SoundManager : MonoBehaviour {
    public static SoundManager Instance;

    public AudioSource musicSource;
    public AudioSource sfxSource;
    public AudioClip jumpSound;
    public AudioClip dashSound;
    public AudioClip ballBounceSound;
    public AudioClip ballHitSound;
    public AudioClip bonusSound;
    public AudioClip gameOverSound;
    public AudioClip bgMusic;
    
    public float musicVolume = 1.0f;
    public float sfxVolume = 1.0f;


    void Awake() {
        if (Instance == null) {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        } else {
            Destroy(gameObject);
        }

        SetSFXVolume(sfxVolume);

        musicSource.volume = musicVolume;
        PlayMusic(bgMusic);
    }

    public void PlayMusic(AudioClip clip) {
        musicSource.clip = clip;
        musicSource.loop = true;
        musicSource.Play();
    }

    public void PlaySFX(AudioClip clip) {
        sfxSource.PlayOneShot(clip);
    }

    public void SetMusicVolume(float volume) {
        musicVolume = volume;
        musicSource.volume = volume;
    }
    
    public void SetSFXVolume(float volume) {
        sfxVolume = volume;
        sfxSource.volume = volume;
    }

}