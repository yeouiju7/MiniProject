using UnityEngine;
using UnityEngine.UI;

public class HealthUI : MonoBehaviour
{
    
    public Image[] heartImages;

    public Sprite fullHeartSprite; 
    public Sprite halfHeartSprite;  
    public Sprite emptyHeartSprite; 

    // 이 함수는 플레이어가 데미지를 입거나 회복할 때마다 PlayerHealth 스크립트에서 호출해야 합니다.
    public void UpdateHealth(int currentHealth)
    {

       

        // 500 체력 (5 하트)을 50 체력 단위로 확인합니다.
        int heartCount = currentHealth / 50;

        // 하트 5개 (배열 크기 5)를 순회하며 이미지 업데이트
        for (int i = 0; i < heartImages.Length; i++)
        {
            // 각 하트 인덱스가 나타내는 최대 체력 (100, 200, 300...)
            int maxHealthForHeart = (i + 1) * 100;

            if (currentHealth >= maxHealthForHeart)
            {
                // 현재 체력이 이 하트가 나타내는 체력 이상이면, 꽉 찬 하트
                heartImages[i].sprite = fullHeartSprite;
            }
            else if (currentHealth >= maxHealthForHeart - 50)
            {
                // 현재 체력이 이 하트가 나타내는 체력 - 50 이상이면, 반쪽 하트 (예: 150/200 에서 150)
                heartImages[i].sprite = halfHeartSprite;
            }
            else
            {
                // 그 외는 빈 하트
                heartImages[i].sprite = emptyHeartSprite;
            }
        }
    }
}