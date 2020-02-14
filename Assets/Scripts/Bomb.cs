using System;
using System.Linq;
using UnityEngine;

public class Bomb : MonoBehaviour
{
    public float FuseTime = 1000;
    public float ExplosionPower = 6;
    public GameObject OwningPlayer;

    private DateTime _startTime;
    private AudioSource _audioSource;
    private Collider _collider;
    private Collider _parentCollider;

    private bool _isExploded;

    public event EventHandler Exploded;
    
    public void Start()
    {
        _startTime = DateTime.Now;

        _audioSource = GetComponent<AudioSource>();
        _collider = GetComponent<Collider>();
        _parentCollider = OwningPlayer.GetComponent<Collider>();

        Physics.IgnoreCollision(_collider, _parentCollider, true);
    }

    public void Update()
    {
        if (DateTime.Now > _startTime + TimeSpan.FromMilliseconds(FuseTime))
        {
            Explode();
        }

        if (_isExploded && !_audioSource.isPlaying)
        {
            Destroy(transform.gameObject);
        }
    }

    public void OnTriggerExit(Collider collider)
    {
        if (collider == _parentCollider)
        {
            Physics.IgnoreCollision(_collider, _parentCollider, false);
        }
    }

    private void Explode()
    {
        if (_isExploded)
        {
            return;
        }
        _isExploded = true;

        var meshRenderer = GetComponent<MeshRenderer>();
        meshRenderer.enabled = false;

        var sphereCollider = GetComponent<SphereCollider>();
        sphereCollider.enabled = false;

        var rigidbody = GetComponent<Rigidbody>();
        rigidbody.constraints = RigidbodyConstraints.FreezeAll;

        _audioSource.PlayOneShot(_audioSource.clip);

        ExplodeInDirection(new Vector3(1, 0, 0));
        ExplodeInDirection(new Vector3(-1, 0, 0));
        ExplodeInDirection(new Vector3(0, 0, 1));
        ExplodeInDirection(new Vector3(0, 0, -1));

        Exploded?.Invoke(this, EventArgs.Empty);
    }

    private void ExplodeInDirection(Vector3 direction)
    {
        var radius = transform.localScale.x;

        Debug.DrawLine(transform.position, transform.position + direction * ExplosionPower, Color.red, 10, false);

        // Check all hits in ascending order of the distance from the bomb
        foreach (var hit in Physics.SphereCastAll(transform.position, radius, direction, ExplosionPower).OrderBy(x => x.distance))
        {
            // Ignore the exploding bomb and the floor
            if (hit.transform == transform || hit.transform.name == "Floor")
            {
                continue;
            }

            switch (hit.transform.tag)
            {
                case "Box":
                    Destroy(hit.transform.gameObject);
                    return;
                case "Bomb":
                    //todo: we may want to explode all subsequent explosions after initial explosions to ensure that kills are reported correctly.
                    hit.transform.gameObject.GetComponent<Bomb>().Explode();
                    return;
                case "Player":
                    hit.transform.gameObject.GetComponent<Player>().Kill();
                    return;
                default:
                    // An intersection has been found with some object that is not
                    // destructible, but does block damage, likely a wall.
                    return;
            }
        }
    }
}
