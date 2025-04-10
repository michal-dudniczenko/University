<script>
    export default {
        name: "NavBar",
        methods: {
            collapseNavbar() {
                const navbarCollapse = document.getElementById('navbarSupportedContent');
                const navbarToggler = document.querySelector('.navbar-toggler');

                // Check if the navbar is expanded and then collapse it
                if (navbarCollapse.classList.contains('show')) {
                    navbarToggler.click();
                }
            }
        }
    }
</script>

<template>
    <nav id="navbar" class="navbar navbar-expand-lg navbar bg-dark border-bottom border-body fixed-top" data-bs-theme="dark">
        <div class="container-fluid">
            <RouterLink class="navbar-brand" to="/">Library</RouterLink>
            <button class="navbar-toggler" type="button" data-bs-toggle="collapse" data-bs-target="#navbarSupportedContent" aria-controls="navbarSupportedContent" aria-expanded="false" aria-label="Toggle navigation">
                <span class="navbar-toggler-icon"></span>
            </button>
            <div class="collapse navbar-collapse" id="navbarSupportedContent">
                <ul class="navbar-nav me-auto mb-2 mb-lg-0">
                    <li class="nav-item ms-3">
                        <RouterLink class="nav-link" @click="collapseNavbar" to="/books">Manage books</RouterLink>
                    </li>
                    <li class="nav-item ms-3">
                        <RouterLink class="nav-link" @click="collapseNavbar" to="/authors">Manage authors</RouterLink>
                    </li>
                    <li class="nav-item ms-3">
                        <RouterLink class="nav-link" @click="collapseNavbar" to="/readers">Manage readers</RouterLink>
                    </li>
                    <li class="nav-item ms-3">
                        <RouterLink class="nav-link" @click="collapseNavbar" to="/loans">Manage loans</RouterLink>
                    </li>
                </ul>
            </div>
        </div>
    </nav>
</template>