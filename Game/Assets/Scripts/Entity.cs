using System.Collections.Generic;
using UnityEngine;


[RequireComponent(typeof(Rigidbody))]
public class Entity : MonoBehaviour
{
    private Rigidbody rb;
    // List Cube
    private Cube[] cubes;
    // ID Cube for 2D Map
    private int[,] grid;
    // Grid
    private Vector3 gridOrigin;


    void Awake()
    {
        SetupRigidbody();
        CollectCubes();
        Recalculate();
    }

    void SetupRigidbody()
    {
        rb = GetComponent<Rigidbody>();
        rb.constraints = RigidbodyConstraints.FreezePositionZ |
                         RigidbodyConstraints.FreezeRotationX |
                         RigidbodyConstraints.FreezeRotationY;

        rb.mass = transform.childCount;
    }

    // Read all Cube to Create Grid
    void CollectCubes()
    {
        cubes = GetComponentsInChildren<Cube>();

        Vector3 min = Vector3.one * float.MaxValue;
        Vector3 max = Vector3.one * float.MinValue;

        foreach (Transform child in transform)
        {
            min = Vector3.Min(min, child.localPosition);
            max = Vector3.Max(max, child.localPosition);
        }

        // Create grid 2D
        Vector2Int size = Vector2Int.RoundToInt(max - min);
        grid = new int[size.x + 1, size.y + 1];
        gridOrigin = min;

        for (int i = 0; i < cubes.Length; i++)
        {
            Vector2Int pos = ToGrid(cubes[i].transform.localPosition);
            int id = i + 1;

            cubes[i].Id = id;
            grid[pos.x, pos.y] = id;
        }
    }

    // Split Groups
    void Recalculate()
    {
        // // Get remain Cube
        List<int> remaining = GetRemainingCubeIds();

        if (remaining.Count == 0)
        {
            Destroy(gameObject);
            return;
        }

        List<List<int>> groups = SplitIntoGroups(remaining);

        if (groups.Count <= 1)
            return;

        CreateNewEntities(groups);

        // Rebuild Entity
        CollectCubes();
    }


    // Get ID remain Cube
    List<int> GetRemainingCubeIds()
    {
        List<int> ids = new();

        foreach (var cube in cubes)
        {
            if (cube != null)
                ids.Add(cube.Id);
        }

        return ids;
    }

    // Find Cube near by
    List<List<int>> SplitIntoGroups(List<int> ids)
    {
        List<List<int>> groups = new();

        while (ids.Count > 0)
        {
            List<int> group = new();
            FloodFill(ids[0], ids, group);
            groups.Add(group);
        }

        return groups;
    }

    // Algorithm DFS
    void FloodFill(int startId, List<int> remaining, List<int> group)
    {
        remaining.Remove(startId);
        group.Add(startId);

        Vector2Int pos = ToGrid(cubes[startId - 1].transform.localPosition);

        Vector2Int[] directions = {
            Vector2Int.up, Vector2Int.right,
            Vector2Int.down, Vector2Int.left
        };

        foreach (var dir in directions)
        {
            int neighborId = GetNeighbor(pos, dir);

            if (remaining.Contains(neighborId))
                FloodFill(neighborId, remaining, group);
        }
    }

    // Create new Entity for new Group
    void CreateNewEntities(List<List<int>> groups)
    {
        // Create Group[>0]
        for (int i = 1; i < groups.Count; i++)
        {
            GameObject newEntity = new("Entity");

            Transform firstCube = cubes[groups[i][0] - 1].transform;
            newEntity.transform.SetPositionAndRotation(firstCube.position, firstCube.rotation);

            foreach (int id in groups[i])
                cubes[id - 1].transform.parent = newEntity.transform;

            newEntity.AddComponent<Entity>();
        }
    }

    // Detach Cube
    public void DetachCube(Cube cube)
    {
        Vector2Int pos = ToGrid(cube.transform.localPosition);

        // Remove from Grid
        grid[pos.x, pos.y] = 0;
        // Remove from List
        cubes[cube.Id - 1] = null;
        // Remove from Entity
        cube.transform.parent = null;

        Rigidbody cubeRb = cube.gameObject.AddComponent<Rigidbody>();
        cubeRb.constraints = RigidbodyConstraints.FreezePositionZ;

        Recalculate();
    }

    // Convert from Position to Grid
    Vector2Int ToGrid(Vector3 localPos)
    {
        return Vector2Int.RoundToInt(localPos - gridOrigin);
    }

    // Get Cube near by
    int GetNeighbor(Vector2Int pos, Vector2Int dir)
    {
        Vector2Int p = pos + dir;

        if (p.x < 0 || p.y < 0 ||
            p.x >= grid.GetLength(0) ||
            p.y >= grid.GetLength(1))
            return 0;

        return grid[p.x, p.y];
    }
}