using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class EnemySpawner : MonoBehaviour
{
    public GameObject enemyPrefab; // Префаб врага
    public Transform[] spawnPoints; // Массив точек спавна (3 штуки)
    public float spawnInterval = 40f; // Интервал между волнами
    public int baseEnemiesPerWave = 5; // Начальное количество врагов
    public int maxWaves = 5; // Количество волн

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
        int enemiesToSpawn = baseEnemiesPerWave + (currentWave - 1) * 2; // +2 врага каждую волну
        enemiesAlive = enemiesToSpawn;

        for (int i = 0; i < enemiesToSpawn; i++)
        {
            Transform spawnPoint = spawnPoints[Random.Range(0, spawnPoints.Length)];
            GameObject enemy = Instantiate(enemyPrefab, spawnPoint.position, spawnPoint.rotation);
            enemy.GetComponent<EnemyController>().OnDeath += EnemyDied; // Подписываемся на событие смерти
        }
    }

    private void EnemyDied()
    {
        enemiesAlive--;

        // Проверка, когда игрок побеждает, например, если враги мертвы
        if (currentWave == maxWaves && enemiesAlive <= 0)
        {
            // Запуск корутины с задержкой 4 секунды перед тем, как показать winPanel
            StartCoroutine(ShowWinPanelAfterDelay(4f));
        }
    }

    private IEnumerator ShowWinPanelAfterDelay(float delay = 4f)
    {
        // Ожидание 4 секунды
        yield return new WaitForSeconds(delay);

        // Отображаем панель победы
        winPanel.SetActive(true);

        // Меняем текст на "You Win"
        GameObject.Find("TitleText").GetComponent<Text>().text = "You Win";

        // Останавливаем игру (или можно оставить для продолжения игры)
        Time.timeScale = 0f; // При необходимости остановить игру
    }
}
