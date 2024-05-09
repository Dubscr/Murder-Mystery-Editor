using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spawn : BasePlaceholder
{
    public Mesh gizmoMesh;
    private void OnDrawGizmos()
    {
        Gizmos.color = new Color(255, 255, 255, 0.1f);
        Gizmos.DrawWireMesh(gizmoMesh, transform.position, Quaternion.Euler(new Vector3(-90, 0, 0)), new Vector3(50, 50, 50));
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, 0.1f);
    }
}
