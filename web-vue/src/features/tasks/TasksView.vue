<script setup lang="ts">
import { onMounted, ref } from 'vue'
import { useTasksStore } from './tasks.store'

const store = useTasksStore()
const newTitle = ref('')

onMounted(store.load)

async function submit() {
  const title = newTitle.value.trim()
  if (!title) return
  await store.add(title)
  if (!store.error) newTitle.value = ''
}
</script>

<template>
  <h1>Tasks</h1>

  <form class="row" @submit.prevent="submit">
    <input v-model="newTitle" placeholder="What needs to be done?" maxlength="200" />
    <button type="submit" :disabled="store.loading">Add</button>
  </form>

  <p v-if="store.error" class="error">{{ store.error }}</p>
  <p v-if="store.loading && !store.items.length" class="muted">Loading…</p>
  <p v-else-if="!store.items.length" class="muted">No tasks yet — add the first one.</p>

  <ul class="list">
    <li v-for="task in store.items" :key="task.id" :class="{ done: task.isDone }">
      <label>
        <input type="checkbox" :checked="task.isDone" @change="store.toggle(task)" />
        {{ task.title }}
      </label>
      <button class="link" @click="store.remove(task.id)">Delete</button>
    </li>
  </ul>
</template>
