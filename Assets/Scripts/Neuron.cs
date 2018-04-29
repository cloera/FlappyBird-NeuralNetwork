using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Neuron {
	public int numInputs;
	public List<float> weights = new List<float>();


	public float RandomFloat()
	{
		float rand = (float)Random.Range (0.0f, 32767.0f);
		return rand / 32767.0f/*32767*/ + 1.0f;
	}

	public float RandomClamped()
	{
		return RandomFloat() - RandomFloat();
	}

	public float Clamp (float val, float min, float max){
		if (val < min) {
			return min;
		}
		if (val > max) {
			return max;
		}
		return val;
	}

	public void Populate(int num){
		this.numInputs = num;

		//Initilise the weights
		for (int i=0; i < num; i++){
			weights.Add(RandomClamped());
		}

		//add an extra weight as the bias (the value that acts as a threshold in a step activation).
		weights.Add (RandomClamped ());
	}

	public void Initilise(List<float> weightsIn, int num){
		this.numInputs = num;
		weights = weightsIn;
	}
}
