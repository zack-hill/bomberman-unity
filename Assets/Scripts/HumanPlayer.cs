using UnityEngine;

public class HumanPlayer : Player
{
    public override void Start()
    {
        base.Start();

    }

    public override void Update()
    {
        base.Update();

        if (IsAlive && Input.GetButtonDown("Fire1"))
        {
            PlaceBomb();
        }
    }
}