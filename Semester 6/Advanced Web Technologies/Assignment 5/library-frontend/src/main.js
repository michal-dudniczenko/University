import { createApp } from 'vue'
import router from './router'
import App from './App.vue'

createApp(App)
    .use(router)
    .mount('#app')

function updateScrollPadding() {
    const navbar = document.getElementById("navbar");
    if (navbar) {
        const navbarHeight = navbar.offsetHeight;
        document.documentElement.style.scrollPaddingTop = navbarHeight - 1 + "px";
    }
}

document.addEventListener("DOMContentLoaded", updateScrollPadding);
window.addEventListener("resize", updateScrollPadding);
      
