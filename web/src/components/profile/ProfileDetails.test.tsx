import { render, screen } from "@testing-library/react";
import { describe, expect, it } from "vitest";
import { findAxeViolations } from "../../test/axe";
import ProfileDetails from "./ProfileDetails";

describe("ProfileDetails", () => {
  it("shows the age band in words, the region and the short line with their labels", () => {
    render(
      <ProfileDetails
        ageBand="YoungAdult"
        region="Amstelveen"
        bio="I like film and music."
      />,
    );

    expect(screen.getByText("Age band")).toBeInTheDocument();
    expect(screen.getByText("18 to 21")).toBeInTheDocument();
    expect(screen.getByText("Region")).toBeInTheDocument();
    expect(screen.getByText("Amstelveen")).toBeInTheDocument();
    expect(screen.getByText("About you")).toBeInTheDocument();
    expect(screen.getByText("I like film and music.")).toBeInTheDocument();
  });

  it("leaves out the label and the row when the short line is empty", () => {
    render(<ProfileDetails ageBand="Adolescent" region="Amstelveen" bio="" />);

    expect(screen.queryByText("About you")).not.toBeInTheDocument();
  });

  it("leaves out the label and the row when the short line is missing", () => {
    render(<ProfileDetails ageBand="Adolescent" region="Amstelveen" />);

    expect(screen.queryByText("About you")).not.toBeInTheDocument();
  });

  it("leaves out the label and the row when the region is missing", () => {
    render(<ProfileDetails ageBand="Adolescent" />);

    expect(screen.queryByText("Region")).not.toBeInTheDocument();
  });

  it("reads each value together with its label", () => {
    render(<ProfileDetails ageBand="Adult" region="Uithoorn" />);

    const term = screen.getByText("Region");
    expect(term.tagName).toBe("DT");
    expect(term.nextElementSibling).toHaveTextContent("Uithoorn");
  });

  it("leaves out the short line when showBio is false even with a bio", () => {
    render(
      <ProfileDetails
        ageBand="YoungAdult"
        region="Amstelveen"
        bio="I like film and music."
        showBio={false}
      />,
    );

    expect(
      screen.queryByText("I like film and music."),
    ).not.toBeInTheDocument();
  });

  it("has no accessibility problems", async () => {
    const { container } = render(
      <ProfileDetails
        ageBand="YoungAdult"
        region="Amstelveen"
        bio="I like film and music."
      />,
    );

    expect(await findAxeViolations(container)).toEqual([]);
  });
});
