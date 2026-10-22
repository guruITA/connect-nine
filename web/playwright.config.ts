import { defineConfig, devices } from "@playwright/test";

/** True on GitLab CI, where the job e2e-test of #267 runs these tests. */
const onCi = Boolean(process.env.CI);

/**
 * Playwright settings for the browser tests in web/e2e (#265).
 * The version of @playwright/test is fixed, so the job e2e-test of #267 can use the
 * Playwright image of the same version.
 */
export default defineConfig({
  // Where the tests are.
  testDir: "./e2e",
  // On CI a forgotten test.only fails the run instead of skipping the other tests.
  forbidOnly: onCi,
  // A list in the terminal, and on CI also an HTML report in playwright-report.
  reporter: onCi ? [["list"], ["html", { open: "never" }]] : "list",
  use: {
    // npm run dev on a laptop, or another address such as the test site through PLAYWRIGHT_BASE_URL.
    baseURL: process.env.PLAYWRIGHT_BASE_URL || "http://localhost:5173",
    // Keep a trace only of a test that failed.
    trace: "retain-on-failure",
  },
  // Only Chromium for now.
  projects: [{ name: "chromium", use: { ...devices["Desktop Chrome"] } }],
});
