using UnityEngine;
using UnityEngine.AI;

public class BreadcrumbGenerator : MonoBehaviour
{
    [Header("Breadcrumb")]
    [SerializeField] private GameObject breadcrumbPrefab;

    [SerializeField]
    private float breadcrumbInterval = 0.2f;

    [SerializeField]
    private bool showBreadcrumbs = true;

    private NavMeshAgent agent;

    private Transform tempContainer;

    private void Awake()
    {
        agent = GetComponent<NavMeshAgent>();

        GameObject container =
            GameObject.Find("BreadcrumbContainer");

        if (container == null)
        {
            container =
                new GameObject("BreadcrumbContainer");
        }

        tempContainer = container.transform;

#if !UNITY_EDITOR
        showBreadcrumbs = false;
#endif
    }

    private void Start()
    {
        InvokeRepeating(
            nameof(CreateBreadcrumb),
            0f,
            breadcrumbInterval);
    }

    private void CreateBreadcrumb()
    {
        if (agent == null)
            return;

        if (agent.velocity.magnitude <= 0.1f)
            return;

        GameObject crumb =
            Instantiate(
                breadcrumbPrefab,
                transform.position,
                Quaternion.identity);

        crumb.transform.SetParent(tempContainer);

        if (!showBreadcrumbs)
        {
            Transform gfx =
                crumb.transform.Find("GFX");

            if (gfx != null)
            {
                gfx.gameObject.SetActive(false);
            }
        }
    }
}