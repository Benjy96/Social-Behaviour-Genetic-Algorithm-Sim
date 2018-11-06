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
    
    private int DNALength = 7;   //dna length 7 because 7 decisions currently implemented
    private int dnaValues = 3;
   
    bool alive = true;
     
    //TODO: make a dynamic dictionary
    bool seeObstacle = false;
    bool seeOther = false;
    bool seeResource = false;

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
        }

        RunInteractionGenes(other);
        RunMovementGenes();
    }

    /// <summary>
    /// Control the movement related genes (Each gene represents a possible instruction to run)
    /// 3 bools (true or false) means 6 genes - 6 decisions to be made
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

        if (seeOther)
        {
            if (dna.GetGene(2) == (int)GeneInstructions.MovementForward) move = 1;
            else if (dna.GetGene(2) == (int)GeneInstructions.MovementLeft) turn = -90;
            else if (dna.GetGene(2) == (int)GeneInstructions.MovementRight) turn = 90;
        }
        else
        {
            if (dna.GetGene(3) == (int)GeneInstructions.MovementForward) move = 1;
            else if (dna.GetGene(3) == (int)GeneInstructions.MovementLeft) turn = -90;
            else if (dna.GetGene(3) == (int)GeneInstructions.MovementRight) turn = 90;
        }

        if (seeResource)
        {
            if (dna.GetGene(4) == (int)GeneInstructions.MovementForward) move = 1;
            else if (dna.GetGene(4) == (int)GeneInstructions.MovementLeft) turn = -90;
            else if (dna.GetGene(4) == (int)GeneInstructions.MovementRight) turn = 90;
        }
        else
        {
            if (dna.GetGene(5) == (int)GeneInstructions.MovementForward) move = 1;
            else if (dna.GetGene(5) == (int)GeneInstructions.MovementLeft) turn = -90;
            else if (dna.GetGene(5) == (int)GeneInstructions.MovementRight) turn = 90;
        }

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
            else if (dna.GetGene(6) == (int)GeneInstructions.InteractionIgnore) return;
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
