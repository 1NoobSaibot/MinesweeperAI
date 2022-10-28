using MinesweeperLib;
using System.Windows.Forms;

namespace Minesweeper
{
	public partial class GameView : UserControl
	{
		private Game _game;
		private GameField _gameField;


		public GameView()
		{
			InitializeComponent();
		}


		public void AttachToGame(Game game)
		{
			this.Controls.Remove(_gameField);
			_game = game;
			InitField(game);
		}


		private void InitField(Game game)
		{
			_gameField = new GameField(game.Width, game.Height);
			_gameField.UpdateCells(_game);
			this.Controls.Add(_gameField);
			this.Size = _gameField.Size;
			_gameField.OnCheck += _HandleOnCheck;
			_gameField.OnOpen += _HandleOnOpen;
		}


		private void _HandleOnOpen (int posX, int posY)
		{
			if (_game.Open(posX, posY))
			{
				_gameField.UpdateCells(_game);
			}
		}


		private void _HandleOnCheck (int posX, int posY)
		{
			if (_game.InvertIsChecked(posX, posY))
			{
				_gameField.UpdateCells(_game);
			}
		}
	}
}
