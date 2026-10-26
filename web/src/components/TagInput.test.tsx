import { render, screen } from "@testing-library/react";
import userEvent from "@testing-library/user-event";
import { describe, expect, it, vi } from "vitest";
import { findAxeViolations } from "../test/axe";
import TagInput from "./TagInput";

const CHOSEN = [
  { id: "tag-1", name: "film" },
  { id: "tag-2", name: "music" },
];

const SUGGESTED = [
  { id: "tag-3", name: "photography" },
  { id: "tag-4", name: "football" },
];

function renderTagInput(
  overrides: Partial<React.ComponentProps<typeof TagInput>> = {},
) {
  const onAdd = vi.fn();
  const onChange = vi.fn();
  const { container } = render(
    <TagInput
      chosenTags={CHOSEN}
      suggestedTags={SUGGESTED}
      value=""
      onChange={onChange}
      onAdd={onAdd}
      busyName={null}
      {...overrides}
    />,
  );
  return { onAdd, onChange, container };
}

describe("TagInput", () => {
  it("shows the chosen tags under Your tags", () => {
    renderTagInput();

    expect(screen.getByText("film")).toBeInTheDocument();
    expect(screen.getByText("music")).toBeInTheDocument();
  });

  it("shows You have no tags yet when there are none chosen", () => {
    renderTagInput({ chosenTags: [] });

    expect(screen.getByText("You have no tags yet")).toBeInTheDocument();
  });

  it("shows the suggested tags as buttons", () => {
    renderTagInput();

    expect(
      screen.getByRole("button", { name: "photography" }),
    ).toBeInTheDocument();
    expect(
      screen.getByRole("button", { name: "football" }),
    ).toBeInTheDocument();
  });

  it("marks a chosen suggestion as pressed and does nothing when pressed again", async () => {
    const user = userEvent.setup();
    const { onAdd } = renderTagInput({
      suggestedTags: [...SUGGESTED, { id: "tag-5", name: "film" }],
    });

    const filmButton = screen.getByRole("button", { name: "film" });
    expect(filmButton).toHaveAttribute("aria-pressed", "true");

    await user.click(filmButton);

    expect(onAdd).not.toHaveBeenCalled();
  });

  it("calls onAdd with the name when an unpressed suggestion is tapped", async () => {
    const user = userEvent.setup();
    const { onAdd } = renderTagInput();

    await user.click(screen.getByRole("button", { name: "photography" }));

    expect(onAdd).toHaveBeenCalledWith("photography");
  });

  it("calls onAdd with the typed name when Add is pressed", async () => {
    const user = userEvent.setup();
    const { onAdd } = renderTagInput({ value: "drone video" });

    await user.click(screen.getByRole("button", { name: "Add" }));

    expect(onAdd).toHaveBeenCalledWith("drone video");
  });

  it("calls onChange as the member types in Add your own", async () => {
    const user = userEvent.setup();
    const { onChange } = renderTagInput();

    await user.type(screen.getByLabelText("Add your own"), "a");

    expect(onChange).toHaveBeenCalledWith("a");
  });

  it("shows the placeholder Type a tag in the field", () => {
    renderTagInput();

    expect(screen.getByPlaceholderText("Type a tag")).toBeInTheDocument();
  });

  it("shows the message of the server below the field", () => {
    renderTagInput({ fieldError: "Fill in a tag" });

    expect(screen.getByText("Fill in a tag")).toBeInTheDocument();
    expect(screen.getByLabelText("Add your own")).toHaveAccessibleDescription(
      "Fill in a tag",
    );
  });

  it("disables the field and Add while the typed tag is being saved", () => {
    renderTagInput({ value: "drone video", busyName: "drone video" });

    expect(screen.getByLabelText("Add your own")).toBeDisabled();
    expect(screen.getByRole("button", { name: "Saving..." })).toBeDisabled();
  });

  it("disables only the busy suggested tag, not the others", () => {
    renderTagInput({ busyName: "football" });

    expect(screen.getByRole("button", { name: "Saving..." })).toBeDisabled();
    expect(screen.getByRole("button", { name: "photography" })).toBeEnabled();
  });

  it("shows the hint about skills and interests", () => {
    renderTagInput();

    expect(
      screen.getByText(
        "Skills and interests only. Nothing about your health, faith or other private matters.",
      ),
    ).toBeInTheDocument();
  });

  it("has no accessibility problems", async () => {
    const { container } = renderTagInput();

    expect(await findAxeViolations(container)).toEqual([]);
  });
});
