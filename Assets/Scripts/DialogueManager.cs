using UnityEngine;
using TMPro;
using System.Collections.Generic;
using System;

public class DialogueManager : MonoBehaviour
{
   
    public enum DialogueType { NORMAL, SHOP_DOUBLE_JUMP }

   
    [Header("UI 연결")]
    public List<GameObject> dialoguePanels; // 다중 대화 패널
    public TextMeshProUGUI dialogueText;
    public GameObject nextButton;

   [Header("구매 UI")]
    public GameObject buyPromptPanel;  

    [Header("클릭 설정")]
    public LayerMask balloonLayer;

    [Header("게임 UI 연결")]
    public TextMeshProUGUI coinDisplay;

    private DialogueType currentDialogueType;
    private GameObject currentActivePanel;
    private const int DOUBLE_JUMP_COST = 100; 

    private string[] currentDialogue;
    private int currentLineIndex;
    private Action onDialogueEndCallback; //대화 종료 후 실행할 콜백 함수 (말풍선 복구)

    public int playerCurrentCoins = 500;
    public Player player;


    void Start()
    {
        //모든 대화 패널과 구매 프롬프트를 시작 시 비활성화합니다.
        foreach (GameObject panel in dialoguePanels)
        {
            if (panel != null) panel.SetActive(false);
        }
        if (buyPromptPanel != null) buyPromptPanel.SetActive(false);
    }

    void Update()
    {
        //Accept 버튼 패널이 켜져 있으면 일반 클릭(Next)을 무시합니다.
        if (buyPromptPanel != null && buyPromptPanel.activeSelf) return;

        if (Input.GetMouseButtonDown(0))
        {
            if (currentActivePanel != null && currentActivePanel.activeSelf)
            {
                DisplayNextLine();
            }
            else
            {
                CheckForBalloonClick();
            }
        }
    }

    private void UpdateCoinUI()
    {
        if (coinDisplay != null)
        { 
            coinDisplay.text = "Coin " + playerCurrentCoins.ToString();
        }
    }


    public void StartDialogue(string[] lines, int panelIndex, Action onEnd, DialogueType type)
    {
       

        if (panelIndex < 0 || panelIndex >= dialoguePanels.Count || dialoguePanels[panelIndex] == null) return;

        currentDialogueType = type;
        onDialogueEndCallback = onEnd;
        currentActivePanel = dialoguePanels[panelIndex];

        currentActivePanel.SetActive(true);

        currentDialogue = lines;
        currentLineIndex = 0;

        if (currentDialogueType == DialogueType.SHOP_DOUBLE_JUMP)
        {
            //곰 상점: 첫 줄만 표시하고 바로 Accept 
            ShowAcceptPrompt(lines[0]);
        }
        else
        {
            //토끼 일반 대화
            DisplayLine(currentLineIndex);
        }

        Time.timeScale = 0f;
    }

   
    public void DisplayNextLine()
    {
        //곰 : 상점 대화 중에는 일반 클릭(패널 밖 포함)은 No로 간주
        if (currentDialogueType == DialogueType.SHOP_DOUBLE_JUMP)
        {
            //곰 대화 시작 시, 아직 Accept가 눌리지 않았다면: EndDialogue() (거절)
            //Accept가 눌린 후 최종 메시지가 화면에 남아있는 상태라면: EndDialogue() (확정 종료)
            EndDialogue();
            return;
        }

        // 토끼 일반 대화 로직
        currentLineIndex++;

        if (currentLineIndex < currentDialogue.Length)
        {
            DisplayLine(currentLineIndex);
        }
        else
        {
            EndDialogue();
        }
    }

    private void DisplayLine(int index)
    {
        dialogueText.text = currentDialogue[index];
      
        if (nextButton != null)
        {
            nextButton.SetActive(index < currentDialogue.Length - 1);
        }
    }

    //상점 구매 로직
    private void ShowAcceptPrompt(string message)
    {
    
        dialogueText.text = message; //곰 대사 표시

        if (nextButton != null) nextButton.SetActive(false); //Next 버튼 비활성화

        
        if (buyPromptPanel != null) buyPromptPanel.SetActive(true); //Accept 버튼 활성화
    }

    //Accept 버튼 클릭 이벤트에 연결될 함수
    public void OnAcceptClicked()
    {
        const int NEW_DOUBLE_JUMP_COST = 100;

        //1. 플레이어 코드가 유효한지 확인
        if (player == null)
        {
            Debug.LogError("PlayerController is not linked to DialogueManager.");
            EndDialogue();
            return;
        }

        //2. PlayerController에서 현재 코인 값 가져옴
        int currentCoins = player.GetCurrentCoins();
        int newCoins = currentCoins; // 새로운 코인 값을 저장할 변수

        //3. 코인이 충분한지 확인합니다. (playerCurrentCoins 변수 사용 제거)
        if (currentCoins >= NEW_DOUBLE_JUMP_COST)
        {
            //3-1. 코인 차감 계산 및 PlayerController에 적용
            newCoins = currentCoins - NEW_DOUBLE_JUMP_COST;
            player.SetCurrentCoins(newCoins); //PlayerController의 코인 값을 변경

            //3-2. 더블 점프 활성화 (코인 지불 성공 시에만)
            player.EnableDoubleJump();

        }
        else 
        {
           
        }

       
        if (buyPromptPanel != null) buyPromptPanel.SetActive(false);
        EndDialogue();
    }

    
    public void EndDialogue()
    {
        if (currentActivePanel != null)
        {
            currentActivePanel.SetActive(false);
        }
        if (buyPromptPanel != null)
        {
            buyPromptPanel.SetActive(false);
        }

        // 대화 종료 후 콜백 실행
        if (onDialogueEndCallback != null)
        {
            onDialogueEndCallback.Invoke();
            onDialogueEndCallback = null;
        }

        currentActivePanel = null;
        Time.timeScale = 1f; // 게임 시간 재개
    }

    //클릭 감지 로직 (Raycast)
    void CheckForBalloonClick()
    {
        Vector2 clickPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        RaycastHit2D hit = Physics2D.Raycast(clickPosition, Vector2.zero, 0f, balloonLayer);

        if (hit.collider != null)
        {
            BalloonClicker clicker = hit.collider.GetComponent<BalloonClicker>();
            if (clicker != null)
            {
                clicker.OnClickStartDialogue();
            }
        }
    }
}