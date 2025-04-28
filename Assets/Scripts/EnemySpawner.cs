using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class EnemySpawner : MonoBehaviour
{
    public GameObject enemyPrefab; 
    public Transform[] spawnPoints;
    public float spawnInterval = 40f; 
    public int baseEnemiesPerWave = 5; 
    public int maxWaves = 5; 

    private int currentWave = 0;
    private int enemiesAlive = 0;

    public GameObject winPanel;

    private void Awake()
    {
        winPanel = GameObject.Find("PausePanel");
    }

    void Start()
    {
        StartCoroutine(SpawnWaves());
    }

    IEnumerator SpawnWaves()
    {
        while (currentWave < maxWaves)
        {
            SpawnWave();
            yield return new WaitForSeconds(spawnInterval);
        }
    }

    void SpawnWave()
    {
        currentWave++;
        int enemiesToSpawn = baseEnemiesPerWave + (currentWave - 1) * 2; 
        enemiesAlive = enemiesToSpawn;

        for (int i = 0; i < enemiesToSpawn; i++)
        {
            Transform spawnPoint = spawnPoints[Random.Range(0, spawnPoints.Length)];
            GameObject enemy = Instantiate(enemyPrefab, spawnPoint.position, spawnPoint.rotation);
            enemy.GetComponent<EnemyController>().OnDeath += EnemyDied; 
        }
    }

    private void EnemyDied()
    {
        enemiesAlive--;

        
        if (currentWave == maxWaves && enemiesAlive <= 0)
        {
          
            StartCoroutine(ShowWinPanelAfterDelay(4f));
        }
    }

    private IEnumerator ShowWinPanelAfterDelay(float delay = 4f)
    {
     
        yield return new WaitForSeconds(delay);

     
        winPanel.SetActive(true);

       
        GameObject.Find("TitleText").GetComponent<Text>().text = "You Win";

    
        Time.timeScale = 0f; 
    }
}
