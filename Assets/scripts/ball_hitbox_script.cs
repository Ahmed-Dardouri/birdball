using UnityEngine;

public class ball_hitbox_script : MonoBehaviour
{
    private bool game_over = false;

    void OnCollisionEnter2D(Collision2D collision){
        if(collision.gameObject.CompareTag("ground") && !game_over){
            Debug.Log("Game Over");
            game_over = true;
            GameObject.FindGameObjectWithTag("logic").GetComponent<logicManagerScript>().game_over();
        }
    }

    void OnCollisionExit2D(Collision2D collision)
    {
        if(collision.gameObject.CompareTag("bird") && !game_over){
            Debug.Log("hit");
            GameObject.FindGameObjectWithTag("logic").GetComponent<logicManagerScript>().addscore(1);
        }
    }
}
