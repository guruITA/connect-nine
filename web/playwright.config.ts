import { defineConfig, devices } from "@playwright/test";

// The browser tests of web/e2e (#265). On a laptop they run against npm run dev. Any other
// address, such as the test site, goes in PLAYWRIGHT_BASE_URL, which the job e2e-test of #267 sets.
// The version of @playwright/test is fixed, so that job can use the Playwright image of the same version.
const onCi = Boolean(process.env.CI);

export default defineConfig({
  testDir: "./e2e",
  forbidOnly: onCi,
  reporter: onCi ? [["list"], ["html", { open: "never" }]] : "list",
  use: {
    baseURL: process.env.PLAYWRIGHT_BASE_URL || "http://localhost:5173",
    trace: "retain-on-failure",
  },
  projects: [{ name: "chromium", use: { ...devices["Desktop Chrome"] } }],
});
