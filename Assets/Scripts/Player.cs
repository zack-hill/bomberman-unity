using System;
using UnityEngine;

public class Player : MonoBehaviour
{
    public GameObject Bomb;

    protected int MaxBombCount = 2;
    protected int ActiveBombCount;
    protected bool IsAlive = true;
    
    public virtual void Start()
    {
    }

    public virtual void Update()
    {
    }

    public void Kill()
    {
        if (!IsAlive)
        {
            return;
        }
        IsAlive = false;
        GetComponent<MeshRenderer>().enabled = false;
        GetComponent<CapsuleCollider>().enabled = false;
        foreach (var meshRenderer in GetComponentsInChildren<MeshRenderer>())
        {
            meshRenderer.enabled = false;
        }
        Destroy(GetComponent<PlayerMovement>());
        Destroy(GetComponent<Rigidbody>());
    }


    protected void PlaceBomb()
    {
        if (ActiveBombCount == MaxBombCount)
        {
            return;
        }

        var bombGameObject = Instantiate(Bomb, transform.position, Quaternion.identity);
        var bomb = bombGameObject.GetComponent<Bomb>();
        bomb.OwningPlayer = gameObject;
        bomb.Exploded += BombOnExploded;

        ActiveBombCount += 1;
    }

    private void BombOnExploded(object sender, EventArgs e)
    {
        ActiveBombCount -= 1;
    }
}
