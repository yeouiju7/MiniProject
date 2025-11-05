using UnityEngine;

public class EnemySpawnManager : MonoBehaviour
{
    // Inspector에 맵 2에 배치할 모든 몬스터 오브젝트를 연결합니다.
    public GameObject[] enemiesToSpawn;

    // 맵 2의 경계선(트리거)에 붙여서 사용합니다.
    // 플레이어가 맵 2로 진입하는 순간을 감지합니다.
    private void OnTriggerEnter2D(Collider2D other)
    {
        // 플레이어 태그 확인
        if (other.CompareTag("Player"))
        {
            SpawnEnemies();
        }
    }

    private void SpawnEnemies()
    {
        // 몬스터가 이미 활성화된 경우 중복 실행 방지
        if (enemiesToSpawn.Length > 0 && enemiesToSpawn[0].activeSelf)
        {
            return;
        }

        foreach (GameObject enemy in enemiesToSpawn)
        {
            if (enemy != null)
            {
                // 몬스터 오브젝트를 활성화하여 출현시킵니다.
                enemy.SetActive(true);
            }
        }

        // 스폰 후 이 트리거를 비활성화 (한 번만 출현시키기 위함)
        Destroy(gameObject);
    }
}