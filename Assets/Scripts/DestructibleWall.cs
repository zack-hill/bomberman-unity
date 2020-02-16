using System;
using UnityEngine;

public class DestructibleWall : MonoBehaviour
{
    public event EventHandler Destroyed;

    void Start()
    {
        
    }

    void Update()
    {
        
    }

    public void Destroy()
    {
        Destroyed?.Invoke(this, EventArgs.Empty);
        Destroy(gameObject);
    }
}
