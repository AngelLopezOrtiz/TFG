using UnityEngine;

public class TriggerDanioArana : MonoBehaviour
{
    private EnemyArana arana;

    void Awake()
    {
        arana = GetComponentInParent<EnemyArana>();
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        arana?.OnTriggerDanio(other);
    }
}