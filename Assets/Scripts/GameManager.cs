using UnityEngine;
using TMPro; 

public class GameManager : MonoBehaviour
{
    // 1. 정적(static) 변수 선언: 씬 전체에서 이 인스턴스에 접근할 수 있게 합니다.
    public static GameManager Instance;

    public TextMeshProUGUI gemCountText;
    public TextMeshProUGUI gemNameText;
    private int currentGemCount = 0;

    //  2. Awake 함수에서 인스턴스 초기화 (싱글톤 패턴)
    void Awake()
    {
        // 씬에 GameManager가 이미 존재하는지 확인합니다.
        if (Instance == null)
        {
            // 아직 없으면, 자기 자신(this)을 Instance로 설정합니다.
            Instance = this;
        }
        else
        {
            
            Destroy(gameObject);
        }
    }

    void Start()
    {
        UpdateGemUI();
    }

    // Gem 개수를 증가시키는 함수 (Gem.cs에서 호출됨)
    public void AddGem(int amount)
    {
        currentGemCount += amount;
        UpdateGemUI();
    }

    // UI 텍스트를 업데이트하는 함수
    private void UpdateGemUI()
    {
        if (gemCountText != null)
        {
            gemCountText.text = "Coin " + currentGemCount.ToString();
        }
    }

    // 3. Gem 획득 사운드 재생 함수 추가
    public AudioSource soundPlayer; 
    public AudioClip gemPickupClip; 

    public void PlayGemPickupSound()
    {
        if (soundPlayer != null && gemPickupClip != null)
        {
            soundPlayer.PlayOneShot(gemPickupClip);
        }
    }
}