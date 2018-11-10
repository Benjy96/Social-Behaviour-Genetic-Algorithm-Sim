using System.Linq;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// By following simple, local rules, we reach a solution - emergent behaviour
/// In this program, each ai will be given a gene. i.e., an instruction. The instructions leading to success will be carried on to the next generation.
/// It is essentially a trial & error method of finding success.
/// 
/// It seems to me the skill of using genetic algorithms as problem solving comes from determining a good number of genes & what the genes do (instructions) AND how you determine fitness
/// 
/// TO DO: Measure fitness by "health management" -  health will decrease over time, meaning bots that walk into walls endlessly, that dont' avoid dangers, etc., will be rated unfit.
/// </summary>
public class PopulationManager : MonoBehaviour
{
    public static float elapsed = 0;
    public float trialTime = 100;

    public GameObject botPrefab;
    public int populationSize = 50;
    public int botSpawnOffsetRange = 10;

    public GameObject resourcePrefab;
    public int numResources = 10;
    
    List<GameObject> population = new List<GameObject>();
    List<GameObject> resources = new List<GameObject>();

    int generation = 1;

    GUIStyle guiStyle = new GUIStyle();

    private void OnGUI()
    {
        guiStyle.fontSize = 25;
        guiStyle.normal.textColor = Color.white;
        GUI.BeginGroup(new Rect(10, 10, 250, 150));
        GUI.Box(new Rect(0, 0, 140, 140), "Stats", guiStyle);
        GUI.Label(new Rect(10, 25, 200, 30), "Gen: " + generation, guiStyle);
        GUI.Label(new Rect(10, 50, 200, 30), string.Format("Time: {0:0.00}", elapsed, guiStyle));
        GUI.Label(new Rect(10, 75, 200, 30), "Population: " + population.Count, guiStyle);
        GUI.EndGroup();
    }

    // Use this for initialization
    void Start()
    {
        for (int i = 0; i < populationSize; i++)
        {
            Vector3 startPos = new Vector3(transform.position.x + Random.Range(-botSpawnOffsetRange, botSpawnOffsetRange),
                                transform.position.y,
                                transform.position.z + Random.Range(-botSpawnOffsetRange, botSpawnOffsetRange));

            GameObject b = Instantiate(botPrefab, startPos, transform.rotation);
            b.GetComponent<Brain>().Init();
            population.Add(b);
        }
        PlaceResources();
    }

    GameObject Breed(GameObject parent1, GameObject parent2)
    {
        Vector3 startPos = new Vector3(transform.position.x + Random.Range(-botSpawnOffsetRange, botSpawnOffsetRange), transform.position.y, transform.position.z + Random.Range(-botSpawnOffsetRange, botSpawnOffsetRange));

        //Create a child
        GameObject offspring = Instantiate(botPrefab, startPos, transform.rotation);
        Brain b = offspring.GetComponent<Brain>();

        //Mutate 1 in 100, else initialize from combined parent dna
        if (Random.Range(0, 100) == 1)
        {
            b.Init();
            b.dna.Mutate();
        }
        else
        {
            b.Init();
            b.dna.Combine(parent1.GetComponent<Brain>().dna, parent2.GetComponent<Brain>().dna);
        }
        return offspring;
    }

    public void BreedNewPopulation()
    {
        //Longest living go to end of list
        List<GameObject> sortedList = population.OrderBy(o => o.GetComponent<Brain>().GetHealth()).ToList();

        //Longest travelled to end of the list - fittest
        //Consider fitness from two factors: time walking and time alive
        //List<GameObject> sortedList = population.OrderBy(o => (o.GetComponent<Brain>().timeWalking + o.GetComponent<Brain>().timeAlive)).ToList();

        population.Clear();

        //Select the fittest to breed (last half of array - 50% selected)
        for (int i = (int)(sortedList.Count / 2.0f) - 1; i < sortedList.Count - 1; i++)
        {
            //Half of each parent's dna will go to each child
            population.Add(Breed(sortedList[i], sortedList[i + 1]));
            population.Add(Breed(sortedList[i + 1], sortedList[i]));
        }

        for (int i = 0; i < sortedList.Count; i++)
        {
            Destroy(sortedList[i]);
        }

        generation++;
    }

    private void PlaceResources()
    {
        for (int i = 0; i < resources.Count; i++)
        {
            Destroy(resources[i]);
        }
        resources.Clear();

        //TODO: dynamically adjust to population size / make population size not limited to 50
        for (int i = 0; i < numResources; i++)
        {
            Vector3 startPos = new Vector3(transform.position.x + Random.Range(-60, 60), transform.position.y, transform.position.z + Random.Range(-60, 60));
            resources.Add(Instantiate(resourcePrefab, startPos, Quaternion.identity));
        }
    }

    // Update is called once per frame
    void Update()
    {
        elapsed += Time.deltaTime;
        if (elapsed >= trialTime)
        {
            BreedNewPopulation();
            PlaceResources();
            elapsed = 0;
        }
    }
}
