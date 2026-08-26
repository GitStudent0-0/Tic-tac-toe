using System.Collections.Concurrent;
using GameLogic;
using Microsoft.AspNetCore.Mvc.ViewFeatures;

namespace TicTacToeApi.Services
{
  public class RoomState
  {
    public TicTacToeGame gameRoom { get; set; }
    public string? player1Id { get; set; }
    public string? player2Id { get; set; }
    public bool isFull => player2Id != null;
  }
  public class GameManager
  {
    private readonly ConcurrentDictionary<string, RoomState> _rooms = new(); // Словарь для всех отдельныч комнат (игр)

    public RoomState createRoom(string gameId, string playerName, string connectId) // Создание игровой комнаты с передачей игроков и айди начинающего игрока
    {
      var p1 = new Player { name = playerName, sign = 1 };
      var game = new TicTacToeGame(p1, new Player(), startId: 1);
      var room = new RoomState { gameRoom = game, player1Id = connectId };
      _rooms[gameId] = room;
      return room;
    }

    public bool tryGetGame(string gameId, out RoomState room) // Получение игры
    {
      return _rooms.TryGetValue(gameId, out room);
    }

    public void removeGame(string gameId) // Удаление игры
    {
      _rooms.TryRemove(gameId, out _);
    }
  }
}
