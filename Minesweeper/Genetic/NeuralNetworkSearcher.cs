using NeuroLib;
using NeuroLib.RegularNeuralNetwork.Evolution;
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

		private Modifier<NeuralNetwork> _mutator;


		public NeuralNetworkSearcher(int amountOfHiddenLayers) : base(100, 10) {
			_storage = new ModelStorage(amountOfHiddenLayers);
			LoadAllCandidates(amountOfHiddenLayers);
			_looper = new Task(_Loop);

			_mutator = new ModificationQueue<NeuralNetwork>(
				new NeuralNetworkCloner(),
				new RandomModification<NeuralNetwork>(
					new ModificationWeight<NeuralNetwork>[]
					{
						new ModificationWeight<NeuralNetwork>(new RandomNeuronInserter(Rand), 1),
						new ModificationWeight<NeuralNetwork>(new RandomNeuronRemover(Rand), 3),
						new ModificationWeight<NeuralNetwork>(new RandomParamChanger(Rand), 1000)
					},
					Rand
				)
			);

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
			NeuralNetwork mutatedClone = _mutator.Modify(model.Network);
			return new Model(mutatedClone, GenerationCounter);
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
}
