import { defineStore } from 'pinia'
import { ref } from 'vue'
import { errorMessage } from '@/api/http'
import { servicesApi, type CreateServiceRequest, type Service, type UpdateServiceRequest } from './services.api'

export const useServicesStore = defineStore('services', () => {
  const items = ref<Service[]>([])
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
      items.value = await servicesApi.getAll()
    })

  const add = (body: CreateServiceRequest) =>
    run(async () => {
      items.value.unshift(await servicesApi.create(body))
    })

  const update = (id: number, body: UpdateServiceRequest) =>
    run(async () => {
      const updated = await servicesApi.update(id, body)
      items.value = items.value.map((service) => (service.id === updated.id ? updated : service))
    })

  const remove = (id: number) =>
    run(async () => {
      await servicesApi.remove(id)
      items.value = items.value.filter((service) => service.id !== id)
    })

  return { items, loading, error, load, add, update, remove }
})
