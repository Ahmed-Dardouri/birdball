using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
public class MenuMngr : MonoBehaviour
{
    public GameObject mainMenuCanvas;
    public GameObject SettingsCanvas;

    public void StartGame()
    {
        Debug.Log("buttong clicked");
        SceneManager.LoadScene("birdball");
        
    }
    public void Quitgame(){
        Application.Quit();
    }

    public void OpenSettingMenu(){
        mainMenuCanvas.SetActive(false);
        SettingsCanvas.SetActive(true);
    }
}