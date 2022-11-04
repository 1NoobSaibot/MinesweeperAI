using Microsoft.VisualStudio.TestTools.UnitTesting;
using Minesweeper;
using NeuroLib;

namespace MinesweeperTest
{
	[TestClass]
	public class NeuralNetworkDtoTest
	{
		[TestMethod]
		public void ConvertingToDTOAndBackReturnsTheSameNetwork()
		{
			NeuralNetwork network = XorNetworkBuilder.BuildNetwork();
			NeuralNetworkDto dto = NeuralNetworkDto.FromNetwork(network);

			network = dto.BuildNetwork();
			XorNetworkTester.TestNetwork(network);
		}
	}
}
