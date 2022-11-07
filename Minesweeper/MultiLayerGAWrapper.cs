namespace Minesweeper
{
	/// <summary>
	/// This class wraps few instanses of genetic algorithm containers.
	/// Every Instance works with different type (different layer amount) neural networks.
	/// </summary>
	internal class MultiLayerGAWrapper
	{
		private readonly NeuralNetworkSearcher[] _searchers;
		private Model _theBestModel;
		private object _theBestLocker = new object();


		public Model TheBestModel {
			get {
				lock (_theBestLocker)
				{
					return _theBestModel;
				}
			}
		}


		public MultiLayerGAWrapper(params int[] hiddenLayersAmounts)
		{
			_searchers = new NeuralNetworkSearcher[hiddenLayersAmounts.Length];
			for (int i = 0; i < hiddenLayersAmounts.Length; i++)
			{
				_searchers[i] = new NeuralNetworkSearcher(hiddenLayersAmounts[i]);
				_searchers[i].OnNewGeneration += _UpdateTheBest;
			}
		}


		private void _UpdateTheBest(NeuralNetworkSearcher searcher)
		{
			Model gotModel = searcher.GetChoosen()[0];
			lock (_theBestLocker)
			{
				if (_theBestModel == null || gotModel.Score > TheBestModel.Score)
				{
					_theBestModel = gotModel.Clone();
				}
			}
		}


		public void StopAndWait()
		{
			for (int i = 0; i < _searchers.Length; i++)
			{
				_searchers[i].Stop();
			}

			for (int i = 0; i < _searchers.Length; i++)
			{
				_searchers[i].Wait();
			}
		}
	}
}
