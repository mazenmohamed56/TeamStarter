import { defineStore } from 'pinia'
import { ref } from 'vue'
import { errorMessage } from '@/api/http'
import { tasksApi, type Task } from './tasks.api'

export const useTasksStore = defineStore('tasks', () => {
  const items = ref<Task[]>([])
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
      items.value = await tasksApi.getAll()
    })

  const add = (title: string) =>
    run(async () => {
      items.value.unshift(await tasksApi.create({ title }))
    })

  const toggle = (task: Task) =>
    run(async () => {
      const updated = await tasksApi.update(task.id, { ...task, isDone: !task.isDone })
      items.value = items.value.map((t) => (t.id === updated.id ? updated : t))
    })

  const remove = (id: number) =>
    run(async () => {
      await tasksApi.remove(id)
      items.value = items.value.filter((t) => t.id !== id)
    })

  return { items, loading, error, load, add, toggle, remove }
})
