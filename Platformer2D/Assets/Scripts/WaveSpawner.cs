﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaveSpawner : MonoBehaviour {
    public enum SpawnState { SPAWNING, WAITING, COUNTING };

    [System.Serializable]
    public class Wave {
        public string name;
        public Transform enemy;
        public int count;
        public float rate;
    }

    public Wave[] waves;
    public float timeBetweenWaves = 5f;
    public Transform[] spawnPoints;
    public int NextWave {
        get { return nextWave + 1; }
    }

    public float WaveCountdown {
        get { return waveCountdown + 1; }
    }
    public SpawnState State {
        get { return state;  }
    }

    private float waveCountdown;  
    private float searchCountdown = 1f;
    private int nextWave = 0;

    
    private SpawnState state = SpawnState.COUNTING;

    private void Start() {
        waveCountdown = timeBetweenWaves;

        if (spawnPoints.Length == 0) {
            Debug.LogError("NO enemy spawn points!");
        }
    }

    private void Update() {

        if (state == SpawnState.WAITING) {
            // Check if enemies are still alive
            if (!EnemyIsAlive()) {
                WaveCompleted();
            }
            else {
                return;
            }
        }

        if (waveCountdown <= 0) {
            if (state != SpawnState.SPAWNING) {
                StartCoroutine(SpawnWave(waves[nextWave]));
            }
        }
        else {
            waveCountdown -= Time.deltaTime;
        }
    }

    void WaveCompleted() {
        Debug.Log("Wave Completed");

        state = SpawnState.COUNTING;
        waveCountdown = timeBetweenWaves;

        if(nextWave + 1 > waves.Length - 1) {
            nextWave = 0;
            Debug.Log("ALL WAVES COMPLETED! Looping...");
        }
        else {
            nextWave++;
        }
        
    }

    bool EnemyIsAlive() {
        searchCountdown -= Time.deltaTime;
        if(searchCountdown <= 0f) {
            searchCountdown = 1f;
            if (GameObject.FindGameObjectWithTag("Enemy") == null) {
                return false;
            }
        }
        return true;
    }

    IEnumerator SpawnWave(Wave _wave) {
        Debug.Log("Spawning Wave " + _wave.name);
        state = SpawnState.SPAWNING;

        for(int i=0; i <_wave.count; i++) {
            SpawnEnemy(_wave.enemy);
            yield return new WaitForSeconds(1f / _wave.rate);
        }

        state = SpawnState.WAITING;
        yield break;
    }

    void SpawnEnemy(Transform _enemy) {
        Debug.Log("Spawning Enemy: " + _enemy.name);

        Transform _sp = spawnPoints[Random.Range(0, spawnPoints.Length)];
        Instantiate(_enemy, _sp.position, _sp.rotation);
    }

}
