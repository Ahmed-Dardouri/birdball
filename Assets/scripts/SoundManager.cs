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

    private float musicVol;
    private float sfxVol;


    void Awake() {
        musicVol = PlayerPrefs.GetFloat("MusicVolume", 0.75f);
        sfxVol = PlayerPrefs.GetFloat("SFXVolume", 0.75f);

        Debug.Log("soundmngr musicVol : " + musicVol);
        Debug.Log("soundmngr sfxVol : " + sfxVol);
        SetSFXVolume(sfxVol);
        SetMusicVolume(musicVol);
        PlayMusic(bgMusic);
    }

    public void PlayMusic(AudioClip clip) {
        musicSource.clip = clip;
        musicSource.loop = true;
        musicSource.Play();
    }

    public void StopMusic() {
        musicSource.Stop();
    }

    public void PlaySFX(AudioClip clip) {
        sfxSource.PlayOneShot(clip);
    }

    public void SetMusicVolume(float volume) {
        musicSource.volume = volume;
    }
    
    public void SetSFXVolume(float volume) {
        sfxSource.volume = volume;
    }

}