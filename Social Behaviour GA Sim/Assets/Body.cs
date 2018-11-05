using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Body : MonoBehaviour {

    public int health = 100;

    public void Damage(int amount)
    {
        health -= amount;
    }

    public void Feed(int amount)
    {
        health = Mathf.Clamp(health += amount, 0, 100);
    }
}
