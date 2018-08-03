using System;
using UnityEngine;

public class Bomb : MonoBehaviour
{
    public float FuseTime = 1000;
    public GameObject Parent;

    private DateTime _startTime;
    private Collider _collider;
    private Collider _parentCollider;

    public void Start()
    {
        _startTime = DateTime.Now;

        _collider = GetComponent<Collider>();
        _parentCollider = Parent.GetComponent<Collider>();

        Physics.IgnoreCollision(_collider, _parentCollider, true);
    }

    public void Update()
    {
        if (DateTime.Now > _startTime + TimeSpan.FromMilliseconds(FuseTime))
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
}
