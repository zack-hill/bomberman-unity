using System;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public GameObject HumanPlayer;
    public GameObject BotPlayer;

    private MapCreator _mapCreator;

    // Start is called before the first frame update
    public void Start()
    {
        _mapCreator = GetComponent<MapCreator>();

        var spawnPoints = _mapCreator.GetSpawnPoints();

        for (var i = 0; i < spawnPoints.Count; i++)
        {
            var spawnPoint = spawnPoints[i];
            var playerGameObject = i == 0 ? Instantiate(HumanPlayer) : Instantiate(BotPlayer);
            playerGameObject.transform.position = spawnPoint;
            playerGameObject.transform.LookAt(new Vector3(0, spawnPoint.y, 0));
            playerGameObject.transform.name = $"Player {i + 1}";
            if (i > 0)
            {
                playerGameObject.transform.name += " (Bot)";
            }

            var player = playerGameObject.GetComponent<Player>();
            player.BombPlaced += PlayerOnBombPlaced;
        }
    }

    // Update is called once per frame
    public void Update()
    {

    }

    private void PlayerOnBombPlaced(object sender, GameObject bombGameObject)
    {
        var bomb = bombGameObject.GetComponent<Bomb>();
        bomb.Exploded += BombOnExploded;

        _mapCreator.RebuildNavMesh();
    }

    private void BombOnExploded(object sender, EventArgs e)
    {
        var bomb = (Bomb)sender;
        bomb.Exploded -= BombOnExploded;

        _mapCreator.RebuildNavMesh();
    }
}
