using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Sword : MonoBehaviour
{
    // Метод для проверки, был ли удар направлен на этого врага
    public bool IsAttackingEnemy(EnemyController enemy)
    {
        // Проверяем, что враг в зоне атаки
        Vector3 directionToEnemy = enemy.transform.position - transform.position;
        float distanceToEnemy = directionToEnemy.magnitude;

        // Убедитесь, что враг в радиусе атаки
        if (distanceToEnemy < 2.0f)
        {
            return true;
        }

        return false;
    }
}
