using UnityEngine;

public class Coin : MonoBehaviour
{
    public int gemValue = 10;

    private void OnTriggerEnter2D(Collider2D other) // 2D 프로젝트 가정
    {
        if (other.CompareTag("Player"))
        {
            Player player = other.GetComponent<Player>();
            if (player != null)
            {
                //Player의 AddCoins 함수를 호출하여 코인을 직접 증가시킵니다.
                player.AddCoins(gemValue);

                //GameManager를 통해 사운드 재생 코드는 유지 가능
                if (GameManager.Instance != null)
                {
                    //GameManager가 코인 사운드를 담당한다고 가정
                    GameManager.Instance.PlayGemPickupSound();
                }

                
                Destroy(gameObject);
            }
        }
    }
}