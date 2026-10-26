import { render, screen } from "@testing-library/react";
import { describe, expect, it } from "vitest";
import { findAxeViolations } from "../test/axe";
import TagList from "./TagList";

describe("TagList", () => {
  it("shows the tag names under the given heading", () => {
    render(
      <TagList
        tags={["football", "photography"]}
        heading="Skills and interests"
      />,
    );

    expect(
      screen.getByRole("heading", { name: "Skills and interests" }),
    ).toBeInTheDocument();
    expect(screen.getByText("football")).toBeInTheDocument();
    expect(screen.getByText("photography")).toBeInTheDocument();
  });

  it("shows no heading when none is given", () => {
    render(<TagList tags={["football"]} />);

    expect(screen.queryAllByRole("heading")).toHaveLength(0);
  });

  it("has no button", () => {
    render(<TagList tags={["football"]} />);

    expect(screen.queryAllByRole("button")).toHaveLength(0);
  });

  it("has no textbox", () => {
    render(<TagList tags={["football"]} />);

    expect(screen.queryAllByRole("textbox")).toHaveLength(0);
  });

  it("has no link", () => {
    render(<TagList tags={["football"]} />);

    expect(screen.queryAllByRole("link")).toHaveLength(0);
  });

  it("has no accessibility problems", async () => {
    const { container } = render(
      <TagList tags={["football", "photography"]} />,
    );

    expect(await findAxeViolations(container)).toEqual([]);
  });
});
