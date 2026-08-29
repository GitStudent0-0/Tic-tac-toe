<template>
  <div class="board">
    <Square v-for="(cell, index) in boardData" :key="index" :value="cell"
      @click="() => handleSquareClick(index)" ></Square>
  </div>
</template>

<script lang="ts">
import Square from './Square.vue';
export default {
  name: 'Board',
  components: { Square },
  props: {
    boardData: {
      type: Array,
      default: () => Array(9).fill(-1)
    }
  },
  emits: ['move'],
  methods: {
    handleSquareClick(index: Number) {
      this.$emit('move', index)
    }
  }
}

</script>

<style scoped lang="scss">
.board {
  display: grid;
  grid-template-columns: repeat(3, 1fr);
  grid-template-rows: repeat(3, 1fr);
  background-color: #bad7df;
  gap: 10px;
}
</style>