import { configDefaults, defineConfig } from "vitest/config";
import react from "@vitejs/plugin-react";

// The backend runs on http://localhost:5000 (api/Connect/Properties/launchSettings.json).
// Every call to /api goes through this proxy, so no component knows the server address
// and the browser sees one origin, which means no CORS setup is needed.
export default defineConfig({
  plugins: [react()],
  server: {
    proxy: {
      "/api": "http://localhost:5000",
    },
  },
  test: {
    environment: "jsdom",
    setupFiles: ["./src/test/setup.ts"],
    // web/e2e holds the Playwright tests, run with npm run test:e2e, not by Vitest (#265).
    exclude: [...configDefaults.exclude, "e2e/**"],
  },
});
