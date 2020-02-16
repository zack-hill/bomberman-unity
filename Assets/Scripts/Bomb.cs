using System;
using System.Linq;
using UnityEngine;

public class Bomb : MonoBehaviour
{
    public float FuseTime = 1000;
    public float ExplosionPower = 3;
    public GameObject OwningPlayer;
    public ParticleSystem FireParticles;

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

        var xPosDist = ExplodeInDirection(new Vector3(1, 0, 0));
        var xNegDist = ExplodeInDirection(new Vector3(-1, 0, 0));
        var zPosDist = ExplodeInDirection(new Vector3(0, 0, 1));
        var zNegDist = ExplodeInDirection(new Vector3(0, 0, -1));

        var xTotal = xPosDist + xNegDist;
        var zTotal = zPosDist + zNegDist;

        var xCenter = transform.position.x - xNegDist + xTotal / 2;
        var zCenter = transform.position.z - zNegDist + zTotal / 2;

        var xParticles = Instantiate(FireParticles, new Vector3(xCenter, transform.position.y, transform.position.z), Quaternion.identity);
        var zParticles = Instantiate(FireParticles, new Vector3(transform.position.x, transform.position.y, zCenter), Quaternion.identity);

        xParticles.transform.localScale = new Vector3(xTotal, transform.localScale.y, transform.localScale.z);
        zParticles.transform.localScale = new Vector3(transform.localScale.z, transform.localScale.y, zTotal);

        Exploded?.Invoke(this, EventArgs.Empty);
    }

    private float ExplodeInDirection(Vector3 direction)
    {
        var radius = transform.localScale.x / 2;

        Debug.DrawLine(transform.position, transform.position + direction * ExplosionPower, Color.red, 10, false);

        // Check all hits in ascending order of the distance from the bomb
        foreach (var hit in Physics.SphereCastAll(transform.position, radius, direction, ExplosionPower).OrderBy(x => x.distance))
        {
            // Ignore the exploding bomb and the floor
            if (hit.transform == transform || hit.transform.name == "Floor")
            {
                continue;
            }

            Hit(hit.transform.gameObject);

            return hit.distance;
        }

        return ExplosionPower;
    }

    private static void Hit(GameObject obj)
    {
        var player = obj.GetComponent<Player>();
        if (player != null)
        {
            player.Kill();
            return;
        }

        var destructibleWall = obj.GetComponent<DestructibleWall>();
        if (destructibleWall != null)
        {
            destructibleWall.Destroy();
            return;
        }

        //todo: we may want to explode all subsequent explosions after initial explosions to ensure that kills are reported correctly.
        var bomb = obj.GetComponent<Bomb>();
        if (bomb != null)
        {
            bomb.Explode();
        }
    }
}
