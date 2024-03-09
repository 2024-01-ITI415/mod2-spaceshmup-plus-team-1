using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

using System.Linq;
using UnityEngine.UI;


public class Main : MonoBehaviour {

    static public Main S; // A singleton for Main
    static Dictionary<WeaponType, WeaponDefinition> WEAP_DICT;

    [Header("Set in Inspector")]
    public GameObject[] prefabEnemies; // Array of Enemy prefabs
    public float enemySpawnPerSecond = 0.5f; // # Enemies/second
    public float enemyDefaultPadding = 1.5f; // Padding for position
    public WeaponDefinition[] weaponDefinitions;
    public GameObject prefabPowerUp;
    public Spawner spawner; 
    public WeaponType[] powerUpFrequency = new WeaponType[]
    {
        WeaponType.blaster, WeaponType.blaster, WeaponType.spread, WeaponType.shield
    };

    private BoundsCheck bndCheck;
  

    public void ShipDestroyed( Enemy e)
    {
        // Potentially generate a PowerUp
        if (Random.value <= e.powerUpDropChance)
        {
            // Choose which PowerUp to pick
            // Pick one from the possibilities in powerUpFrequency
            int ndx = Random.Range(0, powerUpFrequency.Length);
            WeaponType puType = powerUpFrequency[ndx];
            // Spawn a PowerUp
            GameObject go = Instantiate(prefabPowerUp) as GameObject;
            PowerUp pu = go.GetComponent<PowerUp>();
            // Set it to the proper WeaponType
            pu.SetType(puType);

            // Set it to the position of the destroyed ship
            pu.transform.position = e.transform.position;
        }
    }

    private void Awake()
    {
        S = this;
        // Set bndCheck to reference the BoundsCheck component on this GameObject
        bndCheck = GetComponent<BoundsCheck>();

        // Invoke SpawnEnemy() once (in 2 seconds, based on default values)
        Invoke("SpawnEnemy", 1f / enemySpawnPerSecond);

        // A generic Dictionary with WeaponType as the key
        WEAP_DICT = new Dictionary<WeaponType, WeaponDefinition>();
        foreach(WeaponDefinition def in weaponDefinitions)
        {
            WEAP_DICT[def.type] = def;
        }
    }

    public void SpawnEnemy()
    {
        // Pick a random Enemy prefab to instantiate
        int ndx = Random.Range(0, prefabEnemies.Length);
        GameObject go = Instantiate<GameObject>(prefabEnemies[ndx]);

        // Position the ENemy above the screen with a random x position
        float enemyPadding = enemyDefaultPadding;
        if (go.GetComponent<BoundsCheck>() != null)
        {
            enemyPadding = Mathf.Abs(go.GetComponent<BoundsCheck>().radius);
        }

        // Set the initial position for the spawned Enemy
        Vector3 pos = Vector3.zero;
        float xMin = -bndCheck.camWidth + enemyPadding;
        float xMax = bndCheck.camWidth - enemyPadding;
        pos.x = Random.Range(xMin, xMax);
        pos.y = bndCheck.camHeight + enemyPadding;
        go.transform.position = pos;

        // Invoke SpawnEnemy() again
        Invoke("SpawnEnemy", 1f / enemySpawnPerSecond);
    }

    public void DelayedRestart(float delay)
    {
        // Invoke the Restart() method in delay seconds
        Invoke("Restart", delay);
    }

    public void Restart()
    {
       
        SceneManager.LoadScene("_Scene_0");
    }
    
    static public WeaponDefinition GetWeaponDefinition(WeaponType wt)
    {
        
        if (WEAP_DICT.ContainsKey(wt))
        {
            return (WEAP_DICT[wt]);
        }
        
        return new WeaponDefinition();
    }
  
/// <summary>
///  Enemy wave guess
///  attempt on a wave
/// </summary>
   
    public class WaveEnemy
    {
        public int Amount;
        public GameObject Type;
    }

    [System.Serializable]
    public class Wave
    {
        public List<WaveEnemy> Enemy;
    }

    [System.Serializable]
    public class Spawner
    {
        public List<Wave> Waves;
        public int CurrentWave = 0;

        void CreateNextEnemyWave()
        {
            var wave = Waves.ElementAtOrDefault(CurrentWave);

            if (wave != null)
            {
                CreateEnemiesFor(wave);
                CurrentWave++;
            }
        }

        void CreateEnemiesFor(Wave wave)
        {
            if (!wave.Enemy.Any()) return;

            foreach (var enemy in wave.Enemy)
            {
                for (int i = 0; i < enemy.Amount; i++)
                {
                    Instantiate(enemy.Type, Vector3.zero, Quaternion.identity);
                  
                }
            }
        }

        void Update()
        {
            if (Input.GetKeyDown(KeyCode.Space)) CreateNextEnemyWave();
        }
    }

   
}



/// list varioavble publicv - set waves in inspector (5 waves)
/// inside the enemy report score value back to the main class ( in enemy script)
/// implement scoring
///   GameObject.tGo = 
