using UnityEngine;

public class OrbSpawner : MonoBehaviour
{
    public GameObject[] bonusOrbPrefabs;
    public float spawnInterval = 5f;
    public Vector2 spawnAreaMin;
    public Vector2 spawnAreaMax;

    void Start()
    {
        InvokeRepeating("SpawnOrb", spawnInterval, spawnInterval);
    }

    void SpawnOrb()
    {
        Vector2 spawnPos = new Vector2(
            Random.Range(spawnAreaMin.x, spawnAreaMax.x),
            Random.Range(spawnAreaMin.y, spawnAreaMax.y)
        );

        int randomIndex = Random.Range(0, bonusOrbPrefabs.Length);
        Instantiate(bonusOrbPrefabs[randomIndex], spawnPos, Quaternion.identity);
    }
}