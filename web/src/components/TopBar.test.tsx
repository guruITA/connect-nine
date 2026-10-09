import { render, screen, within } from "@testing-library/react";
import userEvent from "@testing-library/user-event";
import { MemoryRouter } from "react-router";
import { describe, expect, it } from "vitest";
import { findAxeViolations } from "../test/axe";
import TopBar from "./TopBar";

function renderTopBar(path = "/") {
  return render(
    <MemoryRouter initialEntries={[path]}>
      <TopBar />
    </MemoryRouter>,
  );
}

describe("TopBar", () => {
  it("links the name Young Amstelland Connect to the home page", () => {
    renderTopBar();

    expect(
      screen.getByRole("link", { name: "Young Amstelland Connect" }),
    ).toHaveAttribute("href", "/");
  });

  it("shows the tabs Home, Discover, Events, Projects and Log in in the main navigation", () => {
    renderTopBar();

    const navigation = screen.getByRole("navigation", { name: "Main" });
    const tabs = within(navigation).getAllByRole("link");
    expect(
      tabs.map((tab) => [tab.textContent, tab.getAttribute("href")]),
    ).toEqual([
      ["Home", "/"],
      ["Discover", "/discover"],
      ["Events", "/events"],
      ["Projects", "/projects"],
      ["Log in", "/login"],
    ]);
  });

  it("marks Home as the current page on the home page", () => {
    renderTopBar("/");

    expect(screen.getByRole("link", { name: "Home" })).toHaveAttribute(
      "aria-current",
      "page",
    );
    expect(screen.getByRole("link", { name: "Discover" })).not.toHaveAttribute(
      "aria-current",
    );
  });

  it("links Make an account to the page where a person creates an account", () => {
    renderTopBar();

    expect(
      screen.getByRole("link", { name: "Make an account" }),
    ).toHaveAttribute("href", "/create-account");
  });

  it("reaches the name, every tab and Make an account with the Tab key, in that order", async () => {
    const user = userEvent.setup();
    renderTopBar();

    const order = [
      "Young Amstelland Connect",
      "Home",
      "Discover",
      "Events",
      "Projects",
      "Log in",
      "Make an account",
    ];
    for (const name of order) {
      await user.tab();
      expect(screen.getByRole("link", { name })).toHaveFocus();
    }
  });

  it("has no accessibility problems", async () => {
    const { container } = renderTopBar();

    expect(await findAxeViolations(container)).toEqual([]);
  });
});
