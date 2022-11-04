﻿using NeuroLib;
using System;
using System.Windows.Forms;

namespace Minesweeper
{
	public partial class AIGameView : Form
	{
		private MultiLayerGAWrapper _searchers;
		private AIGamePlayer _gamePlayer;
		private readonly int _maxScore;


		public AIGameView()
		{
			InitializeComponent();
			gameView1.Enabled = false;

			_searchers = new MultiLayerGAWrapper(1, 2, 3);

			GameDto[] games = GoodGameStorage.GetGameDtos();
			int maxScore = 0;
			for (int i = 0; i < games.Length; i++)
			{
				maxScore += games[i].Width * games[i].Height;
			}

			this._maxScore = maxScore;
			gameLooper.Start();
		}
		

		private void gameLooper_Tick(object sender, System.EventArgs e)
		{
			if (_gamePlayer == null)
			{
				_CreatePlayer();
				return;
			}

			if (_gamePlayer.MakeOneMove())
			{
				gameView1.UpdateCells();
			}
			else
			{
				_gamePlayer = null;
			}
		}

		
		private void _CreatePlayer()
		{
			Random rand = new Random();
			Model theBestModel = _searchers.TheBestModel;
			if (theBestModel == null)
			{
				return;
			}
			NeuralNetwork network = theBestModel.Network;
			
			GameDto[] games = GoodGameStorage.GetGameDtos();
			_gamePlayer = new AIGamePlayer(games[rand.Next(games.Length)], network);
			gameView1.AttachToGame(_gamePlayer.Game);

			string title = theBestModel.ToString() + " / " + _maxScore;
			this.Text = title;
		}
	}
}
