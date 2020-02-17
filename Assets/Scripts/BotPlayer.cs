using System.Diagnostics;
using System.Linq;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
public class BotPlayer : Player
{
    private NavMeshAgent _navMeshAgent;
    private GameObject _humanPlayer;

    private GameObject _currentTarget;
    private readonly Stopwatch _timeSinceLastPathUpdate = new Stopwatch();
    private readonly int _pathUpdateIntervalMs = 100;

    public override void Start()
    {
        base.Start();

        _navMeshAgent = GetComponent<NavMeshAgent>();

        // For some reason, if the NavMeshAgent component is enabled by default the bot player will
        // start in the middle of the map. I should figure out what interaction is causing this.
        _navMeshAgent.enabled = true;

        //todo: This feels like an inelegant way of getting a reference to the human player.
        _humanPlayer = FindObjectsOfType<GameObject>().FirstOrDefault(x => x.GetComponent<HumanPlayer>() != null);
        _currentTarget = _humanPlayer;
        _timeSinceLastPathUpdate.Start();
    }

    public override void Update()
    {
        base.Update();

        if (IsAlive)
        {
            //PlaceBomb();
            // Chase the human player! (Very creepy)
            //_navMeshAgent.SetDestination();

            //var filter = new NavMeshQueryFilter
            //{
            //    agentTypeID = _navMeshAgent.agentTypeID,
            //    areaMask = _navMeshAgent.areaMask,
            //};

            if (_currentTarget != null && _timeSinceLastPathUpdate.ElapsedMilliseconds > _pathUpdateIntervalMs)
            {
                _navMeshAgent.SetDestination(_currentTarget.transform.position);
                _timeSinceLastPathUpdate.Restart();
            }

            //var path = new NavMeshPath();
            //if (NavMesh.CalculatePath(transform.position, _humanPlayer.transform.position, filter, path))
            //{
            //    _navMeshAgent.SetPath(path);
            //}
        }
    }

    public override void Kill()
    {
        if (!IsAlive)
        {
            return;
        }

        base.Kill();

        _navMeshAgent.enabled = false;
    }

    // 1) Move to near a destructible wall or player
    // 2) Place bomb
    // 3) While a bomb is placed, find a safe cell
}
