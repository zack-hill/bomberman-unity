using UnityEngine;

public class GameManager : MonoBehaviour
{
    public GameObject Player;

    private MapCreator _mapCreator;

    // Start is called before the first frame update
    public void Start()
    {
        _mapCreator = GetComponent<MapCreator>();

        foreach (var spawnPoint in _mapCreator.GetSpawnPoints())
        {
            var player = Instantiate(Player);
            player.transform.position = spawnPoint;
            player.transform.LookAt(new Vector3(0, spawnPoint.y, 0)); 
        }
    }

    // Update is called once per frame
    public void Update()
    {
        
    }
}
