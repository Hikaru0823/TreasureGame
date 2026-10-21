using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Stage : SceneSingleton<Stage>
{
    [Header("Resources")]
    [SerializeField] private Collider[] _floorColliders;

    public void SetIgnoreFloorCollision(Collider target)
    {
        foreach (Collider floor in _floorColliders)
        {
            Physics.IgnoreCollision(target, floor, true);
        }
    }

}
