using System.Linq;
using UnityEngine;

public class HumanPlayer : Player
{
    private MapCreator _mapCreator;

    public override void Start()
    {
        base.Start();

        _mapCreator = FindObjectsOfType<GameObject>()
            .First(x => x.GetComponent<MapCreator>() != null)
            .GetComponent<MapCreator>();
    }

    public override void Update()
    {
        base.Update();

        if (IsAlive && Input.GetButtonDown("Fire1"))
        { 
            PlaceBomb();
        }

        if (Input.GetKeyDown(KeyCode.R))
        {
            _mapCreator.RebuildNavMesh();
        }
    }

    public override void Kill()
    {
        if (!IsAlive)
        {
            return;
        }

        base.Kill();

        Destroy(GetComponent<PlayerMovement>());
    }
}