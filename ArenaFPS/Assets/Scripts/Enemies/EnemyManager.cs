using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EnemyManager : MonoBehaviour
{
    [Header("RED")] public float lookDistance;
    [Header("BLUE")] public float wanderRadius;
    public float wanderSpeed;
    public float chaseSpeed;
    public float loseThreshold;
    public float timeForNewPath;
    public float health;
    public float fov;

    [SerializeField] private Text timerText;
    [SerializeField] private Text waveCountText;
    [SerializeField] private Text killCountText;
    [SerializeField] private Text playerHealthText;

    public float time;

    private float waveCount;
    [SerializeField] private GameObject enemy;
    [SerializeField] private float secondsBetweenWaves;
    [SerializeField] private float waveRate;
    [SerializeField] private float spawnRadius;
    [SerializeField] private float countdown;
    [SerializeField] private float nextWaveEnemies;

    private static float killCount;
    public static float playerHealth = 100;

    [SerializeField] private GameObject[] spawnPoints;

    void Start()
    {
        spawnPoints = GameObject.FindGameObjectsWithTag("EnemySpawn");
    }

    void Update()
    {
        time += Time.deltaTime;
        UpdateAllText();

        if(countdown <= 0f)
        {
            StartCoroutine(SpawnWave());
            countdown = secondsBetweenWaves;
            return;
        }
        countdown -= Time.deltaTime;
        countdown = Mathf.Clamp(countdown, 0f, Mathf.Infinity);

    }
    private void UpdateAllText()
    {
        timerText.text = (int)(time / 60) + ":" + (int)(time % 60);
        waveCountText.text = "Wave " + waveCount;
        killCountText.text = "Killed " + killCount;
        playerHealthText.text = "HP: " + playerHealth;
    }
    public static void AddKill()
    {
        killCount += 1;
    }

    private IEnumerator SpawnWave()
    {
        for(int i = 0; i < nextWaveEnemies; i++)
        {
            int spawnNum = Random.Range(0, spawnPoints.Length);
            Quaternion spawnRot = Quaternion.identity;
            GameObject enemyObj = Instantiate(enemy, spawnPoints[spawnNum].transform.position + (Random.insideUnitSphere * spawnRadius), spawnRot);
            enemyObj.transform.parent = GameObject.Find("Enemies").transform;
            yield return new WaitForSeconds(1f / waveRate);
        }
        nextWaveEnemies += waveCount;
        waveCount++;
    }
}
