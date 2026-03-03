using Unity.Netcode;
using Unity.VisualScripting;
using UnityEngine;
public class ProjectileLauncher : NetworkBehaviour
{

    [SerializeField]
    private InputReader inputReader;


    [SerializeField]
    private GameObject serverProjectilePrefab;
    [SerializeField]
    private GameObject clientProjectilePrefab;

    [SerializeField]
    private Transform projectileSpawnPoint;



    [Header("Settings")]
    [SerializeField]
    private float projectileSpeed;


    [SerializeField]
    private GameObject muzzleFlash;

    [SerializeField]
    private  Collider2D playerCollider;

    [SerializeField]
    private float fireRate = 1f; // Disparos por segundo
    private float previousFireTime = 0f;

    [SerializeField]
    private float muzzleFlashDuration = 0.075f;
    private float muzzleFlashTimer = 0f;




    public override void OnNetworkSpawn()
    {
        if (!IsOwner) return;
        inputReader.PrimaryFireEvent += OnFire;
    }

    public override void OnNetworkDespawn()
    {
        if (!IsOwner) return;
        inputReader.PrimaryFireEvent -= OnFire;
    }


    private bool shouldFire = false;
    private void OnFire(bool shouldFire)
    {
        if (!shouldFire) return;
        FireOnce();
    }

    private void FireOnce()
    {
        if (!IsOwner) return;

        // Control de cadencia de disparo
        if (Time.time < previousFireTime + (1f / fireRate))
        {
            return; // Aún no se puede disparar
        }

        previousFireTime = Time.time;

        // 1) Crear proyectil dummy
        SpawnDummyProjectile(projectileSpawnPoint.position, projectileSpawnPoint.up);

        // 2) Avisar al servidor
        PrimaryFireServerRpc(projectileSpawnPoint.position, projectileSpawnPoint.up);
    }


      private void Update()
       {
            if (muzzleFlashTimer > 0)
            {
                muzzleFlashTimer -= Time.deltaTime;
                if (muzzleFlashTimer <= 0)
                {
                    muzzleFlash.SetActive(false);
                }
            }

        }

    private void SpawnDummyProjectile(Vector3 spawnPos, Vector3 direction)
    {
        GameObject projectileInstance = Instantiate(clientProjectilePrefab, spawnPos,Quaternion.identity);

        projectileInstance.transform.up = direction;


        muzzleFlash.SetActive(true);
        muzzleFlashTimer = muzzleFlashDuration;

        //  Ignorar colisión con el tanque que dispara
        var projectileCollider = projectileInstance.GetComponent<CircleCollider2D>();
        if (projectileCollider != null && playerCollider != null)
        {
            Physics2D.IgnoreCollision(playerCollider, projectileCollider);
        }

        // Asignar velocidad (proyectil dummy - cliente)
        if (projectileInstance.TryGetComponent<Rigidbody2D>(out var rb))
        {
            rb.linearVelocity = rb.transform.up * projectileSpeed;
        }
    }

    [ServerRpc]
    private void PrimaryFireServerRpc(Vector3 spawnPos, Vector3 direction)
    {
        GameObject projectileInstance = Instantiate(serverProjectilePrefab, spawnPos, Quaternion.identity);

        // IMPORTANTE: Spawnear en red
        NetworkObject netObj = projectileInstance.GetComponent<NetworkObject>();
        netObj.Spawn();

        projectileInstance.transform.up = direction;

        var projectileCollider = projectileInstance.GetComponent<CircleCollider2D>();
        if (projectileCollider != null && playerCollider != null)
        {
            Physics2D.IgnoreCollision(playerCollider, projectileCollider);
        }

        if (projectileInstance.TryGetComponent<Rigidbody2D>(out var rb))
        {
            rb.linearVelocity = rb.transform.up * projectileSpeed;
        }

        if (projectileInstance.TryGetComponent<DealDamageOnContact>(out var dealDamage))
        {
            dealDamage.SetOwner(OwnerClientId);
        }

        SpawnDummyProjectileClientRpc(spawnPos, direction);
    }

    [ClientRpc]
    private void SpawnDummyProjectileClientRpc(Vector3 spawnPos, Vector3 direction)
    {
        if (IsOwner) return; // Evita crear doble proyectil en quien disparo
        SpawnDummyProjectile(spawnPos, direction);
    }






}