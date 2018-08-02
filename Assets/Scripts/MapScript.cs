using UnityEngine;

public class MapScript : MonoBehaviour
{
    public int MapWidth = 15;

    public int MapHeight = 13;

    public float MetersPerSquare = 3;

    public Material FloorMaterial;

    public GameObject PermanentWall;

    private float _yHeight = 2.5f;

    // Use this for initialization
    void Start()
    {
        CreateFloor();
        CreatePermanentWalls();
    }

    // Update is called once per frame
    void Update()
    {

    }

    private void CreateFloor()
    {
        var halfWidth = MapWidth * MetersPerSquare / 2;
        var halfHeight = MapHeight * MetersPerSquare / 2;
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
            normals = new[] { Vector3.up, Vector3.up, Vector3.up, Vector3.up },
            triangles = new[] { 0, 1, 2, 0, 2, 3 }
        };
        mesh.RecalculateNormals();
        mesh.RecalculateBounds();
        var gameObject = new GameObject("Floor");
        var collider = gameObject.AddComponent<MeshCollider>();
        collider.sharedMesh = mesh;
        var renderer = gameObject.AddComponent<MeshRenderer>();
        renderer.material = FloorMaterial;
        renderer.material.mainTextureScale = new Vector2(MapWidth, MapHeight);
        var filter = gameObject.AddComponent<MeshFilter>();
        filter.mesh = mesh;
    }

    private void CreatePermanentWalls()
    {
        var permanentWalls = new GameObject("Permanent Walls");
        for (var x = 0; x < MapWidth; x++)
        {
            CreatePermanentWall(x, 0, 1, permanentWalls);
            CreatePermanentWall(x, MapHeight - 1, 1, permanentWalls);
        }
        for (var z = 1; z < MapHeight - 1; z++)
        {
            CreatePermanentWall(0, z, 1, permanentWalls);
            CreatePermanentWall(MapWidth - 1, z, 1, permanentWalls);
        }

        for (var x = 1; x < MapWidth - 1; x++)
        {
            for (var z = 1; z < MapHeight - 1; z++)
            {
                if (x % 2 == 0 && z % 2 == 0)
                {
                    CreatePermanentWall(x, z, 0.75f, permanentWalls);
                }
            }
        }
    }

    private void CreatePermanentWall(float x, float z, float yScale, GameObject parent)
    {
        var halfWidth = MapWidth * MetersPerSquare / 2;
        var halfHeight = MapHeight * MetersPerSquare / 2;
        
        var position = new Vector3(
            -halfWidth + x * MetersPerSquare + MetersPerSquare / 2,
            _yHeight * yScale / 2,
            -halfHeight + z * MetersPerSquare + MetersPerSquare / 2);
        var gameObject = Instantiate(PermanentWall, position, Quaternion.identity, parent.transform);
        gameObject.transform.localScale = new Vector3(MetersPerSquare, _yHeight * yScale, MetersPerSquare);
    }
}
