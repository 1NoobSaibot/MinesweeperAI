using System;

namespace MinesweeperLib
{
	internal class MinesweeperGameException : Exception
	{
		public MinesweeperGameException(string message) : base(message) { }
	}
}
