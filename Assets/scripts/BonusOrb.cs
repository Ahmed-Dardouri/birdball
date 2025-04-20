using UnityEngine;

public class BonusOrb : MonoBehaviour
{
    public enum BonusType { ScoreBoost_x3, ScoreBoost_x2, ScoreBoost_plus_3}
    public BonusType type;
    public float duration = 20f; /* seconds */
    private BonusManager _BonusManager;
    private SoundManager soundManager;

    void Start()
    {
        soundManager = GameObject.FindGameObjectWithTag("sound").GetComponent<SoundManager>();
        _BonusManager = GameObject.FindGameObjectWithTag("BonusManager").GetComponent<BonusManager>();
        
        // Self-destroy after 30 seconds if not collected
        Destroy(gameObject, 30f);
    }
    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("ball"))
        {
            soundManager.PlaySFX(soundManager.bonusSound);
            Debug.Log("ball hit bonus : " + type);
            _BonusManager.ActivateBonus(type, duration);
            Destroy(gameObject);
        }
    }
}
