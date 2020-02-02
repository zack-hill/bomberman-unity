using System;
using UnityEngine;

public class Bomb : MonoBehaviour
{
    public float FuseTime = 1000;
    public float ExplosionPower = 6;
    public GameObject Parent;

    private DateTime _startTime;
    private AudioSource _audioSource;
    private Collider _collider;
    private Collider _parentCollider;

    private bool _isExploded;


    public void Start()
    {
        _startTime = DateTime.Now;

        _audioSource = GetComponent<AudioSource>();
        _collider = GetComponent<Collider>();
        _parentCollider = Parent.GetComponent<Collider>();

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
    }

    private void ExplodeInDirection(Vector3 direction)
    {
        var radius = transform.localScale.x;
        var distance = ExplosionPower;
        if (Physics.SphereCast(new Ray(transform.position, direction), radius, out var hit, ExplosionPower))
        {
            if (hit.transform.tag == "Box")
            {
                Destroy(hit.transform.gameObject);
            }
            if (hit.transform.tag == "Bomb")
            {
                //todo: may want to explode all subsequent explosions after initial explosions to ensure that kills are reported correctly.
                hit.transform.gameObject.GetComponent<Bomb>().Explode();
            }

            distance = hit.distance;
        }

        var start = transform.position;
        var end = transform.position + direction * distance;
        Debug.DrawLine(start, end, Color.red, 2, false);
    }
}
