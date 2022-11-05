using System;

namespace Minesweeper
{
	internal class GameGenerator
	{
		private readonly int _width;
		private readonly int _height;
		private readonly int _bombs;

		public GameGenerator(int width, int height, int bombs)
		{
			_width = width;
			_height = height;
			_bombs = bombs;
		}


		public GameDto GenerateOne(Random rnd)
		{
			GameDto res = new GameDto();
			res.Width = _width;
			res.Height = _height;
			res.AmountOfBombs = _bombs;
			res.FirstClickX = rnd.Next(_width);
			res.FirstClickY = rnd.Next(_height);
			res.Seed = rnd.Next();

			return res;
		}


		public GameDto[] GenerateMany(Random rnd, int amount)
		{
			GameDto[] res = new GameDto[amount];

			for (int i = 0; i < amount; i++)
			{
				res[i] = GenerateOne(rnd);
			}

			return res;
		}
	}
}
