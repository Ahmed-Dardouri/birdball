using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class logicManagerScript : MonoBehaviour
{
    public int playerScore;
    public TextMeshProUGUI scoreText;
    public TextMeshProUGUI PBText;
    public TextMeshProUGUI HitMultText;
    public GameObject gameOverScreen;
    private bool game_is_over;

    private int score_multipler = 1;
    
    private const string PBKey = "PB";

    void Start()
    {
        game_is_over = false;

        /* read personal best score */
        int PB = PlayerPrefs.GetInt(PBKey, 0);
        UpdatePBUI(PB);
    }

    void Update()
    {
        if(game_is_over && Input.GetKeyDown(KeyCode.Space) == true){
            restartGame();
        }  

        UpdateHitMultText();
    }

    public void addscore(int score){
        if(game_is_over == false){
            if(score <= 0){
                score = 1;
            }
            playerScore += score * score_multipler;
        }
        
        UpdateScore();
    }

    public void SetScoreMultiplier(int mult){
        if(mult <= 0){
            mult = 1;
        }
        score_multipler = mult;
    }

    public void restartGame(){
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        Time.timeScale = 1f;
    }

    public void game_over(){
        game_is_over = true;
        gameOverScreen.SetActive(true);
        Time.timeScale = 0f;
        CheckAndSavePB();
    }

    public int getScore(){
        return playerScore;
    }

    public void CheckAndSavePB()
    {
        int savedPB = PlayerPrefs.GetInt(PBKey, 0);
        
        if (playerScore > savedPB)
        {
            PlayerPrefs.SetInt(PBKey, playerScore);
            PlayerPrefs.Save(); 
            UpdatePBUI(playerScore);
        }
    }

    private void UpdateScore(){
        scoreText.text = "Score: " + playerScore.ToString();
    }

    private void UpdatePBUI(int score)
    {
        if(PBText != null)
        {
            PBText.text = "Personal Best: " + score.ToString();
        }
    }
    
    private void UpdateHitMultText()
    {
        if(HitMultText != null)
        {
            HitMultText.text = "x" + score_multipler.ToString();
        }
    }
}
