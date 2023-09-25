using MBT;
using NPOI.XSSF.Model;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[MBTNode("AI/Monster/Detect Enemy Service")]
public class DetectEnemyServices : Service
{
    public LayerMask mask = -1;

    [Tooltip("Sphere radius")]
    public float range = 15;
    public TransformReference variableToSet = new TransformReference(VarRefMode.DisableConstant);

    public override void Task()
    {
        var collider = Physics2D.OverlapCircle(transform.position, range, mask);
        variableToSet.Value = collider != null ? collider.transform : null;
    }
}
