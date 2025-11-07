using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

public class PlayerHealth : MonoBehaviour
{

    public int maxHealth = 500;
    private int currentHealth;

    private HealthUI healthUI;

    public float invincibilityDuration = 1.5f; // 무적 지속 시간
    public float blinkInterval = 0.1f; 
    private SpriteRenderer spriteRenderer;
    private bool isInvincible = false;

    public GameObject hitEffectPrefab;

    [Header("Respawn & Scene")]
    public Vector3 respawnPoint; 
    public string gameOverSceneName = "Over"; 


    void Start()
    {
        respawnPoint = transform.position;
        currentHealth = maxHealth;

        healthUI = FindObjectOfType<HealthUI>();
        if (healthUI != null)
        {
            healthUI.UpdateHealth(currentHealth);
        }

        spriteRenderer = GetComponent<SpriteRenderer>();
        if (spriteRenderer == null)
        {
          
        }


    }

    public void DieByFall()
    {
        const int FALL_DAMAGE = 100; // 낙사 대미지: 100 (하트 1개)

        // 체력 감소
        currentHealth -= FALL_DAMAGE;

        // UI 업데이트
        if (healthUI != null)
        {
            healthUI.UpdateHealth(currentHealth);
        }


        if (currentHealth > 0)
        {
            // 1. 리스폰
            transform.position = respawnPoint;
            // 2. 리스폰 후 무적 상태 부여 (기존 코루틴 재사용)
            StartCoroutine(InvincibilityCoroutine());
        }
        else
        {
            // 3. 게임 오버
            Die();
        }
    }

    //몬스터가 호출할 데미지 함수
    public void TakeDamage(int damage)
    {
        if (isInvincible)
        {
            return;
        }

        currentHealth -= damage;

        if (healthUI != null)
        {
            healthUI.UpdateHealth(currentHealth);
        }

        if (currentHealth > 0)
        {
            SpawnHitEffect();  
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
            // 스프라이트 껐다 켜기
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
            spriteRenderer.enabled = true; //켜진 상태로 되돌리기
        }
        isInvincible = false;
    }

    void SpawnHitEffect()
    {
        if (hitEffectPrefab != null)
        {
            // 플레이어 위치에 이펙트 생성
            GameObject effect = Instantiate(hitEffectPrefab, transform.position, Quaternion.identity);

        }
    }

    private void Die()
    {

        gameObject.SetActive(false); // 플레이어 비활성화

        Time.timeScale = 1f;
        SceneManager.LoadScene(gameOverSceneName);
    }

    public void ApplyHitEffect()
    {
      
    }
}
