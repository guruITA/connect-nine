import { render, screen, within } from "@testing-library/react";
import { MemoryRouter } from "react-router";
import { describe, expect, it } from "vitest";
import { findAxeViolations } from "../test/axe";
import SiteFooter from "./SiteFooter";

function renderFooter() {
  return render(
    <MemoryRouter>
      <SiteFooter />
    </MemoryRouter>,
  );
}

describe("SiteFooter", () => {
  it("links to every page in the columns Connect, Account and About", () => {
    renderFooter();

    const navigation = screen.getByRole("navigation", { name: "Footer" });
    const links = within(navigation).getAllByRole("link");
    expect(
      links.map((link) => [link.textContent, link.getAttribute("href")]),
    ).toEqual([
      ["Home", "/"],
      ["Discover", "/discover"],
      ["Events", "/events"],
      ["Projects", "/projects"],
      ["Log in", "/login"],
      ["Make an account", "/create-account"],
      ["Privacy", "/privacy"],
      ["House rules", "/house-rules"],
    ]);
  });

  it("says that Connect is an initiative of Stichting WailSalutem", () => {
    renderFooter();

    expect(screen.getByText("An initiative of")).toBeInTheDocument();
    expect(screen.getByText("Stichting WailSalutem")).toBeInTheDocument();
  });

  it("has no accessibility problems", async () => {
    const { container } = renderFooter();

    expect(await findAxeViolations(container)).toEqual([]);
  });
});
