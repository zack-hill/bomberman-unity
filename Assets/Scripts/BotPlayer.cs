using System.Linq;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
public class BotPlayer : Player
{
    private NavMeshAgent _navMeshAgent;
    private GameObject _humanPlayer;

    public override void Start()
    {
        base.Start();

        _navMeshAgent = GetComponent<NavMeshAgent>();

        // For some reason, if the NavMeshAgent component is enabled by default the bot player will
        // start in the middle of the map. I should figure out what interaction is causing this.
        _navMeshAgent.enabled = true;

        //todo: This feels like an inelegant way of getting a reference to the human player.
        _humanPlayer = FindObjectsOfType<GameObject>().FirstOrDefault(x => x.GetComponent<HumanPlayer>() != null);
    }

    public override void Update()
    {
        base.Update();

        // Chase the human player! (Very creepy)
        _navMeshAgent.SetDestination(_humanPlayer.transform.position);
    }
}
