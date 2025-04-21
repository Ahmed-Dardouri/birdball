using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;
using UnityEngine.UI;

public class logicManagerScript : MonoBehaviour
{
    public int playerScore;
    public TextMeshProUGUI scoreText;
    public TextMeshProUGUI scoreEffectText;
    public TextMeshProUGUI PBText;
    public TextMeshProUGUI HitMultText;
    public TextMeshProUGUI HitMultEffectText;
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
            int added_value = score * score_multipler;
            playerScore += added_value;
            StartCoroutine(runScoreEffect(added_value));
        }
        
        UpdateScore();
    }

    public void SetScoreMultiplier(int mult){
        int sc_mult_prev = score_multipler;
        if(mult <= 0){
            mult = 1;
        }
        score_multipler = mult;
        if(sc_mult_prev < score_multipler){
            int diff = score_multipler - sc_mult_prev;
            if(diff == 3){
                StartCoroutine(runHitMultEffect(3, new Color(0f, 0.675f, 0.96f)));
            }else if(diff == 2){
                StartCoroutine(runHitMultEffect(2, new Color(0.19f, 0.81f, 0.38f)));
            }
        }
    }

    IEnumerator runScoreEffect(int added_value){
        scoreEffectText.color = new Color(0.8117f, 0.4964f, 0.1725f);
        scoreEffectText.text = "+" + added_value.ToString();
        yield return new WaitForSeconds(0.7f);
        scoreEffectText.text = "";
    }
    
    IEnumerator runHitMultEffect(int value, Color color){
        Debug.Log("run effect mult");
        HitMultEffectText.color = color;
        HitMultEffectText.text = "+" + value.ToString();
        yield return new WaitForSeconds(0.7f);
        HitMultEffectText.text = "";
    }

    public void restartGame(){
        SceneManager.LoadScene(1);
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

    public void BacktoMenu(){
        SceneManager.LoadScene(0);
    }
}
