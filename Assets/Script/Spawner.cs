﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spawner : MonoBehaviour {

    public bool devMode;

    public Wave[] waves;
    public Enemy enemy;

    LivingEntity playerEntity;
    Transform playerT;

    Wave currentWave;
    int currentWaveNumber;

    int enemiesRemainingToSpawn;
    int enemiesRemainingAlive;
    float nextSpawnTime;

    MapGenerator map;

    //player가 한 자리에 오래 머무르는지 체크
    float timeBetweenCampingChecks = 2;
    float campThresholdDistance = 1.5f;
    float nextCampCheckTime;
    Vector3 campPositionOld;
    bool isCamping;

    bool isDisabled; //player의 생사여부

    public event System.Action<int> OnNewWave; 

    private void Start()
    {
        playerEntity = FindObjectOfType<Player>();
        playerT = playerEntity.transform;

        nextCampCheckTime = timeBetweenCampingChecks + Time.time;
        campPositionOld = playerT.position;
        playerEntity.OnDeath += OnPlayerDeath;

        map = FindObjectOfType<MapGenerator>();
        NextWave();
    }

    private void Update()
    {
        if (isDisabled) return;

        if(Time.time > nextCampCheckTime)
        {
            nextCampCheckTime = Time.time + timeBetweenCampingChecks;

            isCamping = (Vector3.Distance(playerT.position, campPositionOld) < campThresholdDistance);
            campPositionOld = playerT.position;
        }

        if((enemiesRemainingToSpawn > 0 || currentWave.infinite) && Time.time > nextSpawnTime)
        {
            enemiesRemainingToSpawn--;
            nextSpawnTime = Time.time + currentWave.timeBetweenSpawns;

            StartCoroutine("SpawnEnemy");
        }

        if(devMode)
        {
            if(Input.GetKeyDown(KeyCode.Return))
            {
                StopCoroutine("SpawnEnemy");
                foreach(Enemy enemy in FindObjectsOfType<Enemy>())
                {
                    GameObject.Destroy(enemy.gameObject);
                }
                NextWave();
            }
        }
    }

    IEnumerator SpawnEnemy()
    {
        float spawnDelay = 1;
        float tileflashSpeed = 4;

        Transform randomTile = map.GetRandomOpenTile();

        if (isCamping)
            randomTile = map.GetTileFromPosition(playerT.position);

        Material tileMat = randomTile.GetComponent<Renderer>().material;
        Color initialColor = Color.white;
        Color flashColor = Color.red;
        float spawnTimer = 0;

        while(spawnTimer < spawnDelay)
        {
            tileMat.color = Color.Lerp(initialColor, flashColor, Mathf.PingPong(spawnTimer * tileflashSpeed, 1));
            spawnTimer += Time.deltaTime;
            yield return null;
        }

        //Enemy spawnedEnemy = Instantiate(enemy, randomTile.position + Vector3.up, Quaternion.identity) as Enemy;
        Enemy spawnedEnemy = ObjectPool.Instance().GetObject("Enemy") as Enemy;
        spawnedEnemy.SetCharacteristics(randomTile.position + Vector3.up, currentWave.moveSpeed, currentWave.hitsToKillPlayer, currentWave.enemyHealth, currentWave.skinColor);
    }

    void OnPlayerDeath()
    {
        isDisabled = true;
    }

    public void OnEnemyDeath()
    {
        enemiesRemainingAlive--;
        Debug.Log(enemiesRemainingAlive);

        if(enemiesRemainingAlive == 0)
        {
            NextWave();
        }
    }

    void ResetPlayerPosition()
    {
        playerT.position = map.GetTileFromPosition(Vector3.zero).position + Vector3.up * 3;
    }

    void NextWave()
    {
        if(currentWaveNumber > 0)
        {
            AudioManager.instance.PlaySound2D("Level Complete");
        }
        currentWaveNumber++;

        if(currentWaveNumber - 1 < waves.Length)
        {
            currentWave = waves[currentWaveNumber - 1];

            enemiesRemainingToSpawn = currentWave.enemyCount;
            enemiesRemainingAlive = enemiesRemainingToSpawn;

            if(OnNewWave != null)
            {
                OnNewWave(currentWaveNumber);
            }

            ResetPlayerPosition();
        }
        StopCoroutine("SpawnEnemy");
        ObjectPool.Instance().ReturnAllObject();
    }

    [System.Serializable]
    public class Wave
    {
        public bool infinite;
        public int enemyCount;
        public float timeBetweenSpawns;

        public float moveSpeed;
        public int hitsToKillPlayer;
        public float enemyHealth;
        public Color skinColor;
    }

}
