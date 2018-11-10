using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// This class controls the bots. Think of them as cells - keep it simple.
/// These cells have DNA - which contain genes, and genes run instructions.
/// Genes react to external stimuli. Only some genes react to some stimuli. For example, in real life, Gene 34356 may react to a chemical catalyst by growing a section of a cell.
/// </summary>
public class Brain : MonoBehaviour
{
    public float timeAlive;

    public GameObject botPrefab;
    public GameObject eyes;

    public DNA dna;
    private Body body;
    
    private int DNALength = 6;   //dna length 6 because 6 decisions currently implemented
    private int dnaValues = 4;
   
    bool alive = true;

    //TODO: make a dynamic dictionary
    //TODO: Make genes decide which decisions to make - e.g., if (Gene(x) == 1) consider Genes 3-5, else consider genes 6-7 (this could be for considering an enemy over food, for example - let the dna rank importance of stimuli)
    bool seeWalkable = false;
    bool seeObstacle = false;
    bool seeOther = false;
    bool seeResource = false;

    public int GetHealth()
    {
        return (int)body.health;
    }

    public void Kill()
    {
        body.health = 0;
        alive = false;
        Debug.Log("Ouch!");
        gameObject.SetActive(false);
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag == "Deadly" || body.health <= 0)
        {
            Kill();
        }
    }

    private void OnDestroy()
    {
        Destroy(botPrefab);
    }

    public void Init()
    {
        //dna length 2, 4 possible values
        //0 - forward
        //1 - left
        //2 - right
        //3 - stop
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

        //Register the environment - what the agent sees - can see 1 thing at a time
        RaycastHit hit;
        if (Physics.Raycast(eyes.transform.position, eyes.transform.forward * 10, out hit))
        {
            if (hit.collider.gameObject.tag.Equals("Walkable"))
            {
                seeWalkable = true;
            }
            else if (hit.collider.gameObject.tag.Equals("Deadly"))
            {
                seeObstacle = true;
                Debug.Log("That looks dangerous");
            }
            else if (hit.collider.gameObject.tag.Equals("Bot"))
            {
                seeOther = true;
                Debug.Log("I see another bot!");
                other = hit.collider.gameObject;
            }
            else if (hit.collider.gameObject.tag.Equals("Resource"))
            {
                seeResource = true;
                Debug.Log("I see a resource!");
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

        //Changed bool-gene interactions. Previously I did 2 genes for seeObstacle, but the thing is, even though it was 1 bool, 2 things were implied, seeing a walkable, or seeing an obstacle.
        //Now that we're explicity defining things, I will cover the true/false in two separate ifs, explicitly.
        if (seeWalkable)
        {
            if (dna.GetGene(0) == (int)MovementInstructions.MovementForward) move = 1;
            else if (dna.GetGene(0) == (int)MovementInstructions.MovementLeft) turn = -90;
            else if (dna.GetGene(0) == (int)MovementInstructions.MovementRight) turn = 90;
            else if (dna.GetGene(0) == (int)MovementInstructions.MovementStop) move = 0;
        }

        if (seeObstacle)
        {
            if (dna.GetGene(1) == (int)MovementInstructions.MovementForward) move = 1;
            else if (dna.GetGene(1) == (int)MovementInstructions.MovementLeft) turn = -90;
            else if (dna.GetGene(1) == (int)MovementInstructions.MovementRight) turn = 90;
            else if (dna.GetGene(1) == (int)MovementInstructions.MovementStop) move = 0;
        }

        if (seeOther)
        {
            if (dna.GetGene(2) == (int)MovementInstructions.MovementForward) move = 1;
            else if (dna.GetGene(2) == (int)MovementInstructions.MovementLeft) turn = -90;
            else if (dna.GetGene(2) == (int)MovementInstructions.MovementRight) turn = 90;
            else if (dna.GetGene(2) == (int)MovementInstructions.MovementStop) move = 0;
        }

        if (seeResource)
        {
            if (dna.GetGene(3) == (int)MovementInstructions.MovementForward) move = 1;
            else if (dna.GetGene(3) == (int)MovementInstructions.MovementLeft) turn -= 90;
            else if (dna.GetGene(3) == (int)MovementInstructions.MovementRight) turn = 90;
            else if (dna.GetGene(3) == (int)MovementInstructions.MovementStop) move = 0;
        }

        transform.Translate(0, 0, move * 0.1f);
        transform.Rotate(0, turn, 0);
    }

    /// <summary>
    /// Other objects only affect the 5th gene - e.g., imagine one gene only reacts with this catalyst
    /// </summary>
    private void RunInteractionGenes(GameObject other)
    {
        if (seeOther)
        {
            //TODO: add inner options, such as see colour of enemy and take action depending on their colour
            Body otherBody = other.GetComponent<Body>();

            if (dna.GetGene(4) == (int)InteractionInstructions.InteractionAttack)
            {
                otherBody.Damage(20);   //todo: remove hard coding
                Debug.DrawLine(transform.position, other.transform.position, Color.green);
            }
            else if (dna.GetGene(4) == (int)InteractionInstructions.InteractionGive)
            {
                otherBody.Feed(20);
                //TODO: Add benefits of feeding, e.g.:
                    //1. Increased "visual" indicator of friendliness
                    //2. Money from other
            }
            else if(dna.GetGene(4) == (int)InteractionInstructions.InteractionTrade)
            {
                //TODO: implement if have, feed other, get money
                //TODO: events? benefits / consequences simpler than communication
            }
            else if (dna.GetGene(4) == (int)InteractionInstructions.InteractionIgnore) return;
        }
    }

    private void RunResourceGenes(GameObject resource)
    {
        if (seeResource)
        {
            Resource r = resource.GetComponent<Resource>();

            if (dna.GetGene(5) == (int)ResourceInstructions.ResourceLeave)
            {
                //Do nothing
                Debug.Log("I'm on a diet");
            }
            else if (dna.GetGene(5) == (int)ResourceInstructions.ResourceTake)
            {
                //TODO: Pickup - will drop on death
                Debug.Log("I think I'll keep this for later.");
            }
            else if (dna.GetGene(5) == (int)ResourceInstructions.ResourceEat)
            {
                Debug.Log("Fuck me, that looks tasty. I'll have some of that.");
                r.Eat(20);
                body.Feed(20);
            }
            else if(dna.GetGene(5) == (int)ResourceInstructions.ResourceSpoil)
            {
                //TODO: Implement consequence
                Debug.Log("Eat this, fuckers.");
                r.Spoil();
            }
        }
    }
}

public enum MovementInstructions
{
    MovementForward = 0,
    MovementLeft,
    MovementRight,
    MovementStop,
}

public enum InteractionInstructions
{
    InteractionAttack = 0,
    InteractionGive,
    InteractionTrade,
    InteractionIgnore,
}

public enum ResourceInstructions
{
    ResourceLeave = 0,
    ResourceTake,
    ResourceEat,
    ResourceSpoil
}
