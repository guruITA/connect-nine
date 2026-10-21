import { expect, test } from "@playwright/test";

// The home page in a real browser (#265). It only reads and clicks a link, so it can run against
// any address, also while the form of #264 does not exist yet.
test("a visitor sees the home page and finds the way to make an account", async ({
  page,
}) => {
  // Open the home page.
  const response = await page.goto("/");
  expect(response?.ok()).toBe(true);

  // React drew the page: it has one main heading.
  await expect(page.getByRole("heading", { level: 1 })).toBeVisible();

  // Press the first link Make an account.
  await page.getByRole("link", { name: "Make an account" }).first().click();

  // The browser is now on the page where a person makes an account.
  await expect(page).toHaveURL(/\/create-account$/);
});
