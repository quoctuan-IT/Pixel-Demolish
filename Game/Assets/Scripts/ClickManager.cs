using UnityEngine;
using UnityEngine.InputSystem;

public class ClickManager : MonoBehaviour
{
    [SerializeField] private float radius = 2f;

    void Update()
    {
        if (Mouse.current.leftButton.wasPressedThisFrame)
        {
            Ray ray = Camera.main.ScreenPointToRay(Mouse.current.position.ReadValue());

            if (Physics.Raycast(ray, out RaycastHit hit))
            {
                
                if (hit.collider.TryGetComponent<Cube>(out var centerCube))
                    DetachCircle(centerCube);
            }
        }
    }

    void DetachCircle(Cube center)
    {
        if (center == null)
            return;

        Entity entity = center.GetComponentInParent<Entity>();
        if (entity == null)
            return;

        Cube[] cubes = entity.GetComponentsInChildren<Cube>();
        Vector3 centerPos = center.transform.localPosition;

        // List temp
        System.Collections.Generic.List<Cube> toDetach = new();

        foreach (Cube cube in cubes)
        {
            if (cube == null)
                continue;

            float distance = Vector3.Distance(cube.transform.localPosition, centerPos);

            if (distance <= radius)
                toDetach.Add(cube);
        }

        // Detach
        foreach (Cube cube in toDetach)
        {
            if (cube != null)
                cube.Detach();
        }
    }
}