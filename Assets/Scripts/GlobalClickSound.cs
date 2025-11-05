using UnityEngine;

public class GlobalClickSound : MonoBehaviour
{
    public AudioClip clickSound;
    private AudioSource audioSource;

    void Start()
    {
      
        audioSource = GetComponent<AudioSource>();

        if (audioSource == null)
        {
           
        }
    }

    void Update()
    {
       
        if (Input.GetMouseButtonDown(0))
        {
            PlayClickSound();
        }
    }

    void PlayClickSound()
    {
        if (audioSource != null && clickSound != null)
        {
            // AudioSource에 클릭 효과음을 할당하고 재생
            // PlayOneShot을 사용하면 현재 재생 중인 소리와 별개로 소리를 겹쳐서 재생가능
            audioSource.PlayOneShot(clickSound);
        }
        else
        {
         
        }
    }
}