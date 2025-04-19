using System.Collections;
using UnityEngine;

public class BonusManager : MonoBehaviour
{
    public logicManagerScript logic;

    private int accumulated_multiplier = 1;

    void Start()
    {
        logic = GameObject.FindGameObjectWithTag("logic").GetComponent<logicManagerScript>();
    }

    public void ActivateBonus(BonusOrb.BonusType type, float duration)
    {
        StartCoroutine(BonusRoutine(type, duration));
    }

    IEnumerator BonusRoutine(BonusOrb.BonusType type, float duration)
    {
        switch (type)
        {
            case BonusOrb.BonusType.ScoreBoost_x3:
                accumulated_multiplier += 3;
                logic.SetScoreMultiplier(accumulated_multiplier);
                yield return new WaitForSeconds(duration);
                accumulated_multiplier -= 3;
                logic.SetScoreMultiplier(accumulated_multiplier);
                break;

            case BonusOrb.BonusType.ScoreBoost_x2:
                accumulated_multiplier += 2;
                logic.SetScoreMultiplier(accumulated_multiplier);
                yield return new WaitForSeconds(duration);
                accumulated_multiplier -= 2;
                logic.SetScoreMultiplier(accumulated_multiplier);
                break;

            case BonusOrb.BonusType.ScoreBoost_plus_3:
                logic.addscore(3);
                break;

            default:
                break;
        }
    }
}
