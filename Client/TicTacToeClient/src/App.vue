<template>
  <div class="app">
    <div class="lobby" v-if="!isPlaying"> 
      <div class="lobby__title">Крестики-нолики</div>
      <input v-model="gameId" placeholder="Введите ID комнаты">
      <input v-model="playerName" placeholder="Введите ваше имя">
      <button class="createGame" @click="() => createGame()">Создать комнату</button>
      <button class="joinGame" @click="() => joinExistingGame()">Войти в комнату</button>
      <div class="lobby__status">{{ statusMessage }}</div>
    </div>
    <div class="room" v-else>
      <div class="players-info">
        <div class="rhombus"></div>
        <div class="player" :class="{ active: currentPlayerId === 1 }">{{ player1Name }}</div>
        <span class="vs">VS</span>
        <div class="player" :class="{ active: currentPlayerId === 2 }">{{ player2Name }}</div>
        <div class="circle"></div>
      </div>
      <Board :boardData="boardData" @move="(index) => handleMove(index)" />
    </div>
  </div>
</template>

<script lang="ts">
import Board from './myComponents/Board.vue'
import { startConnection, joinGame, joinAsSecond, makeMove, onEvent } from './api/gameHub'
export default {
  name: 'App',
  components: { Board }, 
  data() {
    return {
      currentPlayerId: 0,
      statusMessage: 'Создайте комнату или войдите в существующую',
      isPlaying: false,
      boardData: Array(9).fill(-1),
      gameId: '',
      playerName: '',
      player1Name: '',
      player2Name: '', 
    }
  },
  methods: {
    registerServerEvents() {
      onEvent('WaitingForPlayer', (msg: string) => {
        this.statusMessage = msg
      }),
      onEvent('GameStarted', (yourId: number, p1Name: string, p2Name: string) => {
        this.currentPlayerId = yourId,
        this.player1Name = p1Name,
        this.player2Name = p2Name,
        this.isPlaying = true
      }),
      onEvent('BoardUpdated', (gameId: string, newBoard: number[]) => {
        this.boardData = newBoard
      }),
      onEvent('GameOver', (winner: number) => {
        alert(`Победил: ${winner === 1 ? this.player1Name : this.player2Name}`)
      }),
      onEvent('RoomNotFound', (msg: string) => alert(msg)),
      onEvent('RoomFull', (msg: string) => alert(msg)),
      onEvent('RoomExists', (msg: string) => alert(msg)),
      onEvent('Error', (msg: string) => alert(msg)),
      onEvent('OpponentDisconnected', (msg: string) => {
        alert(msg)
        this.isPlaying = false
        this.resetGame()
      })
    },
    createGame() {
      if(!this.gameId || !this.playerName)
        return alert('Введите ID игры и имя')
      joinGame(this.gameId, this.playerName)
    },
    joinExistingGame() {
      if (!this.gameId || !this.playerName) 
        return alert('Введите ID игры и имя')
      joinAsSecond(this.gameId, this.playerName)
    },
    handleMove(index: number) {
      if (this.currentPlayerId === 0) 
        return
      makeMove(this.gameId, index);
    },
    resetGame() {
      this.isPlaying = false
      this.currentPlayerId = 0
      this.boardData = Array(9).fill(-1)
      this.player1Name = ''
      this.player2Name = ''
      this.statusMessage = 'Создайте комнату или войдите в существующую'
    },
  },
  async mounted() {
    await startConnection(),
    this.registerServerEvents()
  }
}
</script>

<style scoped lang="scss">
:global(body) {
  background-color: white;
}
.app {
 display: flex;
  justify-content: center; 
  align-items: center;     
  min-height: 100vh;      
  width: 100%;
  font-family: Arial; 
}
.lobby {
  display: grid;
  align-items: center; 
  grid-template-columns: 400px; 
  gap: 20px;
  justify-content: center;
  
  &__title {
    text-align: center;
    color: black;
    font-size: 30px;
    margin-bottom: 50px;
    text-decoration: 5px dashed #ffe2e2 underline;
    text-underline-offset: 10px; 
  }

  &__status {
    color: black;
    text-align: center;
    word-wrap: break-word;
    margin-top: 10px;
    min-height: 20px;
  }
}
.createGame {
  background-color: #bad7df;
  border: none; 
  font-size: 20px;
  border-radius: 10px;
  height: 50px;
  cursor: pointer;
}
.joinGame {
  background-color: #99ddcc;
  border: none; 
  font-size: 20px;
  border-radius: 10px;
  height: 50px;
  cursor: pointer;
}
.createGame:hover {
  background-color:#ffe2e2;
  border: none;
}
.joinGame:hover {
  background-color:#ffe2e2;
  border: none;
}
input, textarea {
  background-color: rgb(255, 255, 255);
  outline: none;
  font-family: Arial, sans-serif; 
  font-size: 15px;
  border-radius: 10px;
  height: 40px;
  border: 2px solid #99ddcc;
}
.players-info {
  font-size: 30px;
  color: black;
  text-align: center;
  margin-bottom: 50px;
  display: flex;
  justify-content: center; 
  align-items: center;     
  gap: 20px;  
}
.rhombus {
  width: 15px;
  height: 15px;
  transform: rotate(45deg);
  background-color: #99ddcc;
}
.circle {
  width: 20px;
  height: 20px;
  border-radius: 50%;
  background-color: #ffe2e2;
}
</style>
