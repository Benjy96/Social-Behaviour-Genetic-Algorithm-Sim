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
    private Body body;
    
    private int DNALength = 8;   //dna length 8 because 8 decisions currently implemented
    private int dnaValues = 4;
   
    bool alive = true;
     
    //TODO: make a dynamic dictionary
    bool seeObstacle = false;
    bool seeOther = false;
    bool seeResource = false;

    public int GetHealth()
    {
        return (int)body.health;
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag == "Deadly" || body.health <= 0)
        {
            alive = false;
            Debug.Log("Ouch!");
            //could wipe stats here if wanted               ----- NOTE -----
            gameObject.SetActive(false);
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
        dna = new DNA(DNALength, dnaValues);
        timeAlive = 0;
        alive = true;

        body = GetComponent<Body>();
        body.SetAggressivenessVisual(dna.GetGene(2), dnaValues);
    }

    //Different genes are used depending on input - environment will affect which gene runs
    //This should produce emergent behaviour - instructions will run depending on environment - local rules will produce behaviour, especially if the environment changes
    private void Update()
    {
        if (!alive) return;
        timeAlive = PopulationManager.elapsed;
        Debug.DrawRay(eyes.transform.position, eyes.transform.forward * 10, Color.red, 10);

        seeObstacle = false;
        seeOther = false;
        GameObject other = null;

        seeResource = false;
        GameObject resource = null;

        //Register the environment - what the agent sees
        RaycastHit hit;
        if (Physics.Raycast(eyes.transform.position, eyes.transform.forward * 10, out hit))
        {
            if (!hit.collider.gameObject.tag.Equals("Walkable"))
            {
                seeObstacle = true;
                
            }
            else if (hit.collider.gameObject.tag.Equals("Bot"))
            {
                seeOther = true;
                other = hit.collider.gameObject;
            }
            else if (hit.collider.gameObject.tag.Equals("Resource"))
            {
                seeResource = true;
                resource = hit.collider.gameObject;
            }
        }

        RunMovementGenes();
        RunInteractionGenes(other);
        RunResourceGenes(resource);
    }

    /// <summary>
    /// Control the movement related genes (Each gene represents a possible instruction to run)
    /// 3 bools (true or false) means 6 genes - 6 decisions to be made
    /// </summary>
    private void RunMovementGenes()
    {
        float turn = 0;
        float move = 0;

        //TODO: Sum result of genes? The last gene will overwrite all previous - think flocking algorithm
        //Need weights since each bool will have a say on input - could weight genes, or make gene weighting genes
        //But yea, if we have multiple setters of the move variable, it needs to be sums rather than assignments, unless we just do one bool for movement
        //TODO: work out how to have multiple environmental factors within genes
        if (seeObstacle)
        {
            if (dna.GetGene(0) == (int)GeneInstructions.MovementForward) move += 0.33f;
            else if (dna.GetGene(0) == (int)GeneInstructions.MovementLeft) turn -= 90;
            else if (dna.GetGene(0) == (int)GeneInstructions.MovementRight) turn += 90;
            else if (dna.GetGene(0) == (int)GeneInstructions.MovementStop) move -= 33;
        }
        else
        {
            if (dna.GetGene(1) == (int)GeneInstructions.MovementForward) move += 0.33f;
            else if (dna.GetGene(1) == (int)GeneInstructions.MovementLeft) turn -= 90;
            else if (dna.GetGene(1) == (int)GeneInstructions.MovementRight) turn += 90;
            else if (dna.GetGene(1) == (int)GeneInstructions.MovementStop) move -= 33;
        }

        if (seeOther)
        {
            if (dna.GetGene(2) == (int)GeneInstructions.MovementForward) move += 0.33f;
            else if (dna.GetGene(2) == (int)GeneInstructions.MovementLeft) turn -= 90;
            else if (dna.GetGene(2) == (int)GeneInstructions.MovementRight) turn += 90;
            else if (dna.GetGene(2) == (int)GeneInstructions.MovementStop) move -= 33;
        }
        else
        {
            if (dna.GetGene(3) == (int)GeneInstructions.MovementForward) move += 0.33f;
            else if (dna.GetGene(3) == (int)GeneInstructions.MovementLeft) turn -= 90;
            else if (dna.GetGene(3) == (int)GeneInstructions.MovementRight) turn += 90;
            else if (dna.GetGene(3) == (int)GeneInstructions.MovementStop) move -= 33;
        }

        if (seeResource)
        {
            if (dna.GetGene(4) == (int)GeneInstructions.MovementForward) move += 0.33f;
            else if (dna.GetGene(4) == (int)GeneInstructions.MovementLeft) turn -= 90;
            else if (dna.GetGene(4) == (int)GeneInstructions.MovementRight) turn += 90;
            else if (dna.GetGene(4) == (int)GeneInstructions.MovementStop) move -= 33;
        }
        else
        {
            if (dna.GetGene(5) == (int)GeneInstructions.MovementForward) move += 0.33f;
            else if (dna.GetGene(5) == (int)GeneInstructions.MovementLeft) turn -= 33;
            else if (dna.GetGene(5) == (int)GeneInstructions.MovementRight) turn += 33;
            else if (dna.GetGene(5) == (int)GeneInstructions.MovementStop) move -= 0.33f;
        }

        move = Mathf.Clamp(move, 0, 1);

        transform.Translate(0, 0, move * 0.1f);
        transform.Rotate(0, turn, 0);
    }

    private void RunInteractionGenes(GameObject other)
    {
        if (seeOther)
        {
            //TODO: add inner options, such as see colour of enemy and take action depending on their colour
            Body otherBody = other.GetComponent<Body>();

            if (dna.GetGene(6) == (int)GeneInstructions.InteractionAttack)
            {
                otherBody.Damage(20);   //todo: remove hard coding
                Debug.DrawLine(transform.position, other.transform.position, Color.green);
            }
            else if (dna.GetGene(6) == (int)GeneInstructions.InteractionGive)
            {
                otherBody.Feed(20);
                //TODO: Add benefits of feeding, e.g.:
                    //1. Increased "visual" indicator of friendliness
                    //2. Money from other
            }
            else if(dna.GetGene(6) == (int)GeneInstructions.InteractionTrade)
            {
                //TODO: implement if have, feed other, get money
                //TODO: events? benefits / consequences simpler than communication
            }
            else if (dna.GetGene(6) == (int)GeneInstructions.InteractionIgnore) return;
        }
    }

    private void RunResourceGenes(GameObject resource)
    {
        if (seeResource)
        {
            Resource r = resource.GetComponent<Resource>();

            if (dna.GetGene(7) == (int)GeneInstructions.ResourceLeave)
            {
                //Do nothing
                Debug.Log("I'm on a diet");
            }
            else if (dna.GetGene(7) == (int)GeneInstructions.ResourceTake)
            {
                //TODO: Pickup - will drop on death
                //Debug.Log("I think I'll keep this for later.");
            }
            else if (dna.GetGene(7) == (int)GeneInstructions.ResourceEat)
            {
                Debug.Log("Fuck me, that looks tasty. I'll have some of that.");
                r.Eat(20);
                body.Feed(20);
            }
            else if(dna.GetGene(7) == (int)GeneInstructions.ResourceSpoil)
            {
                //TODO: Implement consequence
                Debug.Log("Eat this, fuckers.");
                r.Spoil();
            }
        }
    }
}

public enum GeneInstructions
{
    MovementForward = 0,
    MovementLeft,
    MovementRight,
    MovementStop,
    InteractionAttack,
    InteractionGive,
    InteractionTrade,
    InteractionIgnore,
    ResourceLeave,
    ResourceTake,
    ResourceEat,
    ResourceSpoil
}
