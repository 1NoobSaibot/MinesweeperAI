using MinesweeperLib;
using NeuroLib;

namespace Minesweeper
{
	internal class AIGamePlayer
	{
		private readonly Game _game;
		private readonly NeuralNetwork _network;
		private readonly int _amountOfCells;
		private int _currentCellXY = 0;

		public Game Game => _game;

		public AIGamePlayer(GameDto game, NeuralNetwork network)
		{
			this._game = game.MakeGame();
			_network = network;
			_amountOfCells = game.Width * game.Height;
		}


		/// <summary>
		/// Makes all moves until the game ends.
		/// </summary>
		public void PlayToEnd()
		{
			do { }
			while (MakeOneMove());
		}


		/// <summary>
		/// Makes only one move and returns control.
		/// </summary>
		public bool MakeOneMove()
		{
			if (_game.IsGameOver)
			{
				return false;
			}

			for (int i = 0; i < _amountOfCells; i++)
			{
				(int x, int y) = _GetCurrentCellCoordinates(_currentCellXY + i);
				CellState cell = _game.GetCellState(x, y);

				if (cell != CellState.Closed)
				{
					continue;
				}

				_SetInputs(x, y);
				_network.Tick();
				Decision decision = _ReadDecision();

				if (decision == Decision.Skip)
				{
					continue;
				}

				if (decision == Decision.Open)
				{
					_game.Open(x, y);
				} 
				else
				{
					_game.InvertIsChecked(x, y);
				}

				_currentCellXY += i + 1;
				return true;
			}

			_game.GiveUp();
			return false;
		}


		public int CalculateScore()
		{
			if (_game.IsWon)
			{
				return _game.Width * _game.Height;
			}

			_game.GiveUp();

			int score = 0;
			for (int i = 0; i < _game.Width; i++)
			{
				for (int j = 0; j < _game.Height; j++)
				{
					CellState cell = _game.GetCellState(i, j);
					if (
						((int)cell >= 0 && (int)cell <= 8)
						|| cell == CellState.CheckedRight
					)
					{
						score++;
					}
					else if (cell == CellState.CheckedFail)
					{
						score--;
					}
				}
			}

			return score;
		}

		private (int, int) _GetCurrentCellCoordinates(int xy)
		{
			xy = xy % _amountOfCells;
			int x = xy % _game.Height;
			int y = xy / _game.Height;

			return (x, y);
		}

		private void _SetInputs(int x, int y)
		{
			int inputPointer = 0;
			for (int by = -2; by <= 2; by++)
			{
				for (int bx = -2; bx <= 2; bx++)
				{
					(int mainState, int additionalValue) = _GetCell(x + bx, y + by);
					_network.SetInput(inputPointer, mainState);
					_network.SetInput(inputPointer + 1, additionalValue);
					inputPointer += 2;
				}
			}
		}

		private (int, int) _GetCell(int x, int y)
		{
			if (x < 0 || y < 0 || x >= _game.Width || y >= _game.Height)
			{
				return (-1, 0);
			}

			CellState cell = _game.GetCellState(x, y);
			switch (cell)
			{
				case CellState.Closed: return (0, 0);
				case CellState.Checked: return (0, 1);
				default: return (1, (int)cell);
			}
		}


		private Decision _ReadDecision()
		{
			float maxValue = _network.GetOutput(0);
			int maxIndex = 0;

			for (int i = 1; i < 3; i++)
			{
				if (_network.GetOutput(i) > maxValue)
				{
					maxIndex = i;
					maxValue = _network.GetOutput(i);
				}
			}

			return (Decision)maxIndex;
		}
	}
}
