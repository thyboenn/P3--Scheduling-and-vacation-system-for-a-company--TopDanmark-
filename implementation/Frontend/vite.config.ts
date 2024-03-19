import { svelte } from "@sveltejs/vite-plugin-svelte";
import { defineConfig } from "vite";
import svelteConfig from "./svelte.config.js";

// https://vitejs.dev/config/
export default defineConfig({
  plugins: [svelte({ ...svelteConfig })],
  server: {
    proxy: {
      "/api": {
        target: "https://localhost:7158",
        secure: false,
        changeOrigin: true,
        rewrite: (path) => path.replace(/^\/api/, ""),
      },
    },
  },
});
