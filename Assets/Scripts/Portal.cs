using UnityEngine;

public class Portal : MonoBehaviour
{
    
    public Portal destinationPortal;
    private bool isPlayerInside = false;

    public AudioSource currentBGM;   // 현재 BGM (예: BGM_Map1)
    public AudioSource nextBGM;      // 다음 맵 BGM (예: BGM_Map2)

    private AudioSource audioSource;

    void Start()
    {
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
        {
          
        }
    }

    private void OnTriggerEnter2D(Collider2D other) 
    {

        if (other.CompareTag("Player"))
        {
            // 1. 효과음 재생
            if (audioSource != null && audioSource.clip != null)
            {
                audioSource.Play();
            }

        }
        // 충돌한 오브젝트가 플레이어인지 확인
        if (other.CompareTag("Player") && !isPlayerInside)
        {
            // 도착지 포탈이 유효한지 확인
            if (destinationPortal != null)
            {
                Rigidbody2D playerRb = other.GetComponent<Rigidbody2D>();
                if (playerRb != null)
                {
                    // 이동 전 플레이어의 속도를 저장하고 잠시 멈춤
                    Vector2 originalVelocity = playerRb.velocity;
                    playerRb.velocity = Vector2.zero;

                    // 플레이어를 도착지 포탈 위치로 이동
                    other.transform.position = destinationPortal.transform.position;

                    // 이동 후 다시 속도 적용 
                    playerRb.velocity = originalVelocity;

                    // 도착지 포탈에 무한 루프 방지 플래그를 설정
                    destinationPortal.isPlayerInside = true;

                    SwitchBGM();


                }
            }
        }
    }

    private void SwitchBGM()
    {

        if (currentBGM != null && currentBGM.isPlaying)
        {
            currentBGM.Stop();
        }

        if (nextBGM != null && !nextBGM.isPlaying)
        {
            nextBGM.gameObject.SetActive(true);
            nextBGM.Play();
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            isPlayerInside = false;
        }
    }


}