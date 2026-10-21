import { randomUUID } from "node:crypto";
import { expect, test } from "@playwright/test";

// Makes an account through a real browser, as a person new to Connect does (#265, issue 03
// Create an account). The texts are the ones the checklist of the form (#264) names.
test("a person new to Connect makes an account", async ({ page }) => {
  // A new made up address every run, so the test never meets an account it made before.
  const email = `e2e-${Date.now()}@example.org`;
  // Made up at the start of the run and not in Git, 36 characters, so the test keeps passing after
  // the rule of at least fifteen characters (#17). The trace of a failed run does show it
  // (docs/testing/testData.md), which is fine for a made up account without data.
  const password = randomUUID();

  // Open the home page.
  await page.goto("/");

  // Press the first link Make an account: in the top bar now, below the sentence about Connect
  // once issue 06 replaces the top bar (docs/pages/CreateAccount.md). Both lead to the same page.
  await page.getByRole("link", { name: "Make an account" }).first().click();

  // Fill in the email address.
  await page.getByLabel("Email").fill(email);

  // Fill in the password. exact, so it never matches the field Password again of #17.
  await page.getByLabel("Password", { exact: true }).fill(password);

  // Press Make account.
  await page.getByRole("button", { name: "Make account" }).click();

  // Wait until the page says the account is made.
  await expect(page.getByText("Your account is made")).toBeVisible();
});
