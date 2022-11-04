using Microsoft.VisualStudio.TestTools.UnitTesting;
using NeuroLib;

namespace MinesweeperTest
{
	internal static class XorNetworkTester
	{
		public static void TestNetwork(NeuralNetwork xor)
		{
			xor.SetInputs(new float[] { 0, 0 });
			xor.Tick();
			Assert.IsTrue(xor.GetOutput(0) < 0.1);

			xor.SetInputs(new float[] { 0, 1 });
			xor.Tick();
			Assert.IsTrue(xor.GetOutput(0) > 0.9);

			xor.SetInputs(new float[] { 1, 0 });
			xor.Tick();
			Assert.IsTrue(xor.GetOutput(0) > 0.9);

			xor.SetInputs(new float[] { 1, 1 });
			xor.Tick();
			Assert.IsTrue(xor.GetOutput(0) < 0.1);
		}
	}
}
