import { render, screen, within } from "@testing-library/react";
import { MemoryRouter } from "react-router";
import { describe, expect, it, vi } from "vitest";
import App from "../App";
import { findAxeViolations } from "../test/axe";
import Home from "./Home";

function renderHome() {
  return render(
    <MemoryRouter>
      <Home />
    </MemoryRouter>,
  );
}

describe("Home", () => {
  it("shows the heading of the design", () => {
    renderHome();

    expect(
      screen.getByRole("heading", {
        level: 1,
        name: "Do work that matters, close to home",
      }),
    ).toBeInTheDocument();
  });

  it("says in one plain sentence what Connect is", () => {
    renderHome();

    expect(
      screen.getByText(
        "Connect brings young people in Amstelland together with organisations, projects and events, all year round and not only at the gala.",
      ),
    ).toBeInTheDocument();
  });

  it("shows every part of the design in order", () => {
    renderHome();

    const headings = screen
      .getAllByRole("heading", { level: 2 })
      .map((h) => h.textContent);
    expect(headings).toEqual([
      "How does Connect work?",
      "Start where it suits you",
      "Made by a foundation that believes in young people",
      "Reach young talent in Amstelland",
      "Ready to join?",
    ]);
  });

  it("sends every Make an account to the page where a person creates an account", () => {
    renderHome();

    const links = screen.getAllByRole("link", { name: "Make an account" });
    expect(links).toHaveLength(3);
    for (const link of links) {
      expect(link).toHaveAttribute("href", "/create-account");
    }
  });

  it("scrolls to How does Connect work? with the button How does it work?", () => {
    const { container } = renderHome();

    expect(
      screen.getByRole("link", { name: "How does it work?" }),
    ).toHaveAttribute("href", "#how-it-works");
    expect(container.querySelector("#how-it-works")).toContainElement(
      screen.getByRole("heading", { name: "How does Connect work?" }),
    );
  });

  it("shows the four steps from account to first step as a numbered list", () => {
    const { container } = renderHome();

    const stepList = container.querySelector("ol");
    if (!stepList) {
      throw new Error("The home page has no numbered list");
    }
    expect(
      within(stepList)
        .getAllByRole("heading", { level: 3 })
        .map((heading) => heading.textContent),
    ).toEqual([
      "Make an account",
      "Create your profile",
      "Find your way in",
      "Meet each other",
    ]);
  });

  it("links the tiles to Discover, Events and Projects", () => {
    renderHome();

    expect(screen.getByRole("link", { name: "Open Discover" })).toHaveAttribute(
      "href",
      "/discover",
    );
    expect(screen.getByRole("link", { name: "Open Events" })).toHaveAttribute(
      "href",
      "/events",
    );
    expect(screen.getByRole("link", { name: "Open Projects" })).toHaveAttribute(
      "href",
      "/projects",
    );
  });

  it("asks the server for nothing", () => {
    const fetchSpy = vi.spyOn(globalThis, "fetch");

    renderHome();

    expect(fetchSpy).not.toHaveBeenCalled();
  });

  it("is the page at / and, with the top bar and the footer, has no accessibility problems", async () => {
    render(
      <MemoryRouter initialEntries={["/"]}>
        <App />
      </MemoryRouter>,
    );

    expect(screen.getByRole("main")).toContainElement(
      screen.getByRole("heading", {
        level: 1,
        name: "Do work that matters, close to home",
      }),
    );
    // The title and the lang attribute come from index.html, which jsdom does not load.
    expect(await findAxeViolations(document.body)).toEqual([]);
  });
});
