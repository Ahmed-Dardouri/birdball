using UnityEngine;

public class controls_setting_menu_mngr : MonoBehaviour
{
    public GameObject SettingsCanvas;
    public GameObject ControlsSettingsCanvas;

    public void OpenSettingsMenuCanvas(){
        ControlsSettingsCanvas.SetActive(false);
        SettingsCanvas.SetActive(true);
    }
}