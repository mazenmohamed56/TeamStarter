<script setup lang="ts">
import { RouterLink, RouterView } from 'vue-router'
</script>

<template>
  <header class="topbar">
    <strong>TeamStarter</strong>
    <span class="badge">Vue</span>
    <nav>
      <RouterLink to="/tasks">Tasks</RouterLink>
      <RouterLink to="/categories">Categories</RouterLink>
      <!-- TODO (Week 3): Login / Logout links -->
    </nav>
  </header>
  <main class="container">
    <RouterView />
  </main>
</template>
