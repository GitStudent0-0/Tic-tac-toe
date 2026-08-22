using System.Collections.Concurrent;
using GameLogic;
using Microsoft.AspNetCore.Mvc.ViewFeatures;

namespace TicTacToeApi.Services
{
  public class GameManager
  {
    private readonly ConcurrentDictionary<string, TicTacToeGame> _games = new(); // Словать для всех отдельных партий

    public TicTacToeGame createGame(string gameId, Player p1, Player p2, int startId) // Создание игры с передачей игроков и айди начинающего игрока
    {
      var game = new TicTacToeGame(p1, p2, startId);
      _games[gameId] = game;
      return game;
    }

    public bool tryGetGame(string gameId, out TicTacToeGame game) // Получение игры
    {
      return _games.TryGetValue(gameId, out game);
    }

    public void removeGame(string gameId) // Удаление игры
    {
      _games.TryRemove(gameId, out _);
    }
  }
}
