using UnityEngine;

public class EnemyAI : MonoBehaviour
{
    // Inspector에서 설정할 몬스터의 이동 속도 (2 픽셀/프레임에 가까운 값)
    public float moveSpeed = 2f;

    // Inspector에서 설정할 이동 제한 범위 (시작 지점 기준)
    public float moveRange = 2f;

    // 플레이어의 Health 스크립트 연결 (충돌 시 데미지 부여용)
    public PlayerHealth playerHealth;

    // 몬스터의 시작 위치
    private Vector3 startPosition;

    // 이동 방향 (1: 오른쪽, -1: 왼쪽)
    private int direction = 1;

    public float lifetime = 10f;


    void Start()
    {
        startPosition = transform.position;

        // 씬에서 PlayerHealth 컴포넌트를 찾아 연결
        playerHealth = FindObjectOfType<PlayerHealth>();

        Invoke("SelfDestruct", lifetime);
    }

    void Update()
    {
        //1. 제한된 범위 내 이동
        MoveWithinRange();
    }

    void SelfDestruct()
    {
        Destroy(gameObject);
    }

    void MoveWithinRange()
    {
        // 현재 위치와 시작 위치의 차이
        float travelDistance = transform.position.x - startPosition.x;

        // 이동 방향 전환 조건 확인
        if (travelDistance >= moveRange)
        {
            direction = -1; // 왼쪽으로 전환
        }
        else if (travelDistance <= -moveRange)
        {
            direction = 1;  // 오른쪽으로 전환
        }

        // 새로운 위치 계산
        Vector3 newPosition = transform.position + Vector3.right * direction * moveSpeed * Time.deltaTime;
        transform.position = newPosition;

        // 몬스터 스프라이트 방향 뒤집기 (선택 사항)
        if (direction == 1)
        {
            transform.localScale = new Vector3(1, 1, 1);
        }
        else if (direction == -1)
        {
            transform.localScale = new Vector3(-1, 1, 1);
        }
    }

    // 2. 플레이어와 충돌 시
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            Destroy(gameObject);

            // 데미지 적용 및 이펙트 호출
            if (playerHealth != null)
            {
                playerHealth.TakeDamage(50);

            }
        }
    }
}