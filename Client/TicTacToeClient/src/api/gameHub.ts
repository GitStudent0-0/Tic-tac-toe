import * as signalR from "@microsoft/signalr";
const connection = new signalR.HubConnectionBuilder()
  .withUrl("https://localhost:7257/gameHub")
  .configureLogging(signalR.LogLevel.Information)
  .build()

export const startConnection = async () => {
  try {
    await connection.start();
    console.log("Connected to SignalR Hub!");
  } catch (err) {
    console.error("Connection failed: ", err);
  }
};

export const joinGame = (gameId: string, playerName: string) => {
  return connection.invoke("joinGame", gameId, playerName)
}
export const joinAsSecond = (gameId: string, playerName: string) => {
  return connection.invoke("joinAsSecond", gameId, playerName)
}
export const makeMove = (gameId: string, cell: number) => {
  return connection.invoke("makeMove", gameId, cell)
}

export const onEvent = (eventName: string, callback: (...args: any[]) => void) => {
  connection.on(eventName, callback)
}
