import { render, screen, within } from "@testing-library/react";
import userEvent from "@testing-library/user-event";
import { MemoryRouter } from "react-router";
import { describe, expect, it } from "vitest";
import { findAxeViolations } from "../test/axe";
import TopBar from "./TopBar";

function renderTopBar() {
  return render(
    <MemoryRouter>
      <TopBar />
    </MemoryRouter>,
  );
}

describe("TopBar", () => {
  it("links the name Connect to the home page", () => {
    renderTopBar();

    expect(screen.getByRole("link", { name: "Connect" })).toHaveAttribute(
      "href",
      "/",
    );
  });

  it("links Make an account to the page where a person creates an account", () => {
    renderTopBar();

    expect(
      screen.getByRole("link", { name: "Make an account" }),
    ).toHaveAttribute("href", "/create-account");
  });

  it("puts both links in the main navigation", () => {
    renderTopBar();

    const navigation = screen.getByRole("navigation", { name: "Main" });
    expect(within(navigation).getAllByRole("link")).toHaveLength(2);
  });

  it("reaches Connect and then Make an account with the Tab key", async () => {
    const user = userEvent.setup();
    renderTopBar();

    await user.tab();
    expect(screen.getByRole("link", { name: "Connect" })).toHaveFocus();

    await user.tab();
    expect(screen.getByRole("link", { name: "Make an account" })).toHaveFocus();
  });

  it("has no accessibility problems", async () => {
    const { container } = renderTopBar();

    expect(await findAxeViolations(container)).toEqual([]);
  });
});
