<script setup lang="ts">
import { onMounted, ref } from 'vue'
import { useCategoriesStore } from '@/features/categories/categories.store'
import { useServicesStore } from './services.store'
import type { Service } from './services.api'

const store = useServicesStore()
const categories = useCategoriesStore()

const name = ref('')
const description = ref('')
const durationMinutes = ref('')
const price = ref('')
const isActive = ref(true)
const imageUrl = ref('')
const categoryId = ref<number | ''>('')

const formError = ref<string | null>(null)
const editingId = ref<number | null>(null)
const editName = ref('')
const editDescription = ref('')
const editDurationMinutes = ref('')
const editPrice = ref('')
const editIsActive = ref(true)
const editImageUrl = ref('')
const editCategoryId = ref<number | ''>('')
const confirmDeleteId = ref<number | null>(null)

onMounted(() => {
  store.load()
  categories.load()
})

function parseDuration(value: string): number | null {
  const trimmed = value.trim()
  if (!/^\d+$/.test(trimmed)) return null
  const parsed = Number(trimmed)
  return parsed >= 1 ? parsed : null
}

function parsePrice(value: string): number | null {
  const trimmed = value.trim()
  if (!/^\d+(\.\d+)?$/.test(trimmed)) return null
  const parsed = Number(trimmed)
  return parsed >= 0 ? parsed : null
}

async function submit() {
  formError.value = null
  store.error = null
  const trimmed = name.value.trim()
  if (!trimmed) {
    formError.value = 'Name is required.'
    return
  }
  if (categoryId.value === '') {
    formError.value = 'Category is required.'
    return
  }
  const duration = parseDuration(durationMinutes.value)
  if (duration === null) {
    formError.value = 'Duration must be at least 1 minute.'
    return
  }
  const amount = parsePrice(price.value)
  if (amount === null) {
    formError.value = 'Price must be zero or greater.'
    return
  }

  await store.add({
    name: trimmed,
    description: description.value.trim() || null,
    durationMinutes: duration,
    price: amount,
    isActive: isActive.value,
    imageUrl: imageUrl.value.trim() || null,
    categoryId: categoryId.value,
  })
  if (!store.error) {
    name.value = ''
    description.value = ''
    durationMinutes.value = ''
    price.value = ''
    isActive.value = true
    imageUrl.value = ''
    categoryId.value = ''
  }
}

function startEdit(service: Service) {
  confirmDeleteId.value = null
  formError.value = null
  store.error = null
  editingId.value = service.id
  editName.value = service.name
  editDescription.value = service.description ?? ''
  editDurationMinutes.value = String(service.durationMinutes)
  editPrice.value = String(service.price)
  editIsActive.value = service.isActive
  editImageUrl.value = service.imageUrl ?? ''
  editCategoryId.value = service.categoryId
}

function cancelEdit() {
  editingId.value = null
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
  if (editCategoryId.value === '') {
    formError.value = 'Category is required.'
    return
  }
  const duration = parseDuration(editDurationMinutes.value)
  if (duration === null) {
    formError.value = 'Duration must be at least 1 minute.'
    return
  }
  const amount = parsePrice(editPrice.value)
  if (amount === null) {
    formError.value = 'Price must be zero or greater.'
    return
  }

  const id = editingId.value
  await store.update(id, {
    name: trimmed,
    description: editDescription.value.trim() || null,
    durationMinutes: duration,
    price: amount,
    isActive: editIsActive.value,
    imageUrl: editImageUrl.value.trim() || null,
    categoryId: editCategoryId.value,
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
  <h1>Services</h1>

  <form class="stack" @submit.prevent="submit">
    <div class="row">
      <input v-model="name" class="field" placeholder="Service name" maxlength="100" />
      <button type="submit" :disabled="store.loading || !categories.items.length">Add</button>
    </div>
    <input v-model="description" class="field" placeholder="Description (optional)" maxlength="500" />
    <div class="row">
      <input v-model="durationMinutes" type="number" min="1" step="1" placeholder="Duration (minutes)" />
      <input v-model="price" type="number" min="0" step="0.01" placeholder="Price" />
    </div>
    <select v-model="categoryId" class="field" aria-label="Category">
      <option value="">Category</option>
      <option v-for="category in categories.items" :key="category.id" :value="category.id">
        {{ category.name }}
      </option>
    </select>
    <input v-model="imageUrl" class="field" placeholder="Image URL (optional)" maxlength="2048" />
    <label class="check">
      <input v-model="isActive" type="checkbox" />
      Active
    </label>
  </form>

  <p v-if="!categories.loading && !categories.items.length" class="muted">
    Add a category before creating a service.
  </p>
  <p v-if="formError" class="error">{{ formError }}</p>
  <p v-if="store.error" class="error">{{ store.error }}</p>
  <p v-if="categories.error" class="error">{{ categories.error }}</p>
  <p v-if="store.loading && !store.items.length" class="muted">Loading…</p>
  <p v-else-if="!store.loading && !store.error && !store.items.length" class="muted">
    No services yet — add the first one.
  </p>

  <ul class="list">
    <li v-for="service in store.items" :key="service.id" :class="{ editing: editingId === service.id }">
      <form v-if="editingId === service.id" class="edit" @submit.prevent="saveEdit">
        <input v-model="editName" maxlength="100" aria-label="Service name" />
        <input
          v-model="editDescription"
          maxlength="500"
          placeholder="Description (optional)"
          aria-label="Service description"
        />
        <div class="row">
          <input v-model="editDurationMinutes" type="number" min="1" step="1" aria-label="Duration in minutes" />
          <input v-model="editPrice" type="number" min="0" step="0.01" aria-label="Price" />
        </div>
        <select v-model="editCategoryId" aria-label="Category">
          <option v-for="category in categories.items" :key="category.id" :value="category.id">
            {{ category.name }}
          </option>
        </select>
        <input v-model="editImageUrl" maxlength="2048" placeholder="Image URL (optional)" aria-label="Image URL" />
        <label class="check">
          <input v-model="editIsActive" type="checkbox" />
          Active
        </label>
        <div class="actions">
          <button type="submit" :disabled="store.loading">Save</button>
          <button type="button" class="ghost" @click="cancelEdit">Cancel</button>
        </div>
      </form>

      <template v-else>
        <div class="item-body">
          <strong>{{ service.name }}</strong>
          <small>
            {{ service.categoryName }} · {{ service.durationMinutes }} min · {{ service.price }}
            <template v-if="!service.isActive"> · Inactive</template>
          </small>
          <small v-if="service.description">{{ service.description }}</small>
        </div>
        <div class="actions">
          <template v-if="confirmDeleteId === service.id">
            <span class="muted">Delete this service?</span>
            <button class="link" type="button" :disabled="store.loading" @click="remove(service.id)">
              Delete
            </button>
            <button class="ghost" type="button" @click="confirmDeleteId = null">Cancel</button>
          </template>
          <template v-else>
            <button class="ghost" type="button" @click="startEdit(service)">Edit</button>
            <button class="link" type="button" @click="askDelete(service.id)">Delete</button>
          </template>
        </div>
      </template>
    </li>
  </ul>
</template>
