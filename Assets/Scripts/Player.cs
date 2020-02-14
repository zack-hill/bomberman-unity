using System;
using UnityEngine;

public class Player : MonoBehaviour
{
    public GameObject Bomb;

    private int _maxBombCount = 1;
    private int _activeBombCount;
    
    public void Start()
    {
    }

    public void Update()
    {
        if (Input.GetButtonDown("Fire1"))
        {
            if (_activeBombCount == _maxBombCount)
            {
                return;
            }

            var bombGameObject = Instantiate(Bomb, transform.position, Quaternion.identity);
            var bomb = bombGameObject.GetComponent<Bomb>();
            bomb.OwningPlayer = gameObject;
            bomb.Exploded += BombOnExploded;

            _activeBombCount += 1;
        }
    }

    public void Kill()
    {
        GetComponent<MeshRenderer>().enabled = false;
        GetComponent<CapsuleCollider>().enabled = false;
        foreach (var meshRenderer in GetComponentsInChildren<MeshRenderer>())
        {
            meshRenderer.enabled = false;
        }
        Destroy(GetComponent<PlayerMovement>());
        Destroy(GetComponent<Rigidbody>());
    }

    private void BombOnExploded(object sender, EventArgs e)
    {
        _activeBombCount -= 1;
    }
}
