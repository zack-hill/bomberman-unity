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
            var player = i == 0 ? Instantiate(HumanPlayer) : Instantiate(BotPlayer);
            player.transform.position = spawnPoint;
            player.transform.LookAt(new Vector3(0, spawnPoint.y, 0));
            player.transform.name = $"Player {i + 1}";
            if (i > 0)
            {
                player.transform.name += " (Bot)";
            }
        }
    }

    // Update is called once per frame
    public void Update()
    {
        
    }
}
