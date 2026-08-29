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
      if (_gameManager.tryGetGame(gameId, out _))
      {
        await Clients.Caller.SendAsync("RoomExists", "Комната с таким ID уже существует");
        return;
      }
      if (string.IsNullOrEmpty(gameId) || gameId.Length > 10)
      {
        await Clients.Caller.SendAsync("Error", "Некорректный ID комнаты");
        return;
      }
      if (string.IsNullOrEmpty(playerName) || playerName.Length > 20)
      {
        await Clients.Caller.SendAsync("Error", "Некорректное имя игрока");
        return;
      }
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
      await Clients.Client(room.player1Id).SendAsync("GameStarted", 1, room.gameRoom.getPlayer1Name(), room.gameRoom.getPlayer2Name());
      await Clients.Client(room.player2Id).SendAsync("GameStarted", 2, room.gameRoom.getPlayer1Name(), room.gameRoom.getPlayer2Name());
      var board = room.gameRoom.getBoard();
      await Clients.Groups(gameId).SendAsync("BoardUpdated", gameId, board);
    }

    public async Task makeMove(string gameId, int cell) // Совершение ходов 
    {
      if (!_gameManager.tryGetGame(gameId, out RoomState room))
        return;
      int playerSign;
      if (Context.ConnectionId == room.player1Id)
        playerSign = 1;
      else if (Context.ConnectionId == room.player2Id)
        playerSign = 2;
      else
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
          string winnerName = (winner == 1) ? game.getPlayer1Name() : game.getPlayer2Name();    
          await Clients.Group(gameId).SendAsync("GameOver", winner, winnerName);
          _gameManager.removeGame(gameId);
        }
      }
    }
    public override async Task OnDisconnectedAsync(Exception? exception) // Очищает память при отключении
    {
      var roomsToRemove = _gameManager.GetRoomsByConnectionId(Context.ConnectionId);

      foreach (var gameId in roomsToRemove)
      {
        if (_gameManager.tryGetGame(gameId, out RoomState room))
        {
          if (room.player1Id == Context.ConnectionId && room.player2Id != null)
            await Clients.Client(room.player2Id).SendAsync("OpponentDisconnected", "Противник покинул игру");
          else if (room.player2Id == Context.ConnectionId && room.player1Id != null)
            await Clients.Client(room.player1Id).SendAsync("OpponentDisconnected", "Противник покинул игру");
        }
        _gameManager.removeGame(gameId);
      }
     await base.OnDisconnectedAsync(exception);
    }
  }
}
