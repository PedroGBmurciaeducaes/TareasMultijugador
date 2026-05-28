using UnityEngine;

public class Breadcrumb : MonoBehaviour
{
    [SerializeField]
    public float lifespan = 3f;

    private void Update()
    {
        lifespan -= Time.deltaTime;

        if (lifespan <= 0f)
        {
            Destroy(gameObject);
        }
    }
}