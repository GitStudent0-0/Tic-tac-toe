<template>
  <button class="cell" @click="$emit('click')">
    <div class="rhombus" v-if="value===1"></div>
    <div class="circle" v-else-if="value===2"></div>
  </button>
</template>

<script lang="ts">
export default {
  name: 'Square',
  props: {
    value: {
      type: Number,
      default: -1
    }
  },
  emits: ['click'],
}
</script>

<style scoped lang="scss">
.cell {
  width: 150px;
  height: 150px;
  cursor: pointer;
  background-color: white;
  border: none;
  display: flex;
  justify-content: center;
  align-items: center;
}
.rhombus {
  width: 90px;
  height: 90px;
  transform: rotate(45deg);
  background-color: #99ddcc;
}
.circle {
  width: 100px;
  height: 100px;
  border-radius: 50%;
  background-color: #ffe2e2;
}
</style>