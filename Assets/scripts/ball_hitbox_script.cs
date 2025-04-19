using UnityEngine;

public class ball_hitbox_script : MonoBehaviour
{
    private bool game_over = false;
    private bool bird_is_jumping_prev = false;
    private bool bird_is_jumping = false;
    private bool bird_jump_to_be_consumed = true;

    void Update()
    {
        check_bird_jump();
    }


    void OnTriggerEnter2D(Collider2D collision){
        if(collision.gameObject.CompareTag("ground") && !game_over){
            Debug.Log("Game Over");
            game_over = true;
            GameObject.FindGameObjectWithTag("logic").GetComponent<logicManagerScript>().game_over();

        }
    }
    void OnTriggerExit2D(Collider2D collision){
        if(collision.gameObject.CompareTag("bird") && !game_over){
            if(bird_jump_to_be_consumed && bird_is_jumping){
                bird_jump_to_be_consumed = false;
                Debug.Log("hit");
                GameObject.FindGameObjectWithTag("logic").GetComponent<logicManagerScript>().addscore(1);   
            }
        }
    }

    private void check_bird_jump(){
        
        bird_is_jumping  = GameObject.FindGameObjectWithTag("bird").GetComponent<PlayerController>().IsJumping();
        
        if(!bird_is_jumping_prev && bird_is_jumping){
            // just started jumping
            bird_jump_to_be_consumed = true;
        }
        bird_is_jumping_prev = bird_is_jumping;
    }
}
