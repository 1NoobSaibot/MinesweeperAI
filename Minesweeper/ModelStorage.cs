using MinesweeperLib;
using NeuroLib;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.IO;

namespace Minesweeper
{
	internal static class ModelStorage
	{
		private const string fileName = "AI.json";
		private static List<Game> _games;


		static ModelStorage ()
		{
			_Load();
		}


		public static void SaveGame(Game game)
		{
			if (_TryToInsertGame(game))
			{
				_RewriteFile();
			}
		}


		private static bool _TryToInsertGame(Game game)
		{
			for (int i = 0; i < _games.Count; i++)
			{
				if (game.Equals(_games[i]))
				{
					return false;
				}
			}

			_games.Add(game);
			return true;
		}


		private static void _RewriteFile()
		{
			using (StreamWriter writer = new StreamWriter(fileName, false))
			{
				GameDto[] games = new GameDto[_games.Count];
				for (int i = 0; i < games.Length; i++)
				{
					games[i] = GameDto.FromGame(_games[i]);
				}
				string json = JsonConvert.SerializeObject(games);
				writer.Write(json);
			}
		}


		private static void _Load()
		{
			if (!File.Exists(fileName))
			{
				_games = new List<Game>();
				return;
			}

			using (StreamReader reader = new StreamReader(fileName))
			{
				string json = reader.ReadToEnd();
				GameDto[] games = JsonConvert.DeserializeObject<GameDto[]>(json);
				_games = new List<Game>(games.Length);
				for (int i = 0; i < games.Length; i++)
				{
					_games.Add(games[i].MakeGame());
				}
			}
		}
	}


	class NeuralNetworkDto
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
		
	}
}
