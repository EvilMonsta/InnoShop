import { defineConfig } from 'vite'
import react from '@vitejs/plugin-react'

const USE_HTTPS = true

export default defineConfig({
  plugins: [react()],
  server: {
    port: 5173,
    proxy: {
      '/users': {
        target: USE_HTTPS ? 'https://localhost:7171' : 'http://localhost:5151',
        changeOrigin: true,
        secure: false,
        rewrite: p => p.replace(/^\/users/, ''),
      },
      '/products': {
        target: USE_HTTPS ? 'https://localhost:7172' : 'http://localhost:5152',
        changeOrigin: true,
        secure: false,
        rewrite: p => p.replace(/^\/products/, ''),
      },
    },
  },
})
