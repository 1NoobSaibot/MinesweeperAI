using MinesweeperLib;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;

namespace Minesweeper
{
	internal static class GoodGameStorage
	{
		private const string fileName = "GamesToTeachAI.json";
		private static List<Game> _games;


		static GoodGameStorage ()
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


		public static GameDto[] GetGameDtos()
		{
			GameDto[] dtos = new GameDto[_games.Count];
			for (int i = 0; i < dtos.Length; i++)
			{
				dtos[i] = GameDto.FromGame(_games[i]);
			}
			return dtos;
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
				GameDto[] gameDtos = GetGameDtos();
				string json = JsonConvert.SerializeObject(gameDtos);
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


	class GameDto
	{
		[JsonProperty("w")]
		public int Width;
		[JsonProperty("h")]
		public int Height;
		[JsonProperty("a")]
		public int AmountOfBombs;
		[JsonProperty("s")]
		public int Seed;
		[JsonProperty("x")]
		public int FirstClickX;
		[JsonProperty("y")]
		public int FirstClickY;


		public Game MakeGame()
		{
			Game game = new Game(Width, Height, AmountOfBombs, Seed);
			game.Open(FirstClickX, FirstClickY);
			return game;
		}


		public static GameDto FromGame(Game game)
		{
			GameDto res = new GameDto();
			res.Width = game.Width;
			res.Height = game.Height;
			res.AmountOfBombs = game.AmountOfBombs;
			res.Seed = game.Seed;
			res.FirstClickX = game.FirstClickX;
			res.FirstClickY = game.FirstClickY;
			return res;
		}
	}
}
