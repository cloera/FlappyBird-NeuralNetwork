using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public struct NNet_Settings
{
	public int numInputs;
	public int numHiddenLayers;
	public int numNeuronsPerHidden;
	public int numOutputs;
}


public class NeuralNet {
	private int inputAmount;
	private int outputAmount;

	List<float> inputs = new List<float>();
	NLayer inputlayer = new NLayer();

	List<NLayer> hiddenLayers = new List<NLayer>();
	NLayer outputLayer = new NLayer();

	List<float> outputs = new List<float> ();


	public void refresh(){
		outputs.Clear ();

		for (int i=0; i < hiddenLayers.Count; i++) {
			if(i > 0){
				inputs = outputs;
			}
			hiddenLayers[i].Evaluate(inputs, ref outputs);

		}
		inputs = outputs;
		//Process the layeroutputs through the output layer to
		outputLayer.Evaluate (inputs, ref outputs);

	}

	public void SetInput(List<float> input){
		inputs = input;
	}

	public float GetOutput(int ID){
		if (ID >= outputAmount)
			return 0.0f;
		return outputs [ID];
	}

	public int GetTotalOutputs() {
		return outputAmount;
	}

	public void CreateNet(NNet_Settings settings)
	{
		CreateNet (settings.numHiddenLayers, settings.numInputs, settings.numNeuronsPerHidden, settings.numOutputs);
	}

	public void CreateNet(int numOfHiddenLayers, int numOfInputs, int NeuronsPerHidden, int numOfOutputs){
		inputAmount = numOfInputs;
		outputAmount = numOfOutputs;

        if(numOfHiddenLayers > 0)
        {
            NLayer inputLayer = new NLayer();
            inputLayer.PopulateLayer(NeuronsPerHidden, numOfInputs);
            hiddenLayers.Add(inputLayer);
        }

        for (int i=1; i<numOfHiddenLayers; i++){
			NLayer layer = new NLayer();
			layer.PopulateLayer(NeuronsPerHidden, NeuronsPerHidden);
			hiddenLayers.Add (layer);
		}

		outputLayer = new NLayer ();
		outputLayer.PopulateLayer (numOfOutputs, NeuronsPerHidden);
	}

	public void ReleaseNet(){
		if (inputlayer != null) {
			inputlayer = null;
			inputlayer = new NLayer();
		}
		if (outputLayer != null) {
			outputLayer = null;
			outputLayer = new NLayer();
		}
		for (int i=0; i<hiddenLayers.Count; i++) {
			if(hiddenLayers[i]!=null){
				hiddenLayers[i] = null;
			}
		}
		hiddenLayers.Clear ();
		hiddenLayers = new List<NLayer> ();
	}

	public int GetNumofHIddenLayers(){
		return hiddenLayers.Count;
	}

	public Genome ToGenome(){
		Genome genome = new Genome ();

		for (int i=0; i<this.hiddenLayers.Count; i++) {
			List<float> weights = new List<float> ();
			hiddenLayers[i].GetWeights(ref weights);
			for(int j=0; j<weights.Count;j++){
				genome.weights.Add (weights[j]);
			}
		}

		List<float> outweights = new List<float> ();
		outputLayer.GetWeights(ref outweights);
		for (int i=0; i<outweights.Count; i++) {
			genome.weights.Add (outweights[i]);
		}

		return genome;
	}

	public void FromGenome(Genome genome, int numofInputs, int neuronsPerHidden, int numOfOutputs){
		ReleaseNet ();

		outputAmount = numOfOutputs;
		inputAmount = numofInputs;

		int weightsForHidden = numofInputs * neuronsPerHidden;
		NLayer hidden = new NLayer ();

		List<Neuron> neurons = new List<Neuron>();

		for(int i=0; i<neuronsPerHidden; i++){
			//init
			neurons.Add(new Neuron());
			List<float> weights = new List<float>();
			//init

			for(int j=0; j<numofInputs+1;j++){
				weights.Add(0.0f);
				weights[j] = genome.weights[i*neuronsPerHidden + j];
			}
			neurons[i].weights = new List<float>();
			neurons[i].Initilise(weights, numofInputs);
		}
		hidden.LoadLayer (neurons);
		//Debug.Log ("fromgenome, hiddenlayer neruons#: " + neurons.Count);
		//Debug.Log ("fromgenome, hiddenlayer numInput: " + neurons [0].numInputs);
		this.hiddenLayers.Add (hidden);

		//Clear weights and reasign the weights to the output
		int weightsForOutput = neuronsPerHidden * numOfOutputs;
		List<Neuron> outneurons = new List<Neuron> ();

		for(int i=0; i<numOfOutputs; i++){
			outneurons.Add(new Neuron());

			List<float> weights = new List<float>();

			for(int j=0; j<neuronsPerHidden + 1; j++){
				weights.Add (0.0f);
				weights[j] = genome.weights[i*neuronsPerHidden + j];
			}
			outneurons[i].weights = new List<float>();
			outneurons[i].Initilise(weights, neuronsPerHidden);
		}
		this.outputLayer = new NLayer ();
		this.outputLayer.LoadLayer (outneurons);
		//Debug.Log ("fromgenome, outputlayer neruons#: " + outneurons.Count);
		//Debug.Log ("fromgenome, outputlayer numInput: " + outneurons [0].numInputs);
	}
}
