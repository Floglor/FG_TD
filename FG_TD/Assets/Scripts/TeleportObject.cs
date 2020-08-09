using System;
using System.Collections;
using System.Collections.Generic;
using Shooting;
using UnityEngine;

public class TeleportObject : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other is CircleCollider2D) return;

        if (!other.CompareTag(Enemy.MyTag)) return;
        
        GameObject enemyObject = other.gameObject;
        Vector2 transformPosition = enemyObject.transform.position;
        transformPosition.x = 21.57f;
        transformPosition.y = 9.9f;

        enemyObject.transform.position = transformPosition;

        Enemy enemy = enemyObject.GetComponent<Enemy>();
        enemy.target = Waypoints.points[1];
        enemy.waypointIndex = 1;
    }
}
