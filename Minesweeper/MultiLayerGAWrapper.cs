using System;
using System.Threading.Tasks;

namespace Minesweeper
{
	/// <summary>
	/// This class wraps few instanses of genetic algorithm containers.
	/// Every Instance works with different type (different layer amount) neural networks.
	/// </summary>
	internal class MultiLayerGAWrapper
	{
		private readonly NeuralNetworkSearcher[] _searchers;
		private readonly Task _theBestTaker;
		private Model _theBestModel;
		private bool _isStopped = false;
		public Model TheBestModel => _theBestModel;

		public MultiLayerGAWrapper(params int[] hiddenLayersAmounts)
		{
			_searchers = new NeuralNetworkSearcher[hiddenLayersAmounts.Length];
			for (int i = 0; i < hiddenLayersAmounts.Length; i++)
			{
				_searchers[i] = new NeuralNetworkSearcher(hiddenLayersAmounts[i]);
			}

			_theBestTaker = new Task(_FindTheBest);
			_theBestTaker.Start();
		}


		private void _FindTheBest()
		{
			int counter = 0;
			do
			{
				Model gotModel = _searchers[counter % _searchers.Length].GetChoosen()[0];
				counter++;

				if (_theBestModel == null || gotModel.Score > TheBestModel.Score)
				{
					_theBestModel = gotModel.Clone();
				}
			} while (!_isStopped);
		}


		public void StopAndWait()
		{
			_isStopped = true;
			for (int i = 0; i < _searchers.Length; i++)
			{
				_searchers[i].Stop();
			}

			_theBestTaker.Wait();
			for (int i = 0; i < _searchers.Length; i++)
			{
				_searchers[i].Wait();
			}
		}
	}
}
