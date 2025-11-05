using UnityEngine;
using System.Collections; // Coroutine 사용을 위해 추가

public class FloatingAnimation : MonoBehaviour
{
    public float floatStrength = 0.5f;
    public float floatSpeed = 1f;   
    private Vector3 initialPosition; 

    void Start()
    {
        initialPosition = transform.localPosition; // 로컬 위치를 저장 (부모에 상대적)
        // 코루틴 시작: 오브젝트의 Y축 위치를 계속 변경합니다.
        StartCoroutine(FloatCoroutine());
    }

    // 둥둥 떠다니는 애니메이션을 처리하는 코루틴
    IEnumerator FloatCoroutine()
    {
        while (true) // 무한 반복
        {
            // Mathf.Sin 함수를 사용하여 부드러운 파동형 움직임을 만듭니다.
            // Time.time과 floatSpeed로 시간의 흐름을 조절하여 위아래로 이동
            transform.localPosition = new Vector3
                (
                initialPosition.x,
                initialPosition.y + (Mathf.Sin(Time.time * floatSpeed) * floatStrength),
                initialPosition.z
                );

            yield return null; // 다음 프레임까지 대기
        }
    }

    // 오브젝트가 비활성화될 때 코루틴을 멈춰서 오류를 방지
    void OnDisable()
    {
        StopAllCoroutines();
    }

    // 오브젝트가 다시 활성화될 때 코루틴을 다시 시작
    void OnEnable()
    {
        // 오브젝트가 활성화된 위치를 다시 초기 위치로 설정
        initialPosition = transform.localPosition;
        StartCoroutine(FloatCoroutine());
    }
}