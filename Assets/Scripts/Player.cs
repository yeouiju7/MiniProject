using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class Player : MonoBehaviour
{

    [Header("이동 설정")]
    public float moveSpeed = 5f;
    public float jumpForce = 10f;

    [Header("바닥 체크 설정")]
    public Transform groundCheck;      
    public float checkRadius = 0.2f;
    public LayerMask whatIsGround;      //바닥으로 인식할 레이어 마스크

    [Header("사다리 설정")]
    public float climbSpeed = 3f;
    private bool isClimbing;      //현재 사다리를 잡고 있는지
    private bool canClimb;        //사다리 콜라이더 안에 있는지

    [Header("Audio 설정")]
    public AudioClip jumpSoundClip;

    [Header("UI Reference")]
    public TextMeshProUGUI coinDisplay;

    private Rigidbody2D rb;
    private float moveDirection;
    private bool isGrounded;
    private SpriteRenderer spriteRenderer;
    private Animator anim;
    private AudioSource audioSource;
    private Collider2D playerCollider;

    private bool hasDoubleJump = false; //DialogueManager에서 설정됨
    private int jumpCount = 0;          //현재 점프 횟수 추적
    public int maxJumps = 1;            //최대 점프 횟수 (싱글 + 더블 = 2)

    private int currentCoins = 0;

    [Header("Health & Respawn")]
    public int maxHealth = 5;       //최대 라이프 
    private int currentHealth;      //현재 라이프
    public Vector3 respawnPoint;

    [Header("Health UI")]
    public Image[] hearts;

    private void UpdateHealthUI()
    {
        //hearts 배열에 하트 이미지가 연결되어 있는지 확인
        if (hearts == null || hearts.Length == 0)
        {
            Debug.LogWarning("Health UI hearts array is not set up in the Inspector!");
            return;
        }

        //currentHealth 값에 따라 하트 이미지 활성화/비활성화
        for (int i = 0; i < hearts.Length; i++)
        {
   
            if (i < currentHealth)
            {
                // 라이프가 남아있음 -> 풀 하트 이미지로 설정
                hearts[i].enabled = true; // Image 컴포넌트를 활성화
            }
            else
            {
                // 라이프가 부족->빈 하트 이미지로 설정
                hearts[i].enabled = false; // Image 컴포넌트를 비활성화
            }
        }
    }
    private void UpdateCoinUI()
    {
        if (coinDisplay != null)
        {
            coinDisplay.text = "Coin " + currentCoins.ToString();
        }
    }
    public int GetCurrentCoins()
    {
        return currentCoins;
    }
    public void SetCurrentCoins(int newCoinAmount)
    {
        currentCoins = newCoinAmount;
        UpdateCoinUI();

    }
    public void AddCoins(int amount)
    {
        currentCoins += amount;
        UpdateCoinUI();
    }



    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        anim = GetComponent<Animator>();
        audioSource = GetComponent<AudioSource>();
        StartCoroutine(SafeSpawnRoutine());

        currentHealth = maxHealth;
        respawnPoint = transform.position;

    }



    public void DieByFall()
    {
      
        currentHealth -= 1;

        UpdateHealthUI(); 

        if (currentHealth > 0)
        {
            
            transform.position = respawnPoint;
           
        }
        else
        {
            Time.timeScale = 0f;
            SceneManager.LoadScene("Over");
        }
    }

    IEnumerator SafeSpawnRoutine()
    {
        //1. 충돌 비활성화
        //whatIsGround 마스크에 포함된 모든 레이어와의 충돌을 무시합니다.
        int groundLayerNumber = (int)Mathf.Log(whatIsGround.value, 2);

        //플레이어 레이어와 지면 레이어의 충돌 무시 (true)
        Physics2D.IgnoreLayerCollision(gameObject.layer, groundLayerNumber, true);

        //Rigidbody를 키네마틱으로 만들어 물리적인 힘이 작용하지 않게 합니다.
        if (rb != null) rb.isKinematic = true;

        //2. 두 프레임 대기 (물리 시스템이 문제를 '잊어버릴' 시간)
        yield return null;
        yield return null;

        //3. 충돌 및 물리 재활성화
        //충돌을 다시 감지하도록 설정 (false)
        Physics2D.IgnoreLayerCollision(gameObject.layer, groundLayerNumber, false);

        //Rigidbody를 Dynamic으로 복구하고 중력 재적용
        if (rb != null)
        {
            rb.isKinematic = false;
            rb.gravityScale = 1f; //원래 중력 값
        }
    }

    void Update()
    {

        moveDirection = Input.GetAxisRaw("Horizontal");

        if (Input.GetButtonDown("Jump"))
        {
            HandleJump();
        }
        //사다리 오르기 시작/멈춤 처리
        //수직 입력
        float verticalInput = Input.GetAxisRaw("Vertical");

        if (canClimb && Mathf.Abs(verticalInput) > 0.01f && !isClimbing)
        {
            
            StartClimbing(verticalInput);
        }

        //사다리를 타는 중이라면 점프 키를 누르거나 사다리에서 벗어날 때 멈춤
        if (isClimbing)
        {
          
            HandleClimbing(verticalInput);

          
            if (!canClimb && Mathf.Abs(verticalInput) < 0.01f)
            {
                StopClimbing();
            }

            //꼭대기에서 수직 입력이 멈췄을 때 즉시 중력S 복구* 사다리 꼭대기/바닥에 도착해서 수직 입력이 없을 경우 멈춤
            if (canClimb && Mathf.Abs(verticalInput) < 0.01f)
            {
                rb.velocity = new Vector2(rb.velocity.x, 0);
                anim.SetFloat("ClimbSpeed", 0f);
            }
        }

    }

    void FixedUpdate()
    {
        //바닥 체크
        isGrounded = Physics2D.OverlapCircle(groundCheck.position, checkRadius, whatIsGround);

        if (!isClimbing) //사다리를 타고 있지 않을 때만 일반 이동 로직 실행
        {
            //좌우 이동(기존 코드)
            rb.velocity = new Vector2(moveDirection * moveSpeed, rb.velocity.y);

            //중력 원복
            if (rb.gravityScale != 1f)
            {
                rb.gravityScale = 1f;
            }

            //애니메이터 설정(Idle <-> Run 전환)
            anim.SetFloat("Speed", Mathf.Abs(moveDirection));
        }

        //애니메이터 설정(점프/땅 상태 전환)
        anim.SetBool("IsGrounded", isGrounded);


        //캐릭터 좌우 반전
        if (moveDirection > 0)
        {
            spriteRenderer.flipX = false;
        }
        else if (moveDirection < 0)
        {
            spriteRenderer.flipX = true;
        }

    }
    public void EnableDoubleJump()
    {
        hasDoubleJump = true;
        maxJumps = 2; 
    }

    private void HandleJump()
    {
  
        if (isGrounded)
        {
            rb.velocity = new Vector2(rb.velocity.x, jumpForce);

            PlayJumpSound();
        }

        if (jumpCount < maxJumps)
        {
            //점프 실행 코드
            Rigidbody2D rb = GetComponent<Rigidbody2D>();

            //기존의 수직 속도를 0으로 초기화하여 점프 높이를 만듬
            rb.velocity = new Vector2(rb.velocity.x, 0f);

            //점프 힘을 가하여 위로 점프
            rb.AddForce(new Vector2(0f, jumpForce), ForceMode2D.Impulse);

            //점프 횟수를 증가
            jumpCount++;
        }
    }
    private void OnCollisionEnter2D(Collision2D collision)
    {
        //충돌한 오브젝트가 'Ground' 태그를 가지고 있다면
        if (collision.gameObject.CompareTag("Ground"))
        {
            //땅에 닿았으므로 점프 횟수 초기화
            jumpCount = 0;
        }


    }

    private void PlayJumpSound()
    {
        if (audioSource != null && jumpSoundClip != null)
        {
            //PlayOneShot을 사용하여 다른 소리를 방해하지 않고 재생합니다.
            audioSource.PlayOneShot(jumpSoundClip);
        }
       
    }
    private void HandleClimbing(float verticalInput)
    {
        rb.velocity = new Vector2(0, verticalInput * climbSpeed);
        anim.SetFloat("ClimbSpeed", Mathf.Abs(verticalInput)); 
    }

    private void StartClimbing(float verticalInput)
    {
        isClimbing = true;
        rb.gravityScale = 0f;

        // 사다리 타기 시작 시 수평 움직임 초기화
        rb.velocity = new Vector2(0, 0);

        anim.SetBool("IsClimbing", isClimbing);

        //바닥 충돌 무시 시작
        //캐릭터 레이어와 바닥 레이어 간의 충돌을 무시
        Physics2D.IgnoreLayerCollision(gameObject.layer, whatIsGround, true);
        //whatIsGround가 LayerMask 타입-> 비트 마스킹으로 레이어 번호를 추출해야 함

        //whatIsGround 마스크에 포함된 모든 레이어를 찾아서 무시
        //플레이어의 Collider를 비활성화하는 방법
        // LayerMask에서 첫 번째 레이어를 추출하여 사용 
        int groundLayerNumber = Mathf.FloorToInt(Mathf.Log(whatIsGround.value, 2));
        if (groundLayerNumber >= 0)
        {
            Physics2D.IgnoreLayerCollision(gameObject.layer, groundLayerNumber, true);
        }
    }


    private void StopClimbing()
    {
        isClimbing = false;
        rb.gravityScale = 1f;
        anim.SetBool("IsClimbing", false);
        anim.SetFloat("ClimbSpeed", 0f);

        //핵심 수정: 바닥 충돌 무시 해제
        //충돌 무시를 해제하고 다시 충돌을 감지하도록 합니다.
        int groundLayerNumber = Mathf.FloorToInt(Mathf.Log(whatIsGround.value, 2));
        if (groundLayerNumber >= 0)
        {
            Physics2D.IgnoreLayerCollision(gameObject.layer, groundLayerNumber, false);
        }

    }
   
    // 콜라이더 트리거 안에 들어왔을 때
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Ladder"))
        {
            canClimb = true;
        }

        if (other.CompareTag("KillZone"))
        {
            DieByFall(); 
        }

    }

    // 콜라이더 트리거에서 나갔을 때
    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Ladder"))
        {
            canClimb = false;
           
            if (isClimbing)
            {
                StopClimbing();
            }
        }
    }


}