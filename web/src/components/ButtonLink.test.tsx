import { render, screen } from "@testing-library/react";
import { MemoryRouter } from "react-router";
import { describe, expect, it } from "vitest";
import ButtonLink from "./ButtonLink";

describe("ButtonLink", () => {
  it("is a link to another page of Connect", () => {
    render(
      <MemoryRouter>
        <ButtonLink to="/create-account">Make an account</ButtonLink>
      </MemoryRouter>,
    );

    expect(
      screen.getByRole("link", { name: "Make an account" }),
    ).toHaveAttribute("href", "/create-account");
  });

  it("is a link to a part of the same page when the address starts with #", () => {
    render(<ButtonLink to="#how-it-works">How does it work?</ButtonLink>);

    expect(
      screen.getByRole("link", { name: "How does it work?" }),
    ).toHaveAttribute("href", "#how-it-works");
  });
});
