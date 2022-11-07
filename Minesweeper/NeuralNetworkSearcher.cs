using NeuroLib;
using System;
using System.Threading.Tasks;

namespace Minesweeper
{
	internal class NeuralNetworkSearcher : GeneticAlgorithm<Model>
	{
		private readonly GameGenerator _gameGenerator = new GameGenerator(10, 10, 18);
		private GameDto[] _games;
		private Task _looper;
		private object _lock = new object();
		private readonly ModelStorage _storage;
		private bool _isStopped = false;

		private readonly TimeSpan _saveEvery = TimeSpan.FromMinutes(5);
		private DateTime _lastSaveMoment = DateTime.Now;

		public event Action<NeuralNetworkSearcher> OnNewGeneration;


		public NeuralNetworkSearcher(int amountOfHiddenLayers) : base(100, 10) {
			_storage = new ModelStorage(amountOfHiddenLayers);
			LoadAllCandidates(amountOfHiddenLayers);
			_looper = new Task(_Loop);
			_looper.Start();
		}


		public new Model[] GetChoosen()
		{
			lock	(_lock)
			{
				return base.GetChoosen();
			}
		}


		// return negative if A is greater than B
		// return positive if B is Greater than A
		// return 0 if they are equal
		public override int Compare(Model a, Model b)
		{
			if (a.Score != b.Score)
			{
				return b.Score - a.Score;
			}

			// The older is the best
			return a.BornInGeneration - b.BornInGeneration;
		}

		public override Model Cross(Model modelA, Model modelB)
		{
			if (Rand.Next() % 2 == 0)
			{
				return Mutate(modelA);
			}
			return Mutate(modelB);
		}

		public override Model Mutate(Model model)
		{
			return model.Mutate(Rand, GenerationCounter);
		}


		public override void TestCandidate(Model model)
		{
			int score = 0;
			for (int i = 0; i < _games.Length; i++)
			{
				AIGamePlayer player = new AIGamePlayer(_games[i], model.Network);
				player.PlayToEnd();
				score += player.CalculateScore();
			}

			model.Score = score;
		}


		private void _Loop()
		{
			do
			{
				_games = _gameGenerator.GenerateMany(Rand, 100);
				lock (_lock)
				{
					NextGeneration();
					OnNewGeneration?.Invoke(this);
					
					DateTime now = DateTime.Now;
					if (now - _lastSaveMoment > _saveEvery)
					{
						_SaveModels();
						_lastSaveMoment = now;
					}
				}
			} while (!_isStopped);

			_SaveModels();
		}


		private void _SaveModels()
		{
			Model[] choosen = GetChoosen();
			NeuralNetwork[] networks = new NeuralNetwork[choosen.Length];
			for (int i = 0; i < choosen.Length; i++)
			{
				networks[i] = choosen[i].Network;
			}

			_storage.Save(networks);
		}


		private void LoadAllCandidates(int hiddenLayers)
		{
			if (_storage.Data != null && _storage.Data.Length > 0)
			{
				NeuralNetwork[] networks = _storage.GetNetworks();
				for (int i = 0; i < networks.Length; i++)
				{
					Model model = new Model(networks[i]);
					LoadCandidate(model);
				}
				return;
			}

			Model firstModel = _BuildFirstModel(hiddenLayers);
			LoadCandidate(firstModel);
			for (int i = 1; i < AmountOfChoosen; i++)
			{
				Model mutated = Mutate(firstModel);
				LoadCandidate(mutated);
			}
		}


		private Model _BuildFirstModel(int hiddenLayers)
		{
			int[] layers = new int[hiddenLayers + 2];
			layers[0] = 50;
			for (int i = 1; i < hiddenLayers + 1; i++)
			{
				layers[i] = 100;
			}
			layers[layers.Length - 1] = 3;

			NeuralNetwork net = new NeuralNetwork(layers);
			_RandomizeParams(net);
			return new Model(net, GenerationCounter);
		}


		private void _RandomizeParams(NeuralNetwork net)
		{
			NeuralNetworkRandomiser randomiser = new NeuralNetworkRandomiser(Rand);
			randomiser.Modify(net);
		}


		public void Stop()
		{
			_isStopped = true;
		}


		public void Wait()
		{
			_looper.Wait();
		}
	}


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

		internal Model Mutate(Random rand, int currentGeneration)
		{
			NeuralNetwork copy = Network.Clone();
			int layer = rand.Next(Network.AmountOfLayers - 1) + 1;
			if (rand.Next() % 2 == 0)
			{
				_ChangeRandomBias();
			}
			else
			{
				_ChangeRandomWeight();
			}

			return new Model(copy, currentGeneration);

			void _ChangeRandomBias()
			{
				float[] biases = Network.GetBiasVector(layer);
				int x = rand.Next(biases.Length);
				biases[x] = _ChangeValueRandomly(biases[x]);
			}


			void _ChangeRandomWeight()
			{
				float[,] weights = Network.GetWeightMatrix(layer);
				int x = rand.Next(weights.GetLength(0));
				int y = rand.Next(weights.GetLength(1));
				weights[x, y] = _ChangeValueRandomly(weights[x, y]);
			}

			float _ChangeValueRandomly(float originalValue)
			{
				int precision = rand.Next(7);
				int order = originalValue == 0.0
				? 0
					: (int)Math.Log10(Math.Abs(originalValue));

				float scale = (float)Math.Pow(10, order - precision);
				float difference = (float)rand.NextDouble() * 2 - 1;
				return originalValue + scale * difference;
			}
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
