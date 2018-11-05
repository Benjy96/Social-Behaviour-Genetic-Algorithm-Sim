using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// This class controls the bots.
/// </summary>
public class Brain : MonoBehaviour
{
    public float timeAlive;

    public GameObject botPrefab;
    public GameObject eyes;

    public DNA dna;
    
    private int DNALength = 6;   //dna length 6 because we have 6 decisions to make
   
    bool alive = true;
     
    //TODO: make a dynamic dictionary
    bool seeObstacle = false;
    bool seeOther = false;
    bool seeResource = false;

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
        DNALength = 
    }

    //Different genes are used depending on input - environment will affect which gene runs
    //This should produce emergent behaviour - instructions will run depending on environment - local rules will produce behaviour, especially if the environment changes
    private void Update()
    {
        if (!alive) return;
        timeAlive = PopulationManager.elapsed;

        seeObstacle = false;
        seeOther = false;
        GameObject other = null;

        seeResource = false;

        //Register the environment - what the agent sees
        RaycastHit hit;
        if (Physics.Raycast(eyes.transform.position, eyes.transform.forward * 10, out hit))
        {
            if (!hit.collider.gameObject.tag.Equals("Walkable"))
            {
                seeObstacle = true;
                RunMovementGenes();
            }
            else if (hit.collider.gameObject.tag.Equals("Bot"))
            {
                seeOther = true;
                other = hit.collider.gameObject;
                RunInteractionGenes(other);
            }
        }
    }

    /// <summary>
    /// Control the movement related genes (Each gene represents a possible instruction to run)
    /// </summary>
    private void RunMovementGenes()
    {
        float turn = 0;
        float move = 0;

        if (seeObstacle)
        {
            if (dna.GetGene(0) == (int)GeneInstructions.MovementForward) move = 1;
            else if (dna.GetGene(0) == (int)GeneInstructions.MovementLeft) turn = -90;  
            else if (dna.GetGene(0) == (int)GeneInstructions.MovementRight) turn = 90;  
        }
        else
        {
            if (dna.GetGene(1) == (int)GeneInstructions.MovementForward) move = 1;
            else if (dna.GetGene(1) == (int)GeneInstructions.MovementLeft) turn = -90;
            else if (dna.GetGene(1) == (int)GeneInstructions.MovementRight) turn = 90;
        }

        transform.Translate(0, 0, move * 0.1f);
        transform.Rotate(0, turn, 0);
    }

    private void RunInteractionGenes(GameObject other)
    {
        if (seeOther)
        {
            Body otherBody = other.GetComponent<Body>();

            if (dna.GetGene(2) == (int)GeneInstructions.InteractionAttack)
            {
                otherBody.Damage(20);   //todo: remove hard coding
            }
            else if (dna.GetGene(2) == (int)GeneInstructions.InteractionGive)
            {
                otherBody.Feed(20);
                //TODO: Add benefits of feeding, e.g.:
                    //1. Increased "visual" indicator of friendliness
                    //2. Money from other
            }
            else if (dna.GetGene(2) == (int)GeneInstructions.InteractionIgnore) return;
        }
        else
        {
            if (dna.GetGene(3) == (int)GeneInstructions.InteractionAttack)
            {
                //TODO: Implement attacking
            }
            else if (dna.GetGene(3) == (int)GeneInstructions.InteractionGive)
            {
                //TODO: Implement resources
            }
            else if (dna.GetGene(3) == (int)GeneInstructions.InteractionIgnore) return;
        }
    }

    private void RunResourceGenes()
    {

    }
}

public enum GeneInstructions
{
    MovementForward = 0,
    MovementLeft,
    MovementRight,
    InteractionAttack,
    InteractionGive,
    InteractionIgnore,
    ResourceLeave,
    ResourceTake
}
