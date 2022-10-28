using System;

namespace MinesweeperLib
{
  public class Game
  {
    public readonly int AmountOfBombs;
    private readonly Cell[,] _cells;
    private GameState _state = GameState.Created;
    public readonly int Seed;
    private int _firstClickX, _firstClickY;

    public int Width => _cells.GetLength(0);
    public int Height => _cells.GetLength(1);
    public int FirstClickX => _firstClickX;
    public int FirstClickY => _firstClickY;
    public bool IsWon => _state == GameState.Won;
    public event GameEvent OnGameOver;


    public Game(int width, int height, int bombs, int seed)
    {
      if (width * height <= bombs)
      {
        throw new MinesweeperGameException($"Cannot place {bombs} bombs on a field {width}*{height}");
      }

      AmountOfBombs = bombs;
      _cells = new Cell[width, height];
      Seed = seed;
    }


		public Game(int width, int height, int bombs)
      : this(width, height, bombs, new Random().Next())
		{ }


		public CellState GetCellState(int x, int y)
    {
      return _cells[x, y].ToCellState(_state == GameState.Lost);
    }


    public bool Open(int posX, int posY)
    {
			if (_state == GameState.Created)
			{
				_Generate(posX, posY);
			}
      if (_state != GameState.Generated)
      {
        return false;
      }

      if (_TryOpen(posX, posY))
      {
        _CheckWin();
        return true;
      }
      return false;
		}


    public bool InvertIsChecked(int posX, int posY)
    {
			if (_state != GameState.Generated)
			{
        return false;
			}

      return _cells[posX, posY].InvertIsChecked();
		}


    public override bool Equals(object obj)
    {
      if (obj is Game game)
      {
        if (
          game.Width != Width
          || game.Height != Height
          || game.AmountOfBombs != AmountOfBombs
          || game.FirstClickX != FirstClickX
          || game.FirstClickY != FirstClickY
        )
        {
          return false;
        }


        // The situation when seeds are different but generated fields are the same is possible.
        if (game.Seed == Seed)
        {
          return true;
        }

        for (int x = 0; x < Width; x++)
        {
          for (int y = 0; y < Height; y++)
          {
            if (game._cells[x, y].IsMined != _cells[x, y].IsMined)
            {
              return false;
            }
          }
        }

        return true;
      }

      return false;
    }


    private void _Generate(int posX, int posY)
    {
      _firstClickX = posX;
      _firstClickY = posY;
      _PlaceBombsExcept(posX, posY);
      _CountAmountsOfBombsForAllCells();
      _state = GameState.Generated;
    }


    private void _PlaceBombsExcept(int posX, int posY)
    {
			Random random = new Random(Seed);
			for (int i = 0; i < AmountOfBombs; i++)
			{
				int x, y;
				do
				{
					x = random.Next(Width);
					y = random.Next(Height);
				} while (_cells[x, y].IsMined || !isEnoughFar(x, y));

				_cells[x, y].IsMined = true;
			}


      bool isEnoughFar(int x, int y)
      {
        return (Math.Abs(posX - x) + Math.Abs(posY - y)) > 2;
      }
		}


    private void _CountAmountsOfBombsForAllCells()
    {
			for (int i = 0; i < Width; i++)
			{
				for (int j = 0; j < Height; j++)
				{
          if (_cells[i, j].IsMined)
          {
            continue;
          }

					_cells[i, j].AmountOfBombsAround = _CountBombsAround(i, j);
				}
			}
		}


    private byte _CountBombsAround(int i, int j)
    {
      int[,] biases = new int[8, 2]
      {
        { -1, -1 },
        { -1,  0 },
        { -1,  1 },
        {  0,  1 },
        {  1,  1 },
        {  1,  0 },
        {  1, -1 },
        {  0, -1 }
      };


      byte sum = 0;
      for (int k = 0; k < 8; k++)
      {
        int x = i + biases[k, 0];
        int y = j + biases[k, 1];

        if (x < 0 || x >= Width || y < 0 || y >= Height)
        {
          continue;
        }

        if (_cells[x, y].IsMined)
        {
          sum++;
        }
			}

      return sum;
    }


    private bool _TryOpen(int x, int y)
    {
      int res = _cells[x, y].Open();

      if (res == 0)
      {
        return false;
      }

      if (res == -1)
      {
        _SetGameLost();
      }

      return true;
    }


    private void _CheckWin()
    {
      for (int i = 0; i < Width; i++)
      {
        for (int j = 0; j < Height; j++)
        {
          if (_cells[i, j].IsComplete == false)
          {
            return;
          }
        }
      }
      
      _SetGameWon();
    }


    private void _SetGameLost()
    {
			_state = GameState.Lost;
      OnGameOver?.Invoke(this);
		}


    private void _SetGameWon()
    {
      _state = GameState.Won;
      OnGameOver?.Invoke(this);
    }
  }


  public enum GameState
  {
    Created,
    Generated,
    Won,
    Lost
  }


  internal struct Cell
  {
    public bool IsMined;
    public byte AmountOfBombsAround;
    public bool IsOpen { get; private set; }
    public bool IsChecked { get; private set; }
    public bool IsComplete => IsMined ^ IsOpen;

    public bool InvertIsChecked()
    {
      if (IsOpen)
      {
        return false;
      }

      IsChecked = !IsChecked;
      return true;
    }


    public int Open()
    {
      if (IsOpen || IsChecked)
      {
        return 0;
      }

      IsOpen = true;
      return IsMined ? -1 : 1;
    }

    internal CellState ToCellState(bool showBombs = false)
    {
      if (IsChecked)
      {
        return CellState.Checked;
      }

      if (showBombs && IsMined)
      {
        return CellState.Bomb;
      }

      if (!IsOpen)
      {
        return CellState.Closed;
      }

      return (CellState)AmountOfBombsAround;
    }


    public override string ToString()
    {
      return ToCellState().ToString()
        + (IsMined ? " Mined" : String.Empty)
        + (AmountOfBombsAround > 0 ? AmountOfBombsAround.ToString() : String.Empty);
    }
  }


  public enum CellState
  {
    Bomb = -3,
    Checked = -2,
    Closed = -1,
    Zero = 0,
    One = 1,
    Two = 2,
    Three = 3,
    Four = 4,
    Five = 5,
    Six = 6,
    Seven = 7,
    Eight = 8
  }


  public delegate void GameEvent(Game game);
}
