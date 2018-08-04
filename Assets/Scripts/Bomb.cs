using System;
using UnityEngine;

public class Bomb : MonoBehaviour
{
    public float FuseTime = 1000;
    public float ExplositionPower = 6;
    public GameObject Parent;

    private DateTime _startTime;
    private Rigidbody _rigidbody;
    private Collider _collider;
    private Collider _parentCollider;
    private float _debugTime = 0;
    private MeshRenderer _meshRenderer;
    private bool _isExploded;


    public void Start()
    {
        _startTime = DateTime.Now;

        _meshRenderer = GetComponent<MeshRenderer>();
        _rigidbody = GetComponent<Rigidbody>();
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
        if (DateTime.Now > _startTime + TimeSpan.FromMilliseconds(FuseTime) + TimeSpan.FromMilliseconds(_debugTime))
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

        _meshRenderer.enabled = false;
        _rigidbody.constraints = RigidbodyConstraints.FreezeAll;
        ExplodeInDirection(new Vector3(1, 0, 0));
        ExplodeInDirection(new Vector3(-1, 0, 0));
        ExplodeInDirection(new Vector3(0, 0, 1));
        ExplodeInDirection(new Vector3(0, 0, -1));
    }

    private void ExplodeInDirection(Vector3 direction)
    {
        var radius = transform.localScale.x;
        RaycastHit hit;
        var distance = ExplositionPower;
        if (Physics.SphereCast(new Ray(transform.position, direction), radius, out hit, ExplositionPower))
        {
            if (hit.transform.tag == "Box")
            {
                Destroy(hit.transform.gameObject);
            }
            if (hit.transform.tag == "Bomb")
            {
                hit.transform.gameObject.GetComponent<Bomb>().Explode();
            }

            distance = hit.distance;
        }

        var start = transform.position;
        var end = transform.position + direction * distance;
        Debug.DrawLine(start, end, Color.red, 2, false);
    }
}
