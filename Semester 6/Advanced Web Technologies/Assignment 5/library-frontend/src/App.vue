<script>
import NavBar from './components/NavBar.vue';

export default {
    components: {
        NavBar
    }
}
</script>

<template>

    <header>
        <NavBar />
    </header>

    <main class="min-vh-100 bg-dark-subtle">
        <RouterView />
    </main>

</template>

<style scoped>
    main {
        padding-top: 100px;
        padding-bottom: 50px;
    }
</style>
