import { createMemoryHistory, createRouter } from 'vue-router'

import HomePage from './components/HomePage.vue'
import ManageBooks from './components/books/ManageBooks.vue'
import ManageAuthors from './components/authors/ManageAuthors.vue'
import ManageReaders from './components/readers/ManageReaders.vue'
import ManageLoans from './components/loans/ManageLoans.vue'

const routes = [
    { path: '/', component: HomePage },
    { path: '/books', component: ManageBooks },
    { path: '/authors', component: ManageAuthors },
    { path: '/readers', component: ManageReaders },
    { path: '/loans', component: ManageLoans },
]

const router = createRouter({
  history: createMemoryHistory(),
  routes,
})

export default router