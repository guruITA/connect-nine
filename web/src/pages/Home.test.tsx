import { render, screen } from "@testing-library/react";
import { MemoryRouter } from "react-router";
import { describe, expect, it, vi } from "vitest";
import App from "../App";
import { findAxeViolations } from "../test/axe";
import Home from "./Home";

describe("Home", () => {
  it("shows the heading Connect", () => {
    render(<Home />);

    expect(
      screen.getByRole("heading", { level: 1, name: "Connect" }),
    ).toBeInTheDocument();
  });

  it("says in one plain sentence what Connect is", () => {
    render(<Home />);

    expect(
      screen.getByText(
        "On Connect, young people from Amstelland and the organisations around them find each other.",
      ),
    ).toBeInTheDocument();
  });

  it("asks the server for nothing and shows no number", () => {
    const fetchSpy = vi.spyOn(globalThis, "fetch");

    const { container } = render(<Home />);

    expect(fetchSpy).not.toHaveBeenCalled();
    expect(container).not.toHaveTextContent(/\d/);
  });

  it("is the page at / and, with the top bar, has no accessibility problems", async () => {
    render(
      <MemoryRouter initialEntries={["/"]}>
        <App />
      </MemoryRouter>,
    );

    expect(screen.getByRole("main")).toContainElement(
      screen.getByRole("heading", { level: 1, name: "Connect" }),
    );
    // The title and the lang attribute come from index.html, which jsdom does not load.
    expect(await findAxeViolations(document.body)).toEqual([]);
  });
});
