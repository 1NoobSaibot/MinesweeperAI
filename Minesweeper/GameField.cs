using MinesweeperLib;
using System;
using System.Windows.Forms;

namespace Minesweeper
{
	public partial class GameField : UserControl
	{
		private readonly GameCell[,] _cells;
		private const int MIN_CELL_SIZE_PX = 50;

		public readonly int MinWidth;
		public readonly int MinHeight;
		public event GameMove OnCheck;
		public event GameMove OnOpen;

		public GameField(int width, int height)
		{
			InitializeComponent();

			MinWidth = MIN_CELL_SIZE_PX * width;
			MinHeight = MIN_CELL_SIZE_PX * height;

			_cells = new GameCell[width, height];
			for (int i = 0; i < width; i++)
			{
				for (int j = 0; j < height; j++)
				{
					GameCell cell = new GameCell(i, j);
					_cells[i, j] = cell;
					cell.OnCheck += _FireOnCheck;
					cell.OnOpen += _FireOnOpen;
					this.Controls.Add(cell);
					cell.Location = new System.Drawing.Point(i * MIN_CELL_SIZE_PX, j * MIN_CELL_SIZE_PX);
				}
			}

			this.Size = new System.Drawing.Size(width * MIN_CELL_SIZE_PX, height * MIN_CELL_SIZE_PX);
		}


		public void UpdateCells(Game game)
		{
			for (int i = 0; i < game.Width; i++)
			{
				for (int j = 0; j < game.Height; j++)
				{
					_cells[i, j].SetValue(game.GetCellState(i, j));
				}
			}
		}


		private void _FireOnCheck(int posX, int posY)
		{
			OnCheck(posX, posY);
		}


		private void _FireOnOpen(int posX, int posY)
		{
			OnOpen(posX, posY);
		}
	}
}
