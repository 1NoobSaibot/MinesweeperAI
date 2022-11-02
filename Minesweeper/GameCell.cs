using MinesweeperLib;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;

namespace Minesweeper
{
	public partial class GameCell : UserControl
	{
		private static readonly Dictionary<CellState, (string Text, Color back, Color font)> _valueToStyle;

		public readonly int PositionX;
		public readonly int PositionY;
		public event GameMove OnOpen;
		public event GameMove OnCheck;

		public GameCell(int posX, int posY)
		{
			InitializeComponent();
			PositionX = posX;
			PositionY = posY;
		}


		internal void SetValue(CellState cellState)
		{
			(string Text, Color back, Color font) = _valueToStyle[cellState];
			_button.Text = Text;
			_button.BackColor = back;
			_button.ForeColor = font;
		}


		static GameCell() {
			_valueToStyle = new Dictionary<CellState, (string Text, Color back, Color font)>();

			_valueToStyle.Add(CellState.Bomb, ("✹", Color.WhiteSmoke, Color.Orange));
			_valueToStyle.Add(CellState.ExplodedBomb, ("✹", Color.Red, Color.Black));
			_valueToStyle.Add(CellState.Checked, ("🚩", Color.WhiteSmoke, Color.Red));
			_valueToStyle.Add(CellState.CheckedRight, ("🚩", Color.WhiteSmoke, Color.Green));
			_valueToStyle.Add(CellState.CheckedFail, ("🚩", Color.WhiteSmoke, Color.Blue));
			_valueToStyle.Add(CellState.Closed, (String.Empty, Color.WhiteSmoke, Color.Black));
			_valueToStyle.Add(CellState.Zero, (String.Empty, Color.White, Color.Black));
			_valueToStyle.Add(CellState.One, ("1", Color.WhiteSmoke, Color.Blue));
			_valueToStyle.Add(CellState.Two, ("2", Color.WhiteSmoke, Color.Green));
			_valueToStyle.Add(CellState.Three, ("3", Color.WhiteSmoke, Color.Red));
			_valueToStyle.Add(CellState.Four, ("4", Color.WhiteSmoke, Color.DarkBlue));
			_valueToStyle.Add(CellState.Five, ("5", Color.WhiteSmoke, Color.Brown));
			_valueToStyle.Add(CellState.Six, ("6", Color.WhiteSmoke, Color.LightBlue));
			_valueToStyle.Add(CellState.Seven, ("7", Color.WhiteSmoke, Color.Black));
			_valueToStyle.Add(CellState.Eight, ("8", Color.WhiteSmoke, Color.White));
		}


		private void _button_Click(object sender, EventArgs e)
		{
			MouseEventArgs mouseEventArgs = (MouseEventArgs)e;
			if (mouseEventArgs.Button == MouseButtons.Left)
			{
				OnOpen(PositionX, PositionY);
			}
			else if (mouseEventArgs.Button == MouseButtons.Right)
			{
				OnCheck(PositionX, PositionY);
			}
		}
	}

	public delegate void GameMove(int posX, int posY);
}
