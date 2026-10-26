import { expect, test } from "@playwright/test";

// This file runs against production after every deploy (job smoke-production, #1059). It must stay read
// only: no form is sent, nobody logs in and no account is made, because production may hold real data.
// It only checks what every version of the home page has.

/**
 * The home page of the live version opens and React draws the screen (#1059).
 *
 * @param page - The browser page Playwright opens for this test.
 * @returns Nothing. The test fails when the page does not load or the root element stays empty.
 */
test("the home page opens and the screen is drawn", async ({ page }) => {
  // The server answers the home page.
  const response = await page.goto("/");
  expect(response?.ok()).toBe(true);

  // React drew the screen inside the element with the id root of web/index.html.
  await expect(page.locator("#root")).not.toBeEmpty();
});

/**
 * The health address of the live version says healthy (#1059).
 *
 * @param page - The browser page Playwright opens for this test.
 * @returns Nothing. The test fails when the server or its database does not answer healthy.
 */
test("the health address says healthy", async ({ page }) => {
  // The request of the page itself, so it goes to the same address as the browser.
  const response = await page.request.get("/api/health");
  expect(response.status()).toBe(200);
  expect(await response.json()).toEqual({ status: "healthy" });
});
