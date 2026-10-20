<script setup lang="ts">
import { onMounted, ref } from 'vue'
import { useCategoriesStore } from './categories.store'
import type { Category } from './categories.api'

const store = useCategoriesStore()
const name = ref('')
const description = ref('')
const formError = ref<string | null>(null)
const editingId = ref<number | null>(null)
const editName = ref('')
const editDescription = ref('')
const confirmDeleteId = ref<number | null>(null)

onMounted(store.load)

async function submit() {
  formError.value = null
  store.error = null
  const trimmed = name.value.trim()
  if (!trimmed) {
    formError.value = 'Name is required.'
    return
  }

  await store.add(trimmed, description.value.trim() || null)
  if (!store.error) {
    name.value = ''
    description.value = ''
  }
}

function startEdit(category: Category) {
  confirmDeleteId.value = null
  formError.value = null
  store.error = null
  editingId.value = category.id
  editName.value = category.name
  editDescription.value = category.description ?? ''
}

function cancelEdit() {
  editingId.value = null
  editName.value = ''
  editDescription.value = ''
}

async function saveEdit() {
  if (editingId.value === null) return
  formError.value = null
  store.error = null
  const trimmed = editName.value.trim()
  if (!trimmed) {
    formError.value = 'Name is required.'
    return
  }

  const id = editingId.value
  await store.update(id, {
    name: trimmed,
    description: editDescription.value.trim() || null,
  })
  if (!store.error) cancelEdit()
}

function askDelete(id: number) {
  cancelEdit()
  formError.value = null
  store.error = null
  confirmDeleteId.value = id
}

async function remove(id: number) {
  await store.remove(id)
  if (!store.error) confirmDeleteId.value = null
}
</script>

<template>
  <h1>Categories</h1>

  <form class="stack" @submit.prevent="submit">
    <div class="row">
      <input v-model="name" placeholder="Category name" maxlength="100" />
      <button type="submit" :disabled="store.loading">Add</button>
    </div>
    <input v-model="description" placeholder="Description (optional)" maxlength="500" />
  </form>

  <p v-if="formError" class="error">{{ formError }}</p>
  <p v-if="store.error" class="error">{{ store.error }}</p>
  <p v-if="store.loading && !store.items.length" class="muted">Loading…</p>
  <p v-else-if="!store.loading && !store.error && !store.items.length" class="muted">
    No categories yet — add the first one.
  </p>

  <ul class="list">
    <li v-for="category in store.items" :key="category.id" :class="{ editing: editingId === category.id }">
      <form v-if="editingId === category.id" class="edit" @submit.prevent="saveEdit">
        <input v-model="editName" maxlength="100" aria-label="Category name" />
        <input
          v-model="editDescription"
          maxlength="500"
          placeholder="Description (optional)"
          aria-label="Category description"
        />
        <div class="actions">
          <button type="submit" :disabled="store.loading">Save</button>
          <button type="button" class="ghost" @click="cancelEdit">Cancel</button>
        </div>
      </form>

      <template v-else>
        <div class="item-body">
          <strong>{{ category.name }}</strong>
          <small v-if="category.description">{{ category.description }}</small>
        </div>
        <div class="actions">
          <template v-if="confirmDeleteId === category.id">
            <span class="muted">Delete this category?</span>
            <button class="link" type="button" :disabled="store.loading" @click="remove(category.id)">
              Delete
            </button>
            <button class="ghost" type="button" @click="confirmDeleteId = null">Cancel</button>
          </template>
          <template v-else>
            <button class="ghost" type="button" @click="startEdit(category)">Edit</button>
            <button class="link" type="button" @click="askDelete(category.id)">Delete</button>
          </template>
        </div>
      </template>
    </li>
  </ul>
</template>
