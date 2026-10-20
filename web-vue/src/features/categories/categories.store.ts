import { defineStore } from 'pinia'
import { ref } from 'vue'
import { errorMessage } from '@/api/http'
import { categoriesApi, type Category, type UpdateCategoryRequest } from './categories.api'

export const useCategoriesStore = defineStore('categories', () => {
  const items = ref<Category[]>([])
  const loading = ref(false)
  const error = ref<string | null>(null)

  /** Runs an API call with shared loading/error handling. */
  async function run(action: () => Promise<void>) {
    loading.value = true
    error.value = null
    try {
      await action()
    } catch (e) {
      error.value = errorMessage(e)
    } finally {
      loading.value = false
    }
  }

  const load = () =>
    run(async () => {
      items.value = await categoriesApi.getAll()
    })

  const add = (name: string, description?: string | null) =>
    run(async () => {
      items.value.unshift(await categoriesApi.create({ name, description }))
    })

  const update = (id: number, body: UpdateCategoryRequest) =>
    run(async () => {
      const updated = await categoriesApi.update(id, body)
      items.value = items.value.map((category) => (category.id === updated.id ? updated : category))
    })

  const remove = (id: number) =>
    run(async () => {
      await categoriesApi.remove(id)
      items.value = items.value.filter((category) => category.id !== id)
    })

  return { items, loading, error, load, add, update, remove }
})
