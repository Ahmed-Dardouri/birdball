using UnityEngine;
using UnityEngine.UI;

public class audio_setting_menu_mngr : MonoBehaviour
{
    public GameObject SettingsCanvas;
    public GameObject AudiosSettingsCanvas;

    public Slider musicSlider;
    public Slider sfxSlider;

    public void Start()
    {
        float musicVol = PlayerPrefs.GetFloat("MusicVolume", 0.75f);
        float sfxVol = PlayerPrefs.GetFloat("SFXVolume", 0.75f);

        SetMusicVolume(musicVol);
        SetSFXVolume(sfxVol);

        musicSlider.onValueChanged.AddListener(delegate { OnMusicSliderChanged(); });
        sfxSlider.onValueChanged.AddListener(delegate { OnSFXSliderChanged(); });
    }

    public void OpenSettingsMenuCanvas(){
        AudiosSettingsCanvas.SetActive(false);
        SettingsCanvas.SetActive(true);
    }

    private void SetMusicVolume(float volume){
        PlayerPrefs.SetFloat("MusicVolume", volume);
        musicSlider.value = volume;
    }

    private void SetSFXVolume(float volume){
        PlayerPrefs.SetFloat("SFXVolume", volume);
        sfxSlider.value = volume;
    }

    private void OnMusicSliderChanged(){
        float volume = musicSlider.value;
        Debug.Log("musicSlider.value : " + musicSlider.value);
        SetMusicVolume(volume);
    }

    private void OnSFXSliderChanged(){
        float volume = sfxSlider.value;
        Debug.Log("sfxSlider.value : " + sfxSlider.value);
        SetSFXVolume(volume);
    }
}