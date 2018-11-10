using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Body : MonoBehaviour {

    MeshRenderer renderer;

    public float health = 100;

    private void Awake()
    {
        renderer = GetComponent<MeshRenderer>();
    }

    public void SetAggressivenessVisual(int x, int maxDNAVal)
    {
        float scaledValue = x / maxDNAVal;
        renderer.material.color = new Color(1 - scaledValue, 0, scaledValue);
    }

    public void Damage(int amount)
    {
        health -= amount;
    }

    public void Feed(int amount)
    {
        //health = Mathf.Clamp(health += amount, 0, 100);
        health += amount;
        //resource amount is 300
        //a resource is approximately 5x bigger than a bot
        //therefore, eating the whole thing should make the bot 5 times bigger
        transform.localScale *= ((amount / 300) * 5);
    }

    private void Update()
    {
        health -= Time.deltaTime;
    }
}
