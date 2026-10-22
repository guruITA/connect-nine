import { expect, test } from "@playwright/test";

/**
 * A visitor opens the home page and finds the way to Make an account (#265).
 * It only reads and clicks a link, so it can run against any address.
 *
 * @param page - The browser page Playwright opens for this test.
 * @returns Nothing. The test fails when the page does not load or the link leads elsewhere.
 */
test("a visitor sees the home page and finds the way to make an account", async ({
  page,
}) => {
  // The server answers and React draws the page with one main heading.
  const response = await page.goto("/");
  expect(response?.ok()).toBe(true);
  await expect(page.getByRole("heading", { level: 1 })).toBeVisible();

  // Press the first link Make an account.
  await page.getByRole("link", { name: "Make an account" }).first().click();

  // The browser is on the page where a person makes an account.
  await expect(page).toHaveURL(/\/create-account$/);
});
