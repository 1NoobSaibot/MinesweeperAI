using NeuroLib;
using System;

namespace Minesweeper
{
	class Model
	{
		public readonly NeuralNetwork Network;
		public readonly int BornInGeneration;
		public int Score;


		public Model(NeuralNetwork network, int bornInGeneration)
		{
			Network = network;
			BornInGeneration = bornInGeneration;
		}

		public Model(NeuralNetwork neuralNetwork)
		{
			Network = neuralNetwork;
		}


		public override string ToString()
		{
			int[] layers = Network.GetConstructorParams();
			string layersString = String.Empty;
			for (int i = 0; i < layers.Length; i++)
			{
				layersString += layers[i].ToString() + " ";
			}
			return $"{layersString} Score = {Score}";
		}


		internal Model Clone()
		{
			Model clone = new Model(Network.Clone());
			clone.Score = Score;
			return clone;
		}
	}


	enum Decision
	{
		Open = 0,
		Check = 1,
		Skip = 2
	}
}
