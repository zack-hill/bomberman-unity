using UnityEngine;

public class Player : MonoBehaviour
{
    public GameObject Bomb;
    
    public void Start()
    {
    }

    public void Update()
    {
        if (Input.GetButtonDown("Fire1"))
        {
            var bomb = Instantiate(Bomb, transform.position, Quaternion.identity);
            bomb.GetComponent<Bomb>().Parent = gameObject;
        }
    }
}
