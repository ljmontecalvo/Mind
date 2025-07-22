using System.Collections.Generic;
using UnityEngine;

public class PulseWave : MonoBehaviour
{
    public GameObject tokenPrefab;
    public float spawnRadiusMax;
    public float spawnRadiusMin;

    private List<GameObject> tokens = new List<GameObject>();

    public void Start()
    {
        for (int i = 0; i < 100; i++)
        {
            float spawnPointX = Random.Range(-spawnRadiusMax, spawnRadiusMax);
            float spawnPointY = Random.Range(-spawnRadiusMax, spawnRadiusMax);

            Vector3 randomPosition = new Vector3(spawnPointX, spawnPointY, 0);
            float spawnPointDistance = Vector3.Distance(gameObject.transform.position, randomPosition);

            if (spawnPointDistance > spawnRadiusMin && spawnPointDistance < spawnRadiusMax)
            {
                tokens.Add(Instantiate(tokenPrefab, randomPosition, Quaternion.identity));
            }
            else { i--; }
        }
    }
}
