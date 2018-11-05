using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// This class controls the bots.
/// </summary>
public class Brain : MonoBehaviour
{
    public GameObject botPrefab;
    public GameObject eyes;

    public DNA dna;
    
    public int DNALength = 2;   //dna length 2 because we have 2 decisions to make
    public float timeAlive;

    bool alive = true;
    bool seeObstacle = true;

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag == "dead")
        {
            alive = false;
            //could wipe stats here if wanted               ----- NOTE -----
        }
    }

    private void OnDestroy()
    {
        Destroy(botPrefab);
    }

    public void Init()
    {
        //dna length 2, 3 possible values
        //0 - forward
        //1 - left
        //2 - right
        dna = new DNA(DNALength, 3);
        timeAlive = 0;
        alive = true;
    }

    //Different genes are used depending on input - environment will affect which gene runs
    //This should produce emergent behaviour - instructions will run depending on environment - local rules will produce behaviour, especially if the environment changes
    private void Update()
    {
        if (!alive) return;
        timeAlive = PopulationManager.elapsed;

        seeObstacle = false;

        //Register the environment - what the agent sees
        RaycastHit hit;
        if (Physics.Raycast(eyes.transform.position, eyes.transform.forward * 10, out hit))
        {
            if (!hit.collider.gameObject.tag.Equals("Walkable"))
            {
                seeObstacle = true;
            }
        }

        HandleMovement();
    }

    /// <summary>
    /// Control the movement related genes
    /// </summary>
    private void HandleMovement()
    {
        float turn = 0;
        float move = 0;

        if (seeObstacle)
        {
            if (dna.GetGene(0) == 0)
            {
                move = 1; //forward
            }
            else if (dna.GetGene(0) == 1) turn = -90;  //turn left
            else if (dna.GetGene(0) == 2) turn = 90;   //turn right
        }
        else
        {
            if (dna.GetGene(1) == 0)
            {
                move = 1;
            }
            else if (dna.GetGene(1) == 1) turn = -90;
            else if (dna.GetGene(1) == 2) turn = 90;
        }

        transform.Translate(0, 0, move * 0.1f);
        transform.Rotate(0, turn, 0);
    }
}
