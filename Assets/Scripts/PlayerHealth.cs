using UnityEngine;
using System.Collections;

public class PlayerHealth : MonoBehaviour
{
    // 최대 생명력: 500 (하트 5개)
    public int maxHealth = 500;

    // 현재 생명력
    private int currentHealth;

    private HealthUI healthUI;

    public float invincibilityDuration = 1.5f; // 무적 지속 시간 (초)
    public float blinkInterval = 0.1f;         // 깜빡이는 간격 (빠를수록 더 자주 깜빡임)
    private SpriteRenderer spriteRenderer;
    private bool isInvincible = false;

    public GameObject hitEffectPrefab;


    void Start()
    {
        currentHealth = maxHealth;

        healthUI = FindObjectOfType<HealthUI>();
        if (healthUI != null)
        {
            healthUI.UpdateHealth(currentHealth);
        }

        spriteRenderer = GetComponent<SpriteRenderer>();
        if (spriteRenderer == null)
        {
            Debug.LogError("Player 오브젝트에 SpriteRenderer가 없습니다.");
        }
    }

    // ⭐ 몬스터가 호출할 데미지 함수
    public void TakeDamage(int damage)
    {
        if (isInvincible)
        {
            return;
        }

        currentHealth -= damage;

        // UI 업데이트 함수 호출
        if (healthUI != null)
        {
            healthUI.UpdateHealth(currentHealth);
        }

        if (currentHealth > 0)
        {
            // 1. 피격 이펙트 생성 (선택 사항)
            SpawnHitEffect();

            // 2. 깜빡임 코루틴 시작
            StartCoroutine(InvincibilityCoroutine());
        }

        if (currentHealth <= 0)
        {
            Die();
        }

        
    }
    IEnumerator InvincibilityCoroutine()
    {
        isInvincible = true;
        float timer = 0f;

        while (timer < invincibilityDuration)
        {
            // 스프라이트 껐다 켜기 (깜빡임 효과)
            if (spriteRenderer != null)
            {
                spriteRenderer.enabled = !spriteRenderer.enabled;
            }

            // 지정된 깜빡임 간격만큼 대기
            yield return new WaitForSeconds(blinkInterval);

            timer += blinkInterval;
        }

        // 코루틴 종료 후 원래 상태로 복귀
        if (spriteRenderer != null)
        {
            spriteRenderer.enabled = true; // 반드시 켜진 상태로 되돌립니다.
        }
        isInvincible = false;
    }

    void SpawnHitEffect()
    {
        if (hitEffectPrefab != null)
        {
            // 플레이어 위치에 이펙트 생성 (Quaternion.identity는 회전 없음)
            GameObject effect = Instantiate(hitEffectPrefab, transform.position, Quaternion.identity);

            // 이펙트가 재생된 후 자동으로 사라지게 설정 (보통 이펙트 자체 스크립트에 Destroy 로직이 있음)
            // 예를 들어, 1초 뒤에 파괴되도록 설정: Destroy(effect, 1f);
        }
    }

    private void Die()
    {
        // TODO: (선택 사항) 게임 오버 또는 리스폰 로직 구현

        gameObject.SetActive(false); // 플레이어 비활성화
    }

    // (선택 사항) 충돌 이펙트를 위한 함수 (예: 화면 흔들림, 깜빡임)
    public void ApplyHitEffect()
    {
        // TODO: 플레이어 피격 시 깜빡임(Flash) 애니메이션 또는 사운드 재생
    }
}
