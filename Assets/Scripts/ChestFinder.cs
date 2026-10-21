using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChestFinder : MonoBehaviour
{
    [Header("Resources")]
    [SerializeField] private Camera _camera;

    private Chest _currentTarget;

    void Start()
    {
        if (_camera == null)
        {
            _camera = Camera.main;
        }
    }

    void Update()
    {
        Chest nextTarget = null;
        Ray ray = _camera.ScreenPointToRay(Input.mousePosition);

        Debug.DrawRay(ray.origin, ray.direction * 100f, Color.red);

        if (Physics.Raycast(ray, out RaycastHit hit))
        {
            GameObject obj = hit.collider.gameObject;

            if (obj.CompareTag("Chest"))
            {
                nextTarget = hit.collider.GetComponentInParent<Chest>();
            }
        }

        if (nextTarget == _currentTarget)
            return;

        if (_currentTarget != null)
            _currentTarget.OnDeselected();
        _currentTarget = nextTarget;
        if (_currentTarget != null)
            _currentTarget.OnSelected();
    }
}
