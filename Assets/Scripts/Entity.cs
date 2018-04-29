using System.Collections;
using System.Collections.Generic;
using UnityEngine;



public class Entity : MonoBehaviour {

	//Agent testAgent;
	//private List<Agent> agents;
	public float currentAgentFitness;
	public float bestFitness;
	private float currentTimer;
	//private int checkPointsHit;
	public int populationPerGeneration;

	public NeuralNet neuralNet;

	public GeneticAlg genAlg;

	public NNet_Settings nnSettings;

	//hit hit;

	public void OnGUI(){
		int x = 300;
		int y = 400;
		GUI.Label (new Rect (x, y, 200, 20), "CurrentFitness: " + currentAgentFitness);
		GUI.Label (new Rect (x, y+20, 200, 20), "BestFitness: " + bestFitness);
		GUI.Label (new Rect (x+200, y, 200, 20), "Genome: " + genAlg.currentGenome + " of " + genAlg.totalPopulation);
		GUI.Label (new Rect (x+200, y + 20, 200, 20), "Generation: " + genAlg.generation);

	}

	// Use this for initialization
	void Start () {

		genAlg = new GeneticAlg ();


		int totalWeights = ((nnSettings.numInputs + 1) * nnSettings.numNeuronsPerHidden)
		                   + (nnSettings.numNeuronsPerHidden + 1) * nnSettings.numOutputs;
	
		genAlg.GenerateNewPopulation (populationPerGeneration, totalWeights);
		currentAgentFitness = 0.0f;
		bestFitness = 0.0f;

        currentTimer = 0.0f;

		neuralNet = new NeuralNet ();
		neuralNet.CreateNet (1, nnSettings.numInputs, nnSettings.numNeuronsPerHidden, nnSettings.numOutputs);
		Genome genome = genAlg.GetNextGenome ();
		//neuralNet.FromGenome (genome, 5, 8, 2);
		neuralNet.FromGenome (genome, nnSettings.numInputs, nnSettings.numNeuronsPerHidden, nnSettings.numOutputs);

	}

	// Update is called once per frame
	void Update ()
    {
        //Time scale for testing.
        if (Input.anyKeyDown)
        {
            Time.timeScale *= 2.0f;

            if (Time.timeScale >= 8.0f)
            {
                Time.timeScale = 1.0f;
            }
        }

        currentTimer += Time.deltaTime;
	}

	public void EntityFailed()
	{
		//if(genAlg.GetCurrentGenomeIndex() == 15-1){
		if(genAlg.GetCurrentGenomeIndex() == populationPerGeneration-1){
			EvolveGenomes();
			return;
		}
		NextTestSubject();
	}

    // Update fitness of entity
    // Based on distance traveled
    // The farther the distance the best
	public void EntityUpdate(float distance)
	{
		currentAgentFitness = distance * ((float)GameControl.instance.GetScore() + 1.0f);
		if (currentAgentFitness > bestFitness) {
			bestFitness = currentAgentFitness;
		}
	}

	public void NextTestSubject(){
		genAlg.SetGenomeFitness (currentAgentFitness, genAlg.GetCurrentGenomeIndex ());
		currentAgentFitness = 0.0f;
		Genome genome = genAlg.GetNextGenome ();

		neuralNet.FromGenome (genome, nnSettings.numInputs, nnSettings.numNeuronsPerHidden, nnSettings.numOutputs);

	}

	public void BreedNewPopulation(){
		genAlg.ClearPopulation ();
		int totalWeights = ((nnSettings.numInputs + 1) * nnSettings.numNeuronsPerHidden)
			+ (nnSettings.numNeuronsPerHidden + 1) * nnSettings.numOutputs;
		genAlg.GenerateNewPopulation (populationPerGeneration, totalWeights);
	}

	public void EvolveGenomes(){
		genAlg.BreedPopulation ();
		NextTestSubject ();
	}

	public int GetCurrentMemberOfPopulation(){
		return genAlg.GetCurrentGenomeIndex ();
	}


}
