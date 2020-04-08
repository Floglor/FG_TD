using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class waypointsscript : MonoBehaviour
{
    public float range;
    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.magenta;
        Gizmos.DrawWireSphere(transform.position, range);
    }
}
