using UnityEngine;

public class SpawnGameObjectOnDestroy : MonoBehaviour
{

    [SerializeField] 
    private GameObject particlePrefab;

    private void OnDestroy()
    {
        Instantiate(particlePrefab, transform.position, Quaternion.identity);
    }
}

    

