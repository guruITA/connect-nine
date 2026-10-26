import { render, screen } from "@testing-library/react";
import { MemoryRouter } from "react-router";
import { describe, expect, it } from "vitest";
import { findAxeViolations } from "../test/axe";
import LoggedOutMessage from "./LoggedOutMessage";

const MEMBER_PATH = "/members/00000000-0000-4000-8000-000000000141";

function renderMessage(path: string) {
  return render(
    <MemoryRouter>
      <LoggedOutMessage path={path} />
    </MemoryRouter>,
  );
}

describe("LoggedOutMessage", () => {
  it("shows the heading and the sentence why", () => {
    renderMessage(MEMBER_PATH);

    expect(
      screen.getByRole("heading", { name: "Log in to see members" }),
    ).toBeInTheDocument();
    expect(
      screen.getByText(
        "Member pages on Connect are only for people who are logged in, to keep the young people on Connect safe.",
      ),
    ).toBeInTheDocument();
  });

  it("links Log in to login with the member path encoded in next", () => {
    renderMessage(MEMBER_PATH);

    expect(screen.getByRole("link", { name: "Log in" })).toHaveAttribute(
      "href",
      "/login?next=%2Fmembers%2F00000000-0000-4000-8000-000000000141",
    );
  });

  it("leaves a path outside /members out of next", () => {
    renderMessage("/account");

    expect(screen.getByRole("link", { name: "Log in" })).toHaveAttribute(
      "href",
      "/login",
    );
  });

  it("has no accessibility problems", async () => {
    const { container } = renderMessage(MEMBER_PATH);

    expect(await findAxeViolations(container)).toEqual([]);
  });
});
