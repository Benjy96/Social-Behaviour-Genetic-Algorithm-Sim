using UnityEngine;

public class Resource : MonoBehaviour {

    public int resourceAmount = 300;

    public void Eat(int amount)
    {
        resourceAmount = Mathf.Clamp(resourceAmount -= amount, 0, resourceAmount);
        if (resourceAmount <= 0) Spoil();
    }

    public void Spoil()
    {
        Destroy(gameObject);
    }
}
