using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NLayer {

	private int totalNeurons;
	private int totalInputs;


	List<Neuron> neurons = new List<Neuron>();

	public float Sigmoid(float a, float p) {
		float ap = (-a) / p;
		return (1 / (1 + Mathf.Exp (ap)));
	}

	public float BiPolarSigmoid(float a, float p){
		float ap = (-a) / p;
		return (2 / (1 + Mathf.Exp (ap)) - 1);
	}

	public void Evaluate(List<float> input, ref List<float> output){
		int inputIndex = 0;
		//Debug.Log ("input.count " + input.Count);
		//Debug.Log ("totalneuron " + totalNeurons);
		//cycle over all the neurons and sum their weights against the inputs
		for (int i=0; i< totalNeurons; i++) {
			float activation = 0.0f;

			//Debug.Log ("numInputs " + (neurons[i].numInputs - 1));

			//sum the weights to the activation value
			//we do the sizeof the weights - 1 so that we can add in the bias to the activation afterwards.
			for(int j=0; j< neurons[i].numInputs - 1; j++){

				activation += input[inputIndex] * neurons[i].weights[j];
				inputIndex++;
			}

			//add the bias
			//the bias will act as a threshold value to
			activation += neurons[i].weights[neurons[i].numInputs] * (-1.0f);//BIAS == -1.0f

			output.Add(Sigmoid(activation, 1.0f));
			inputIndex = 0;
		}
	}

	public void LoadLayer(List<Neuron> input){
		totalNeurons = input.Count;
		neurons = input;
	}

	public void PopulateLayer(int numOfNeurons, int numOfInputs){
		totalInputs = numOfInputs;
		totalNeurons = numOfNeurons;

		if (neurons.Count < numOfNeurons) {
			for(int i=0; i<numOfNeurons; i++){
				neurons.Add(new Neuron());
			}
		}

		for(int i=0; i<numOfNeurons; i++){
			neurons[i].Populate(numOfInputs);
		}
	}

	public void SetWeights(List<float> weights, int numOfNeurons, int numOfInputs){
		int index = 0;
		totalInputs = numOfInputs;
		totalNeurons = numOfNeurons;

		if (neurons.Count < numOfNeurons) {
			for (int i=0; i<numOfNeurons - neurons.Count; i++){
				neurons.Add(new Neuron());
			}
		}
		//Copy the weights into the neurons.
		for (int i=0; i<numOfNeurons; i++) {
			if(neurons[i].weights.Count < numOfInputs){
				for(int k=0; k<numOfInputs-neurons[i].weights.Count; k++){
					neurons[i].weights.Add (0.0f);
				}
			}
			for(int j=0; j<numOfInputs; j++){
				neurons[i].weights[j] = weights[index];
				index++;
			}
		}
	}

	public void GetWeights(ref List<float> output){
		//Calculate the size of the output list by calculating the amount of weights in each neurons.
		output.Clear ();

		for (int i=0; i<this.totalNeurons; i++) {
			for(int j=0; j<neurons[i].weights.Count; j++){
				output[totalNeurons*i + j] = neurons[i].weights[j];
			}
		}
	}

	public void SetNeurons(List<Neuron> neurons, int numOfNeurons, int numOfInputs){
		totalInputs = numOfInputs;
		totalNeurons = numOfNeurons;
		this.neurons = neurons;
	}
}
