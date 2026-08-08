public struct Player
{
  public string name { get; set; }
  public int id { get; set; }
  public int sing { get; set; }
}

namespace GameLogic
{
  public class TicTacToeGame
  {
    const int N = 3;
    private int[,] board = new int[N, N];
    private Player _player1;
    private Player _player2;
    private Queue<int> q1 = new Queue<int>();
    private Queue<int> q2 = new Queue<int>();
    public int currentPlayerId { get; private set; }
    public TicTacToeGame(Player p1, Player p2, int startId)
    {
      _player1 = p1;
      _player2 = p2;
      currentPlayerId = startId;
      init();
    }

    private void init() // Инициализация игровой доски
    {
      for (int i = 0; i < N; i++)
        for (int j = 0; j < N; j++)
          board[i, j] = -1;
      q1.Clear();
      q2.Clear();
    }

    private void gameExpansion() // Условие, что удаляется первый элемент, если больше трех одинаковых символов
    {
      int cell = 0;
      if (q1.Count > 3)
      {
        cell = q1.Dequeue();
        int row = cell / N;
        int col = cell % N;
        board[row, col] = -1;
      }
      if (q2.Count > 3) 
      {
        cell = q2.Dequeue();
        int row = cell / N;
        int col = cell % N;
        board[row, col] = -1;
      }
    }

    public bool makeMove(int playerId, int cell) // Совершение шага игроком. Клетки по порядку 0...8
    {
      bool move = false;
      if (playerId == currentPlayerId && cell >= 0 && cell <= 8)
      {
        int row = cell / N;
        int col = cell % N;
        int curSing = (playerId == _player1.id) ? _player1.sing : _player2.sing;
        if (board[row, col] == -1)
        { 
          board[row, col] = curSing;
          if (playerId == _player1.id)
            q1.Enqueue(cell);
          else
            q2.Enqueue(cell);
          gameExpansion();
          currentPlayerId = (playerId == _player1.id) ? _player2.id : _player1.id;
          move = true;
        }
      }
      return move;
    }

    public int isGameOver() // Проверка условия окончания игры
    {
      for (int i = 0; i < N; i++)
        if (board[i, 0] != -1 && board[i, 0] == board[i, 1] && board[i, 1] == board[i, 2])
          return board[i, 0];
      for (int i = 0; i < N; i++)
        if (board[0, i] != -1 && board[0, i] == board[1, i] && board[1, i] == board[2, i])
          return board[0, i];
      if (board[0, 0] != -1 && board[0, 0] == board[1, 1] && board[1, 1] == board[2, 2])
        return board[0, 0];
      if (board[0, 2] != -1 && board[0, 2] == board[1, 1] && board[1, 1] == board[2, 0])
        return board[0, 2];
      return -1;
    }
  }
}
