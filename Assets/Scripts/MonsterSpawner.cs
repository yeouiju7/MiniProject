using UnityEngine;

public class MonsterSpawner : MonoBehaviour
{
    public GameObject[] enemyPrefabs;
    public float spawnInterval = 5f;
    private Vector3 spawnPosition;

    void Start()
    {
        spawnPosition = transform.position;

        // 일정 시간마다 몬스터 생성 함수 반복 호출
        InvokeRepeating("SpawnEnemy", 0f, spawnInterval);
    }

    void SpawnEnemy()
    {
        //1. 프리팹 배열이 비어있는지 확인
        if (enemyPrefabs.Length == 0)
        {
            Debug.LogError("Spawnable enemy prefabs not set on " + gameObject.name);
            return;
        }

        // 2. 배열에서 무작위로 하나의 몬스터 프리팹 선택
        int randomIndex = Random.Range(0, enemyPrefabs.Length);
        GameObject prefabToSpawn = enemyPrefabs[randomIndex];

        //3. 몬스터 생성
        GameObject newEnemy = Instantiate(prefabToSpawn, spawnPosition, Quaternion.identity);
    }
}