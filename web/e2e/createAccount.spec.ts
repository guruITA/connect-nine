import { randomUUID } from "node:crypto";
import { expect, test } from "@playwright/test";

/**
 * A person new to Connect makes an account in a real browser (#265).
 *
 * @param page - The browser page Playwright opens for this test.
 * @returns Nothing. The test fails when the page never says "Your account is made".
 */
test("a person new to Connect makes an account", async ({ page }) => {
  // A new made up address every run, so the test never meets an old account.
  const email = `e2e-${Date.now()}@example.org`;
  // Aa1! at the end meets every password rule of CreateAccountRequestValidator.
  const password = `${randomUUID()}Aa1!`;

  // Open the home page and go to Make an account.
  await page.goto("/");
  await page.getByRole("link", { name: "Make an account" }).first().click();

  // Fill in the form and send it. exact skips the field Password again of #17.
  await page.getByLabel("Email").fill(email);
  await page.getByLabel("Password", { exact: true }).fill(password);
  await page.getByRole("button", { name: "Make account" }).click();

  // The page confirms the account.
  await expect(page.getByText("Your account is made")).toBeVisible();
});
