using UnityEngine;

public class BloodPuddle : MonoBehaviour
{
    [SerializeField]
    public float lifespan = 4f;

    private void Update()
    {
        lifespan -= Time.deltaTime;

        if (lifespan <= 0f)
        {
            Destroy(gameObject);
        }
    }
}