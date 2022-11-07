using NeuroLib;
using Newtonsoft.Json;

namespace Minesweeper
{
	class ModelStorage : Storage<NeuralNetworkDto[]>
	{
		private const string _fileNamePrefix = "AI_5x5_";

		public ModelStorage(int forAmountOfLayer) : base(_fileNamePrefix + forAmountOfLayer + ".json")
		{ }


		public NeuralNetwork[] GetNetworks()
		{
			if (Data == null)
			{
				return new NeuralNetwork[0];
			}

			NeuralNetwork[] res = new NeuralNetwork[Data.Length];
			for (int i = 0; i < res.Length; i++)
			{
				res[i] = Data[i].BuildNetwork();
			}

			return res;
		}


		public void Save(NeuralNetwork[] networks)
		{
			NeuralNetworkDto[] dtos = new NeuralNetworkDto[networks.Length];

			for (int i = 0; i < networks.Length; i++)
			{
				dtos[i] = NeuralNetworkDto.FromNetwork(networks[i]);
			}

			Save(dtos);
		}
	}


	public class NeuralNetworkDto
	{
		/// <summary>
		/// x = Layer
		/// y = inNeurons
		/// z = outNeurons
		/// </summary>
		[JsonProperty("w")]
		public float[][][] Weights;


		/// <summary>
		/// x = (layer - 1)
		/// y = neuron
		/// </summary>
		[JsonProperty("b")]
		public float[][] Biases;


		public static NeuralNetworkDto FromNetwork(NeuralNetwork network)
		{
			int[] layers = network.GetConstructorParams();
			NeuralNetworkDto res = new NeuralNetworkDto();
			res.Biases = new float[layers.Length - 1][];
			res.Weights = new float[layers.Length - 1][][];

			for (int i = 0; i < res.Biases.Length; i++)
			{
				float[] biasesSrc = network.GetBiasVector(i + 1);
				float[] biasesDest = new float[biasesSrc.Length];

				for (int j = 0; j < biasesSrc.Length; j++)
				{
					biasesDest[j] = biasesSrc[j];
				}
				res.Biases[i] = biasesDest;
			}

			for (int i = 0; i < res.Weights.Length; i++)
			{
				float[,] weightsSrc = network.GetWeightMatrix(i + 1);
				float[][] weightsDest = new float[weightsSrc.GetLength(0)][];

				for (int inIndex = 0; inIndex < weightsDest.Length; inIndex++)
				{
					weightsDest[inIndex] = new float[weightsSrc.GetLength(1)];
					for (int outindex = 0; outindex < weightsDest[inIndex].Length; outindex++)
					{
						weightsDest[inIndex][outindex] = weightsSrc[inIndex, outindex];
					}
				}

				res.Weights[i] = weightsDest;
			}

			return res;
		}


		public NeuralNetwork BuildNetwork()
		{
			int[] layers = _ExctractConstructorParams();
			NeuralNetwork res = new NeuralNetwork(layers);

			_SetUpBiases(res);
			_SetUpWeights(res);
			return res;
		}


		private int[] _ExctractConstructorParams()
		{
			int[] layers = new int[Biases.Length + 1];
			layers[0] = Weights[0].Length;

			for (int i = 0; i < Biases.Length; i++)
			{
				layers[i + 1] = Biases[i].Length;
			}

			return layers;
		}


		private void _SetUpBiases(NeuralNetwork res)
		{
			for (int i = 0; i < Biases.Length; i++)
			{
				float[] biasesDest = res.GetBiasVector(i + 1);
				float[] biasesSrc = Biases[i];

				for (int j = 0; j < biasesSrc.Length; j++)
				{
					biasesDest[j] = biasesSrc[j];
				}
			}
		}


		private void _SetUpWeights(NeuralNetwork res)
		{
			for (int i = 0; i < Weights.Length; i++)
			{
				float[,] weightsDest = res.GetWeightMatrix(i + 1);
				float[][] weightsSrc = Weights[i];

				for (int inIndex = 0; inIndex < weightsSrc.Length; inIndex++)
				{
					for (int outindex = 0; outindex < weightsSrc[inIndex].Length; outindex++)
					{
						weightsDest[inIndex, outindex] = weightsSrc[inIndex][outindex];
					}
				}
			}
		}
	}
}
