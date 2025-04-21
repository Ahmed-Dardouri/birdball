using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
public class MenuMngr : MonoBehaviour
{
    public GameObject mainMenuCanvas;
    public GameObject SettingsCanvas;
    public GameObject[] control_settings;

    public void StartGame()
    {
        Debug.Log("buttong clicked");
        for(int i = 0; i < control_settings.Length; i++){
            if(control_settings[i] != null){
                if(control_settings[i].TryGetComponent<InputRebinder>(out var control))
                {
                    control.LoadRebinds();
                }
            }
        }
        SceneManager.LoadScene("birdball");
        Time.timeScale = 1f;
        
    }
    public void Quitgame(){
        Application.Quit();
    }

    public void OpenSettingMenu(){
        mainMenuCanvas.SetActive(false);
        SettingsCanvas.SetActive(true);
    }
}