using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
public class MenuMngr : MonoBehaviour
{
    public void StartGame()
    {
        Debug.Log("buttong clicked");
        SceneManager.LoadScene("birdball");
        
    }
    public void quitgame(){
        Application.Quit();
    }
}