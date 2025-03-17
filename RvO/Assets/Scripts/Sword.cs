using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Sword : MonoBehaviour
{
    // ����� ��� ��������, ��� �� ���� ��������� �� ����� �����
    public bool IsAttackingEnemy(EnemyController enemy)
    {
        // ���������, ��� ���� � ���� �����
        Vector3 directionToEnemy = enemy.transform.position - transform.position;
        float distanceToEnemy = directionToEnemy.magnitude;

        // ���������, ��� ���� � ������� �����
        if (distanceToEnemy < 2.0f)
        {
            return true;
        }

        return false;
    }
}
