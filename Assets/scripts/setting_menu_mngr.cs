using UnityEngine;

public class setting_menu_mngr : MonoBehaviour
{
    public GameObject mainMenuCanvas;
    public GameObject SettingsCanvas;
    public GameObject ControlsSettingsCanvas;
    public GameObject AudiosSettingsCanvas;

    public void OpenControlsSettings(){
        SettingsCanvas.SetActive(false);
        ControlsSettingsCanvas.SetActive(true);
    }
    
    public void OpenAudioSettings(){
        SettingsCanvas.SetActive(false);
        AudiosSettingsCanvas.SetActive(true);
    }

    public void OpenMainMenuCanvas(){
        SettingsCanvas.SetActive(false);
        mainMenuCanvas.SetActive(true);
    }
}