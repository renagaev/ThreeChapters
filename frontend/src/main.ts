import './assets/index.css'

import {createApp} from 'vue'
import {retrieveLaunchParams} from '@telegram-apps/sdk-vue'

import App from './App.vue'
import router from './router'
import {errorHandler} from './errorHandler'
import {init} from './init'

// Mock the environment in case, we are outside Telegram.
import './mockEnv'
import {OpenAPI} from "@/client";
import {createPinia} from "pinia";

// Configure all application dependencies.
init(retrieveLaunchParams().startParam === 'debug' || import.meta.env.DEV)

OpenAPI.BASE = import.meta.env.VITE_API_BASE
OpenAPI.HEADERS = {"Authorization": retrieveLaunchParams().initDataRaw!}
const pinia = createPinia()
const app = createApp(App)
app.use(pinia)

app.config.errorHandler = errorHandler
app.use(router)
app.mount('#app')
