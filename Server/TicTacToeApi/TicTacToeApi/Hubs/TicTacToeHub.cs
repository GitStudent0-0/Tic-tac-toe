using Microsoft.AspNetCore.SignalR;
using GameLogic;
using TicTacToeApi.Services;

namespace TicTacToeApi.Hubs
{
  public class TicTacToeHub : Hub
  {
    private readonly GameManager _gameManager;
    public TicTacToeHub(GameManager gameManager)
    {
      _gameManager = gameManager;
    }

    public async Task joinGame(string gameId, string playerName) // Создание комнаты, группы и присоединение первого игрока
    {
      var room = _gameManager.createRoom(gameId, playerName, Context.ConnectionId);
      await Groups.AddToGroupAsync(Context.ConnectionId, gameId);
      await Clients.Caller.SendAsync("WaitingForPlayer", "Ожидание второго игрока...");
    }

    public async Task joinAsSecond(string gameId, string playerName) // Присоединение второго игрока
    {
      if (!_gameManager.tryGetGame(gameId, out RoomState room))
      {
        await Clients.Caller.SendAsync("RoomNotFound", "Комната не найдена");
        return;
      }
      else if (room.isFull)
      {
        await Clients.Caller.SendAsync("RoomFull", "Комната уже заполнена");
        return;
      }
      var p2 = new Player { name = playerName, sign = 2 };
      room.gameRoom.setSecondPlayer(p2);
      room.player2Id = Context.ConnectionId;
      await Groups.AddToGroupAsync(Context.ConnectionId, gameId);
      await Clients.Client(room.player1Id).SendAsync("GameStarted", 1);
      await Clients.Client(room.player2Id).SendAsync("GameStarted", 2);
      var board = room.gameRoom.getBoard();
      await Clients.Groups(gameId).SendAsync("BoardUpdated", gameId, board);
    }

    public async Task makeMove(string gameId, int playerSign, int cell) // Совершение ходов 
    {
      if (!_gameManager.tryGetGame(gameId, out RoomState room))
        return;
      var game = room.gameRoom;
      if (game.currentPlayerId != playerSign) 
        return;
      if (game.makeMove(playerSign, cell))
      {
        await Clients.Group(gameId).SendAsync("BoardUpdated", gameId, game.getBoard());
        int winner = game.isGameOver();
        if (winner != -1)
        {
          await Clients.Group(gameId).SendAsync("GameOver", winner);
          _gameManager.removeGame(gameId);
        }
      }
    }
  }
}
