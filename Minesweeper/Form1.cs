using MinesweeperLib;
using System.Windows.Forms;

namespace Minesweeper
{
	public partial class Form1 : Form
	{
		public Form1()
		{
			InitializeComponent();
			gameView1.AttachToGame(new Game(10, 10, 20));
		}
	}
}
