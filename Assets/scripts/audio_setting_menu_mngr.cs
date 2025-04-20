using UnityEngine;

public class audio_setting_menu_mngr : MonoBehaviour
{
    public GameObject SettingsCanvas;
    public GameObject AudiosSettingsCanvas;

    public void OpenSettingsMenuCanvas(){
        AudiosSettingsCanvas.SetActive(false);
        SettingsCanvas.SetActive(true);
    }
}