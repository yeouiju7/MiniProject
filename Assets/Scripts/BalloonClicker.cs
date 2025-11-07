using UnityEngine;
using System;

public class BalloonClicker : MonoBehaviour
{
    //DialogueSystemManager 오브젝트를 연결합니다.
    public DialogueManager dialogueManager;

    [TextArea(3, 10)]
    public string[] dialogueLines; //Inspector에서 작성할 대화 내용

    [Header("패널 및 타입 설정")]
    //DialogueManager의 List<GameObject> dialoguePanels에서 사용할 인덱스
    public int panelID = 0;

    public DialogueManager.DialogueType dialogueType = DialogueManager.DialogueType.NORMAL;

    public void OnClickStartDialogue()
    {
        if (dialogueManager != null)
        {
            //1. 말풍선을 비활성화합니다.
            gameObject.SetActive(false);

            //2. 대화 매니저에게 모든 정보를 전달하며 시작을 요청
            dialogueManager.StartDialogue(
                dialogueLines,
                panelID,
                OnDialogueFinished, //대화 종료 시 실행될 함수 (말풍선 복구)
                dialogueType
            );
        }
    }

    // 대화가 끝났을 때 DialogueManager에 의해 호출되어 말풍선을 다시 활성화
    private void OnDialogueFinished()
    {
        gameObject.SetActive(true);
    }
}