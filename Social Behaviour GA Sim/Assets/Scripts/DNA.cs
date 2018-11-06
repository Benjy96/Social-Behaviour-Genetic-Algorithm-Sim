using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// DNA Length is related to the number of decisions to make - 3 bools in brain means 6 decisions
/// 
/// For each "input", or thing in the environment that the agent can interact with/detect, every gene should either react or not react, with multiple actions as possibilities.
/// For example, imagine a single gene inside the body. It can either be exposed to a chemical, or not exposed to it. If it is exposed, it might act in 100 different ways. If not, it might do nothing.
/// </summary>
public class DNA
{
    /// <summary>
    /// Genes are instructions (ways of interacting with the environment). I want these types of instructions:
    /// 1. Movement
    ///     1.1: See Obstacle - 2 possibilities (move or don't)
    ///     1.2: See Other - 2 possibilities (move or don't)
    ///     1.3: See Resource - 2 possibilities (move or don't)
    /// 2. Resource Gathering
    /// 3. Attacking
    /// </summary>
    List<int> genes = new List<int>();
    int dnaLength = 0;
    int maxValues = 0;

    //Constructor
    public DNA(int l, int v)
    {
        dnaLength = l;
        maxValues = v;
        SetGenesRandom();
    }

    public void SetGenesRandom()
    {
        genes.Clear();
        for (int i = 0; i < dnaLength; i++)
        {
            genes.Add(Random.Range(0, maxValues));
        }
    }

    public void SetInt(int pos, int value)
    {
        genes[pos] = value;
    }

    /// <summary>
    /// Take half of genes from parent 1 and parent 2 and put in offspring genes. I.e., offspring's genes are first half of parent 1 and second half of parent 2
    /// </summary>
    public void Combine(DNA d1, DNA d2)
    {
        for (int i = 0; i < dnaLength; i++)
        {
            if (i < dnaLength / 2.0)
            {
                int c = d1.genes[i];
                genes[i] = c;
            }
            else
            {
                int c = d2.genes[i];
                genes[i] = c;
            }
        }
    }

    public void Mutate()
    {
        genes[Random.Range(0, dnaLength)] = Random.Range(0, maxValues);
    }

    public int GetGene(int pos)
    {
        return genes[pos];
    }
}