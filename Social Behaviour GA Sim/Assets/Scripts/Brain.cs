using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// The entities that have the genes/instructions for turning when they see the edge will be the ones to survive.
/// This would produce emergent behaviour if obstacles were moving.
/// Essentially an obstacle avoidance script.
/// </summary>
public class Brain : MonoBehaviour
{
    public GameObject botPrefab;
    public GameObject eyes;

    public DNA dna;
    
    public int DNALength = 2;   //dna length 2 because we have 2 decisions to make
    public float timeAlive;
    public float timeWalking;

    bool alive = true;
    bool seeGround = true;

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

        //Debug.DrawRay(eyes.transform.position, eyes.transform.forward * 10, Color.red, 10);
        seeGround = false;

        RaycastHit hit;
        if (Physics.Raycast(eyes.transform.position, eyes.transform.forward * 10, out hit))
        {
            if (hit.collider.gameObject.tag == "platform")
            {
                seeGround = true;
            }
        }
        timeAlive = PopulationManager.elapsed;

        float turn = 0;
        float move = 0;

        if (seeGround)
        {
            if (dna.GetGene(0) == 0)
            {
                move = 1; //forward
                timeWalking += Time.deltaTime;
            }
            else if (dna.GetGene(0) == 1) turn = -90;  //turn left
            else if (dna.GetGene(0) == 2) turn = 90;   //turn right
        }
        else
        {
            if (dna.GetGene(1) == 0)
            {
                move = 1;
                timeWalking += Time.deltaTime;
            }
            else if (dna.GetGene(1) == 1) turn = -90;
            else if (dna.GetGene(1) == 2) turn = 90;
        }

        transform.Translate(0, 0, move * 0.1f);
        transform.Rotate(0, turn, 0);
    }
}
