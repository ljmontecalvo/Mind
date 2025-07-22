using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PulseWave : MonoBehaviour
{
    [Header("Token Settings")]
    public GameObject tokenPrefab;
    public int tokenCount;
    public float spawnRadiusMax;
    public float spawnRadiusMin;
    
    [Header("Barb Obstacle Settings")]
    public GameObject barbObstaclePrefab;
    public int barbObstacleCount;
    
    [Header("Performance Settings")]
    [Tooltip("Maximum tokens to spawn per frame (0 = spawn all at once)")]
    public int tokensPerFrame;
    
    [Header("Spacing Settings")]
    [Tooltip("Minimum distance between tokens")]
    public float minTokenDistance = 1f;
    [Tooltip("Maximum attempts to find a valid position for each token")]
    public int maxPlacementAttempts = 50;
    
    private List<GameObject> tokens = new List<GameObject>();
    private List<GameObject> barbObstacles = new List<GameObject>();
    private List<Vector3> tokenPositions = new List<Vector3>();
    private List<Vector3> allObjectPositions = new List<Vector3>();
    private Vector3 centerPosition;
    
    private float minRadiusSquared;
    private float maxRadiusSquared;

    private void Start()
    {
        centerPosition = transform.position;
        minRadiusSquared = spawnRadiusMin * spawnRadiusMin;
        maxRadiusSquared = spawnRadiusMax * spawnRadiusMax;
        
        tokens.Capacity = tokenCount;
        tokenPositions.Capacity = tokenCount;
        barbObstacles.Capacity = barbObstacleCount;
        allObjectPositions.Capacity = tokenCount + barbObstacleCount;
        
        if (spawnRadiusMin >= spawnRadiusMax)
        {
            Debug.LogError("spawnRadiusMin must be less than spawnRadiusMax!");
            return;
        }
        
        if (tokenPrefab == null)
        {
            Debug.LogError("Token prefab is not assigned!");
            return;
        }
        
        if (tokensPerFrame <= 0 || tokenCount <= tokensPerFrame)
        {
            SpawnAllTokens();
            SpawnAllBarbObstacles();
        }
        else
        {
            StartCoroutine(SpawnTokensOverTime());
        }
    }
    
    private void SpawnAllTokens()
    {
        for (int i = 0; i < tokenCount; i++)
        {
            Vector3 randomPosition = GetValidTokenPosition();
            if (randomPosition != Vector3.zero)
            {
                GameObject newToken = Instantiate(tokenPrefab, randomPosition, Quaternion.identity);
                tokens.Add(newToken);
                tokenPositions.Add(randomPosition);
                allObjectPositions.Add(randomPosition);
            }
            else
            {
                Debug.LogWarning($"Could not find valid position for token {i + 1}. Skipping...");
            }
        }
    }
    
    private IEnumerator SpawnTokensOverTime()
    {
        int spawnedCount = 0;
        
        while (spawnedCount < tokenCount)
        {
            int tokensThisFrame = Mathf.Min(tokensPerFrame, tokenCount - spawnedCount);
            
            for (int i = 0; i < tokensThisFrame; i++)
            {
                Vector3 randomPosition = GetValidTokenPosition();
                if (randomPosition != Vector3.zero)
                {
                    GameObject newToken = Instantiate(tokenPrefab, randomPosition, Quaternion.identity);
                    tokens.Add(newToken);
                    tokenPositions.Add(randomPosition);
                    allObjectPositions.Add(randomPosition);
                    spawnedCount++;
                }
                else
                {
                    Debug.LogWarning($"Could not find valid position for token {spawnedCount + 1}. Skipping...");
                    spawnedCount++;
                }
            }
            
            yield return null;
        }
        
        SpawnAllBarbObstacles();
    }
    
    private void SpawnAllBarbObstacles()
    {
        if (barbObstaclePrefab == null || barbObstacleCount <= 0)
            return;
            
        for (int i = 0; i < barbObstacleCount; i++)
        {
            Vector3 randomPosition = GetValidObstaclePosition();
            if (randomPosition != Vector3.zero)
            {
                GameObject newObstacle = Instantiate(barbObstaclePrefab, randomPosition, Quaternion.identity);
                barbObstacles.Add(newObstacle);
                allObjectPositions.Add(randomPosition);
            }
            else
            {
                Debug.LogWarning($"Could not find valid position for barb obstacle {i + 1}. Skipping...");
            }
        }
    }
    
    private Vector3 GetValidTokenPosition()
    {
        for (int attempt = 0; attempt < maxPlacementAttempts; attempt++)
        {
            Vector3 candidatePosition = GetRandomPositionInAnnulus();
            
            bool isValidPosition = true;
            for (int i = 0; i < tokenPositions.Count; i++)
            {
                float distanceSquared = (candidatePosition - tokenPositions[i]).sqrMagnitude;
                if (distanceSquared < minTokenDistance * minTokenDistance)
                {
                    isValidPosition = false;
                    break;
                }
            }
            
            if (isValidPosition)
            {
                return candidatePosition;
            }
        }
        
        return Vector3.zero;
    }
    
    private Vector3 GetValidObstaclePosition()
    {
        for (int attempt = 0; attempt < maxPlacementAttempts; attempt++)
        {
            Vector3 candidatePosition = GetRandomPositionInAnnulus();
            
            bool isValidPosition = true;
            for (int i = 0; i < allObjectPositions.Count; i++)
            {
                float distanceSquared = (candidatePosition - allObjectPositions[i]).sqrMagnitude;
                if (distanceSquared < minTokenDistance * minTokenDistance)
                {
                    isValidPosition = false;
                    break;
                }
            }
            
            if (isValidPosition)
            {
                return candidatePosition;
            }
        }
        
        return Vector3.zero;
    }
    
    private Vector3 GetRandomPositionInAnnulus()
    {
        float randomRadius = Mathf.Sqrt(Random.Range(minRadiusSquared, maxRadiusSquared));
        
        float randomAngle = Random.Range(0f, 2f * Mathf.PI);
        
        float x = centerPosition.x + randomRadius * Mathf.Cos(randomAngle);
        float y = centerPosition.y + randomRadius * Mathf.Sin(randomAngle);
        
        return new Vector3(x, y, centerPosition.z);
    }
    
    private Vector3 GetRandomPositionInAnnulus_Alternative()
    {
        Vector3 randomPosition;
        float distanceSquared;
        
        int maxAttempts = 100;
        int attempts = 0;
        
        do
        {
            float x = Random.Range(-spawnRadiusMax, spawnRadiusMax);
            float y = Random.Range(-spawnRadiusMax, spawnRadiusMax);
            randomPosition = new Vector3(centerPosition.x + x, centerPosition.y + y, centerPosition.z);
            
            distanceSquared = (randomPosition - centerPosition).sqrMagnitude;
            attempts++;
            
        } while ((distanceSquared < minRadiusSquared || distanceSquared > maxRadiusSquared) && attempts < maxAttempts);
        
        if (attempts >= maxAttempts)
        {
            Debug.LogWarning("Could not find valid spawn position after maximum attempts. Using fallback position.");
            float fallbackRadius = (spawnRadiusMin + spawnRadiusMax) * 0.5f;
            float fallbackAngle = Random.Range(0f, 2f * Mathf.PI);
            randomPosition = centerPosition + new Vector3(
                fallbackRadius * Mathf.Cos(fallbackAngle),
                fallbackRadius * Mathf.Sin(fallbackAngle),
                0f
            );
        }
        
        return randomPosition;
    }
    
    private void OnDestroy()
    {
        foreach (GameObject token in tokens)
        {
            if (token != null)
            {
                DestroyImmediate(token);
            }
        }
        tokens.Clear();
        
        foreach (GameObject obstacle in barbObstacles)
        {
            if (obstacle != null)
            {
                DestroyImmediate(obstacle);
            }
        }
        barbObstacles.Clear();
    }
}
