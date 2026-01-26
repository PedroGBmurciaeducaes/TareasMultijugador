using UnityEngine;
public class Lifetime : MonoBehaviour
{
    [SerializeField]
    private float lifetime = 1f; // 1 segundo por defecto
    private void Start()
    {
    Destroy(gameObject, lifetime);
    }
}
