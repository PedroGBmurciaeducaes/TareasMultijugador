using System.Collections.Generic;
using UnityEngine;

public class AttackPriorityManager : MonoBehaviour
{
    public static AttackPriorityManager Instance;

    [SerializeField]
    private int maxAttackers = 2;

    private readonly List<EnemyAI> attackers = new List<EnemyAI>();



    private void Awake()
    {
        Instance = this;
    }


    public void SetMaxAttackers(int amount)
    {
        maxAttackers = amount; 
    }

    public bool RequestAttack(EnemyAI enemy)
    {
        attackers.RemoveAll(x => x == null);

        if (attackers.Contains(enemy))
            return true;

        if (attackers.Count >= maxAttackers)
            return false;

        attackers.Add(enemy);

        return true;
    }

    public void ReleaseAttack(EnemyAI enemy)
    {
        if (attackers.Contains(enemy))
        {
            attackers.Remove(enemy);
        }
    }

    public bool HasFreeAttackSlot()
    {
        return attackers.Count < maxAttackers;
    }

    public bool AreAllSlotsOccupied()
    {
        attackers.RemoveAll(x => x == null);

        return attackers.Count >= maxAttackers;
    }

    public void CurrentMaxAttackers(Health playerHealth)
    {
        float healthPercent = playerHealth.health / playerHealth.healthMax;

        // Mucha vida -> más presión
        if (healthPercent > 0.7f)
        {
            maxAttackers = 4;
        }

        if (healthPercent > 0.35f && healthPercent <= 0.7f)
        {
            maxAttackers = 3;
        }

        // Vida media
        if (healthPercent < 0.35f)
        {
            int attackers =2 ;

            maxAttackers = attackers;
        }
    }
}