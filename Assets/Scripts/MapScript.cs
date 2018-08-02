using UnityEngine;
using Random = UnityEngine.Random;

namespace Assets.Scripts
{
    public class MapScript : MonoBehaviour
    {
        [Range(5, 25)]
        public int MapWidth = 15;

        [Range(5, 25)]
        public int MapHeight = 13;

        public float MetersPerSquare = 3;

        public float BoxScale = 0.75f;

        public Material FloorMaterial;

        public GameObject Box;

        public GameObject PermanentWall;

        private float _yHeight = 2.5f;

        private float _percentageChanceToSkipBox = 10;

        private int _fullMapWidth => MapWidth + 2;

        private int _fullMapHeight => MapHeight + 2;

        // Use this for initialization
        void Start()
        {
            CreateFloor();
            CreatePermanentWalls();
            CreateBoxes();
        }

        // Update is called once per frame
        void Update()
        {

        }

        private void CreateFloor()
        {
            var halfWidth = _fullMapWidth * MetersPerSquare / 2;
            var halfHeight = _fullMapHeight * MetersPerSquare / 2;
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
            renderer.material.mainTextureScale = new Vector2(_fullMapWidth, _fullMapHeight);
            var filter = gameObject.AddComponent<MeshFilter>();
            filter.mesh = mesh;
        }

        private void CreatePermanentWalls()
        {
            var permanentWalls = new GameObject("Permanent Walls");
            for (var x = 0; x < _fullMapWidth; x++)
            {
                CreatePermanentWall(x, 0, permanentWalls);
                CreatePermanentWall(x, _fullMapHeight - 1, permanentWalls);
            }
            for (var z = 1; z < _fullMapHeight - 1; z++)
            {
                CreatePermanentWall(0, z, permanentWalls);
                CreatePermanentWall(_fullMapWidth - 1, z, permanentWalls);
            }

            for (var x = 1; x < _fullMapWidth - 1; x++)
            {
                for (var z = 1; z < _fullMapHeight - 1; z++)
                {
                    if (x % 2 == 0 && z % 2 == 0)
                    {
                        CreatePermanentWall(x, z, permanentWalls);
                    }
                }
            }
        }

        private void CreatePermanentWall(int x, int z, GameObject parent)
        {
            var worldPosition = GetPositionInGrid(x, z);
            var position = new Vector3(worldPosition.x, _yHeight / 2, worldPosition.y);
            var gameObject = Instantiate(PermanentWall, position, Quaternion.identity, parent.transform);
            gameObject.transform.localScale = new Vector3(MetersPerSquare, _yHeight, MetersPerSquare);
        }

        private void CreateBoxes()
        {
            var boxes = new GameObject("Boxes");
            for (var x = 1; x < _fullMapWidth - 1; x++)
            {
                for (var z = 3; z < _fullMapHeight - 3; z++)
                {
                    CreateBox(x, z, boxes);
                }
            }
            for (var x = 3; x < _fullMapWidth - 3; x++)
            {
                CreateBox(x, 1, boxes);
                CreateBox(x, 2, boxes);
                CreateBox(x, _fullMapHeight - 2, boxes);
                CreateBox(x, _fullMapHeight - 3, boxes);
            }
        }

        private void CreateBox(int x, int z, GameObject parent)
        {
            if (x % 2 == 0 && z % 2 == 0)
            {
                return;
            }
            if (Random.Range(0, 100) < _percentageChanceToSkipBox)
            {
                return;
            }

            var worldPosition = GetPositionInGrid(x, z);
            var boxScale = MetersPerSquare * BoxScale;
            var position = new Vector3(worldPosition.x, boxScale / 2, worldPosition.y);
            var gameObject = Instantiate(Box, position, Quaternion.identity, parent.transform);
            gameObject.transform.localScale = new Vector3(boxScale, boxScale, boxScale);
        }

        private Vector2 GetPositionInGrid(int x, int z)
        {
            var halfWidth = _fullMapWidth * MetersPerSquare / 2;
            var halfHeight = _fullMapHeight * MetersPerSquare / 2;
            return new Vector3(
                -halfWidth + x * MetersPerSquare + MetersPerSquare / 2,
                -halfHeight + z * MetersPerSquare + MetersPerSquare / 2);
        }
    }
}
