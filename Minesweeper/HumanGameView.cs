using MinesweeperLib;
using System.Threading;
using System.Windows.Forms;

namespace Minesweeper
{
	public partial class HumanGameView : Form
	{
		private Game _game;


		public HumanGameView()
		{
			InitializeComponent();
			_CreateGame();
		}


		private void _CreateGame()
		{
			_game = new Game(10, 10, 18);
			gameView1.AttachToGame(_game);
			_game.OnGameOver += _OnGameOver;
		}


		private void _OnGameOver(Game game)
		{
			if (game.IsWon)
			{
				GoodGameStorage.SaveGame(game);
			}

			Thread.Sleep(2000);
			_CreateGame();
		}
	}
}
