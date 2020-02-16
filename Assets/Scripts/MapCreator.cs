using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using Random = UnityEngine.Random;

public class MapCreator : MonoBehaviour
{
    [Range(5, 25)] 
    public int MapWidth = 15;
    [Range(5, 25)] 
    public int MapHeight = 13;
    public float MetersPerSquare = 3;
    public Material FloorMaterial;
    public GameObject PermanentWall;
    public GameObject DestructibleWall;
    public Vector3 DestructibleWallScale = new Vector3(1, 1, 1);

    [Tooltip("The percentage chance to not spawn a destructible wall at a given cell.")]
    public float GapChance = 10;

    private int FullMapWidth => MapWidth + 2;
    private int FullMapHeight => MapHeight + 2;
    private Transform _mapRoot;
    private NavMeshSurface _navMeshSurface;

    public void Start()
    {
        CreateMap();
    }

    public void Update()
    {
    }

    public void CreateMap()
    {
        if (_mapRoot != null)
        {
            Destroy(_mapRoot);
        }

        _mapRoot = new GameObject("Map").transform;
        CreateFloor();
        CreatePermanentWalls();
        CreateDestructibleWalls();
        RebuildNavMesh();
    }

    public List<Vector3> GetSpawnPoints()
    {
        return new List<Vector3>
        {
            GetSpawnPoint(1, 1),
            GetSpawnPoint(1, MapHeight),
            GetSpawnPoint(MapWidth, 1),
            GetSpawnPoint(MapWidth, MapHeight),
        };
    }

    public void RebuildNavMesh()
    {
        _navMeshSurface.BuildNavMesh();
    }

    private Vector3 GetSpawnPoint(int x, int z)
    {
        var worldPosition = GetPositionInGrid(x, z);
        return new Vector3(worldPosition.x, MetersPerSquare / 2, worldPosition.y);
    }

    private void CreateFloor()
    {
        var halfWidth = FullMapWidth * MetersPerSquare / 2;
        var halfHeight = FullMapHeight * MetersPerSquare / 2;
        var mesh = new Mesh
        {
            name = "Floor",
            vertices = new[]
            {
                new Vector3(-halfWidth, 0, -halfHeight),
                new Vector3(-halfWidth, 0, halfHeight),
                new Vector3(halfWidth, 0, halfHeight),
                new Vector3(halfWidth, 0, -halfHeight)
            },
            uv = new[]
            {
                new Vector2(0, 0),
                new Vector2(0, 1),
                new Vector2(1, 1),
                new Vector2(1, 0)
            },
            normals = new[] {Vector3.up, Vector3.up, Vector3.up, Vector3.up},
            triangles = new[] {0, 1, 2, 0, 2, 3}
        };
        mesh.RecalculateNormals();
        mesh.RecalculateBounds();
        var floor = new GameObject("Floor");
        floor.transform.parent = _mapRoot;
        var collider = floor.AddComponent<MeshCollider>();
        collider.sharedMesh = mesh;
        var renderer = floor.AddComponent<MeshRenderer>();
        renderer.material = FloorMaterial;
        renderer.material.mainTextureScale = new Vector2(FullMapWidth, FullMapHeight);
        var filter = floor.AddComponent<MeshFilter>();
        filter.mesh = mesh;
        _navMeshSurface = floor.AddComponent<NavMeshSurface>();
    }

    private void CreatePermanentWalls()
    {
        var permanentWalls = new GameObject("Permanent Walls");
        permanentWalls.transform.parent = _mapRoot;

        for (var x = 0; x < FullMapWidth; x++)
        {
            for (var z = 0; z < FullMapHeight; z++)
            {
                if (x == 0 ||
                    x == FullMapWidth - 1 ||
                    z == 0 ||
                    z == FullMapHeight - 1 ||
                    x % 2 == 0 &&
                    z % 2 == 0)
                {
                    CreatePermanentWall(x, z, permanentWalls);
                }
            }
        }
    }

    private void CreatePermanentWall(int x, int z, GameObject parent)
    {
        var worldPosition = GetPositionInGrid(x, z);
        var position = new Vector3(worldPosition.x, MetersPerSquare / 2, worldPosition.y);
        var gameObject = Instantiate(PermanentWall, position, Quaternion.identity, parent.transform);
        gameObject.name = $"Wall ({x}, {z})";
        gameObject.transform.localScale = new Vector3(MetersPerSquare, MetersPerSquare, MetersPerSquare);
    }

    private void CreateDestructibleWalls()
    {
        var boxes = new GameObject("Boxes");
        boxes.transform.parent = _mapRoot;
        for (var x = 1; x < FullMapWidth - 1; x++)
        {
            for (var z = 3; z < FullMapHeight - 3; z++)
            {
                CreateDestructibleWall(x, z, boxes);
            }
        }

        for (var x = 3; x < FullMapWidth - 3; x++)
        {
            CreateDestructibleWall(x, 1, boxes);
            CreateDestructibleWall(x, 2, boxes);
            CreateDestructibleWall(x, FullMapHeight - 2, boxes);
            CreateDestructibleWall(x, FullMapHeight - 3, boxes);
        }
    }

    private void CreateDestructibleWall(int x, int z, GameObject parent)
    {
        if (x % 2 == 0 && z % 2 == 0)
        {
            return;
        }

        if (Random.Range(0, 100) < GapChance)
        {
            return;
        }

        var worldPosition = GetPositionInGrid(x, z);
        var boxScale = MetersPerSquare * DestructibleWallScale;
        var position = new Vector3(worldPosition.x, boxScale.y / 2, worldPosition.y);
        var gameObject = Instantiate(DestructibleWall, position, Quaternion.identity, parent.transform);
        gameObject.name = $"DestructibleWall ({x}, {z})";
        gameObject.transform.localScale = boxScale;
        var destructibleWall = gameObject.GetComponent<DestructibleWall>();
        destructibleWall.Destroyed += DestructibleWallOnDestroyed;
    }

    private Vector2 GetPositionInGrid(int x, int z)
    {
        var halfWidth = FullMapWidth * MetersPerSquare / 2;
        var halfHeight = FullMapHeight * MetersPerSquare / 2;
        return new Vector3(
            -halfWidth + x * MetersPerSquare + MetersPerSquare / 2,
            -halfHeight + z * MetersPerSquare + MetersPerSquare / 2);
    }

    private void DestructibleWallOnDestroyed(object sender, EventArgs e)
    {
        var destructibleWall = (DestructibleWall) sender;
        destructibleWall.Destroyed -= DestructibleWallOnDestroyed;
        RebuildNavMesh();
    }
}

