using UnityEngine;

public class EffectDestroy : MonoBehaviour
{
    // Inspector에서 설정할 이펙트가 화면에 표시될 시간 (애니메이션 길이에 맞춰 설정)
    public float effectDuration = 0.5f;

    void Start()
    {
        Destroy(gameObject, effectDuration);
    }
}