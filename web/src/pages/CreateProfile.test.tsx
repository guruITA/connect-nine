import { render, screen, within } from "@testing-library/react";
import userEvent from "@testing-library/user-event";
import { MemoryRouter } from "react-router";
import { describe, expect, it, vi } from "vitest";
import type { Mock } from "vitest";
import { createMemberProfile } from "../api/memberProfiles";
import type { CreateMemberProfileResult } from "../api/memberProfiles";
import { findAxeViolations } from "../test/axe";
import CreateProfile from "./CreateProfile";

vi.mock("../api/memberProfiles", async () => {
  const actual = await vi.importActual<typeof import("../api/memberProfiles")>(
    "../api/memberProfiles",
  );
  return {
    ...actual,
    createMemberProfile: vi.fn(),
  };
});

const mockedCreateMemberProfile = createMemberProfile as Mock;

function renderPage() {
  return render(
    <MemoryRouter>
      <CreateProfile />
    </MemoryRouter>,
  );
}

async function fillValidForm(user: ReturnType<typeof userEvent.setup>) {
  await user.type(screen.getByLabelText("First name"), "Robin");
  await user.selectOptions(screen.getByLabelText("Age band"), "15 to 17");
  await user.type(screen.getByLabelText("Region"), "Amstelveen");
  await user.type(screen.getByLabelText("About you"), "Hello there");
}

function createDeferred() {
  let resolve!: (value: CreateMemberProfileResult) => void;
  const promise = new Promise<CreateMemberProfileResult>((res) => {
    resolve = res;
  });
  return { promise, resolve };
}

describe("CreateProfile", () => {
  it("offers exactly the three age bands with none chosen at the start", () => {
    renderPage();

    const select = screen.getByLabelText("Age band") as HTMLSelectElement;
    expect(select.value).toBe("");

    const bandOptions = within(select)
      .getAllByRole("option")
      .filter((option) => !(option as HTMLOptionElement).disabled);
    expect(bandOptions.map((option) => option.textContent)).toEqual([
      "15 to 17",
      "18 to 21",
      "22 to 30",
    ]);
  });

  it("counts down the characters left in About you as the member types", async () => {
    const user = userEvent.setup();
    renderPage();

    expect(
      screen.getByText(
        "Do not write your school, address or phone number here. 280 characters left.",
      ),
    ).toBeInTheDocument();

    await user.type(screen.getByLabelText("About you"), "Hi");

    expect(
      screen.getByText(
        "Do not write your school, address or phone number here. 278 characters left.",
      ),
    ).toBeInTheDocument();
  });

  it("says how many characters are too many past 280", async () => {
    const user = userEvent.setup();
    renderPage();

    await user.click(screen.getByLabelText("About you"));
    await user.paste("x".repeat(282));

    expect(screen.getByLabelText("About you")).toHaveAccessibleDescription(
      "Do not write your school, address or phone number here. 2 characters too many.",
    );
  });

  it("disables the Save button and says Saving... while the request is in flight", async () => {
    const user = userEvent.setup();
    const deferred = createDeferred();
    mockedCreateMemberProfile.mockReturnValue(deferred.promise);
    renderPage();

    await fillValidForm(user);
    await user.click(screen.getByRole("button", { name: "Save" }));

    const button = screen.getByRole("button", { name: "Saving..." });
    expect(button).toBeDisabled();

    deferred.resolve({ outcome: "saved", id: "profile-1" });
    expect(await screen.findByRole("status")).toHaveTextContent(
      "Your profile is saved",
    );
  });

  it("shows Your profile is saved in a status area on success", async () => {
    const user = userEvent.setup();
    mockedCreateMemberProfile.mockResolvedValue({
      outcome: "saved",
      id: "profile-1",
    });
    renderPage();

    await fillValidForm(user);
    await user.click(screen.getByRole("button", { name: "Save" }));

    const status = await screen.findByRole("status");
    expect(status).toHaveTextContent("Your profile is saved");
    expect(
      within(status).getByRole("link", { name: "See my profile" }),
    ).toHaveAttribute("href", "/profile");
  });

  it("shows each wrong field message below its own field and focuses the first wrong field", async () => {
    const user = userEvent.setup();
    mockedCreateMemberProfile.mockResolvedValue({
      outcome: "fieldErrors",
      fieldErrors: {
        firstName: "Fill in your first name",
        region:
          "Fill in only your town or area, not your street, house number or postcode",
      },
    });
    renderPage();

    await fillValidForm(user);
    await user.click(screen.getByRole("button", { name: "Save" }));

    const firstNameInput = screen.getByLabelText("First name");
    await screen.findByText("Fill in your first name");
    expect(firstNameInput).toHaveFocus();
    expect(firstNameInput).toHaveAccessibleDescription(
      "Fill in your first name",
    );

    const regionInput = screen.getByLabelText("Region");
    expect(
      screen.getByText(
        "Fill in only your town or area, not your street, house number or postcode",
      ),
    ).toBeInTheDocument();
    expect(regionInput).toHaveAccessibleDescription(
      expect.stringContaining(
        "Fill in only your town or area, not your street, house number or postcode",
      ),
    );
    expect(regionInput).toHaveAttribute("aria-invalid", "true");

    expect(screen.getByLabelText("Region")).toHaveValue("Amstelveen");
  });

  it("shows the conflict of the server above the form in an alert", async () => {
    const user = userEvent.setup();
    mockedCreateMemberProfile.mockResolvedValue({
      outcome: "alreadyHasProfile",
      message: "You already have a profile",
    });
    renderPage();

    await fillValidForm(user);
    await user.click(screen.getByRole("button", { name: "Save" }));

    expect(await screen.findByRole("alert")).toHaveTextContent(
      "You already have a profile",
    );
    expect(screen.getByLabelText("First name")).toHaveValue("Robin");
  });

  it("shows the region hint above the field and links it to the field", () => {
    renderPage();

    expect(screen.getByLabelText("Region")).toHaveAccessibleDescription(
      "Your town or area, for example Amstelveen. Not your street, house number or postcode.",
    );
  });

  it("says beside the form what Connect never asks for", () => {
    renderPage();

    expect(
      screen.getByRole("complementary", { name: "What Connect asks" }),
    ).toHaveTextContent(
      "Never asks for your exact age, your birth date, your school or your street address",
    );
  });

  it("shows Connect is not reachable right now in an alert and keeps every value", async () => {
    const user = userEvent.setup();
    mockedCreateMemberProfile.mockResolvedValue({ outcome: "notReachable" });
    renderPage();

    await fillValidForm(user);
    await user.click(screen.getByRole("button", { name: "Save" }));

    const alert = await screen.findByRole("alert");
    expect(alert).toHaveTextContent(
      "Connect is not reachable right now, your text is kept",
    );

    expect(screen.getByLabelText("First name")).toHaveValue("Robin");
    expect(screen.getByLabelText("Age band")).toHaveValue("Adolescent");
    expect(screen.getByLabelText("Region")).toHaveValue("Amstelveen");
    expect(screen.getByLabelText("About you")).toHaveValue("Hello there");
  });

  it("shows Log in first to save your profile in an alert and keeps every value when not logged in", async () => {
    const user = userEvent.setup();
    mockedCreateMemberProfile.mockResolvedValue({ outcome: "notLoggedIn" });
    renderPage();

    await fillValidForm(user);
    await user.click(screen.getByRole("button", { name: "Save" }));

    const alert = await screen.findByRole("alert");
    expect(alert).toHaveTextContent("Log in first to save your profile");

    expect(screen.getByLabelText("First name")).toHaveValue("Robin");
    expect(screen.getByLabelText("Region")).toHaveValue("Amstelveen");
    expect(screen.getByLabelText("About you")).toHaveValue("Hello there");
  });

  it("has no accessibility problems on the empty form", async () => {
    const { container } = renderPage();

    expect(await findAxeViolations(container)).toEqual([]);
  });

  it("has no accessibility problems with field messages shown", async () => {
    const user = userEvent.setup();
    mockedCreateMemberProfile.mockResolvedValue({
      outcome: "fieldErrors",
      fieldErrors: {
        firstName: "Fill in your first name",
        region:
          "Fill in only your town or area, not your street, house number or postcode",
      },
    });
    const { container } = renderPage();

    await fillValidForm(user);
    await user.click(screen.getByRole("button", { name: "Save" }));
    await screen.findByText("Fill in your first name");

    expect(await findAxeViolations(container)).toEqual([]);
  });
});
