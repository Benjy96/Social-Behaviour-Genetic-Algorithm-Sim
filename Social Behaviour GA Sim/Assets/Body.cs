using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Body : MonoBehaviour {

    MeshRenderer renderer;

    public float health = 100;

    private void Awake()
    {
        renderer = GetComponent<MeshRenderer>();
        transform.localScale = new Vector3(1f, 1f, 1f);
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

    public void Feed(float amount)
    {
        //health = Mathf.Clamp(health += amount, 0, 100);
        health += amount;
        //resource amount is 300
        //a resource is approximately 5x bigger than a bot
        //therefore, eating the whole thing should make the bot approx 5 times bigger - this makes them more easily spotted by others
        float newScaleFactor = transform.localScale.x + ((amount / 300f) * 5f);
        gameObject.transform.localScale = new Vector3(newScaleFactor, newScaleFactor, newScaleFactor);
        Debug.Log("local scale is: " + gameObject.transform.localScale);
    }

    private void Update()
    {
        health -= Time.deltaTime;
    }
}
