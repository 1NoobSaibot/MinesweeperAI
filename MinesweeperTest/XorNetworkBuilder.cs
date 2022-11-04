using NeuroLib;

namespace MinesweeperTest
{
	internal static class XorNetworkBuilder
	{
		public static NeuralNetwork BuildNetwork()
		{
			NeuralNetwork xor = new NeuralNetwork(2, 3, 1);

			xor.GetWeightMatrix(1)[0, 0] = -1.5f;
			xor.GetWeightMatrix(1)[0, 1] = +1.4f;
			xor.GetWeightMatrix(1)[1, 0] = +1.4f;
			xor.GetWeightMatrix(1)[1, 1] = -1.5f;

			xor.GetWeightMatrix(2)[0, 0] = +2.5f;
			xor.GetWeightMatrix(2)[1, 0] = +2.5f;

			xor.GetBiasVector(1)[0] = -0.8f;
			xor.GetBiasVector(1)[1] = -0.8f;
			xor.GetBiasVector(2)[0] = -1.25f;

			return xor;
		}
	}
}
