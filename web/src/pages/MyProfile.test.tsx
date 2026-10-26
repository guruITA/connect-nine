import { render, screen } from "@testing-library/react";
import { MemoryRouter, Route, Routes } from "react-router";
import { afterEach, describe, expect, it, vi } from "vitest";
import type { Mock } from "vitest";
import { getCurrentMemberProfile } from "../api/memberProfiles";
import { findAxeViolations } from "../test/axe";
import MyProfile from "./MyProfile";

vi.mock("../api/memberProfiles", async () => {
  const actual = await vi.importActual<typeof import("../api/memberProfiles")>(
    "../api/memberProfiles",
  );
  return {
    ...actual,
    getCurrentMemberProfile: vi.fn(),
  };
});

const mockedGetCurrentMemberProfile = getCurrentMemberProfile as Mock;

const PROFILE = {
  firstName: "Robin",
  ageBand: "From18To21" as const,
  region: "Amstelveen",
  bio: "I like film and music.",
};

function renderPage() {
  return render(
    <MemoryRouter initialEntries={["/profile"]}>
      <Routes>
        <Route path="/profile" element={<MyProfile />} />
        <Route path="/profile/new" element={<p>Create your profile</p>} />
      </Routes>
    </MemoryRouter>,
  );
}

describe("MyProfile", () => {
  afterEach(() => {
    mockedGetCurrentMemberProfile.mockReset();
  });

  it("shows Loading your profile while waiting", () => {
    mockedGetCurrentMemberProfile.mockReturnValue(new Promise(() => {}));
    renderPage();

    expect(screen.getByText("Loading your profile...")).toBeInTheDocument();
  });

  it("shows the first name as the one heading and the profile details", async () => {
    mockedGetCurrentMemberProfile.mockResolvedValue({
      outcome: "loaded",
      profile: PROFILE,
    });
    renderPage();

    expect(
      await screen.findByRole("heading", { name: "Robin" }),
    ).toBeInTheDocument();
    expect(screen.getByText("18 to 21")).toBeInTheDocument();
    expect(screen.getByText("Amstelveen")).toBeInTheDocument();
    expect(screen.getByText("I like film and music.")).toBeInTheDocument();
  });

  it("leaves out About you when the short line is empty", async () => {
    mockedGetCurrentMemberProfile.mockResolvedValue({
      outcome: "loaded",
      profile: { ...PROFILE, bio: null },
    });
    renderPage();

    await screen.findByRole("heading", { name: "Robin" });

    expect(screen.queryByText("About you")).not.toBeInTheDocument();
  });

  it("goes to Create your profile when there is no profile yet", async () => {
    mockedGetCurrentMemberProfile.mockResolvedValue({ outcome: "noProfile" });
    renderPage();

    expect(await screen.findByText("Create your profile")).toBeInTheDocument();
  });

  it("shows Log in first to see this page in an alert when not logged in", async () => {
    mockedGetCurrentMemberProfile.mockResolvedValue({ outcome: "notLoggedIn" });
    renderPage();

    expect(await screen.findByRole("alert")).toHaveTextContent(
      "Log in first to see this page",
    );
  });

  it("shows Connect is not reachable right now in an alert", async () => {
    mockedGetCurrentMemberProfile.mockResolvedValue({
      outcome: "notReachable",
    });
    renderPage();

    expect(await screen.findByRole("alert")).toHaveTextContent(
      "Connect is not reachable right now, try again later",
    );
  });

  it("sets the browser tab title without the first name", async () => {
    mockedGetCurrentMemberProfile.mockResolvedValue({
      outcome: "loaded",
      profile: PROFILE,
    });
    renderPage();

    await screen.findByRole("heading", { name: "Robin" });

    expect(document.title).toBe("Connect, My profile");
  });

  it("shows no email address and no Edit button", async () => {
    mockedGetCurrentMemberProfile.mockResolvedValue({
      outcome: "loaded",
      profile: PROFILE,
    });
    renderPage();

    await screen.findByRole("heading", { name: "Robin" });

    expect(screen.queryByText(/@/)).not.toBeInTheDocument();
    expect(
      screen.queryByRole("button", { name: "Edit" }),
    ).not.toBeInTheDocument();
    expect(
      screen.queryByRole("link", { name: "Edit" }),
    ).not.toBeInTheDocument();
  });

  it("shows a decorative circle with the first letter of the first name", async () => {
    mockedGetCurrentMemberProfile.mockResolvedValue({
      outcome: "loaded",
      profile: PROFILE,
    });
    renderPage();

    await screen.findByRole("heading", { name: "Robin" });

    expect(screen.getByText("R", { selector: "span" })).toHaveAttribute(
      "aria-hidden",
      "true",
    );
  });

  it("has no accessibility problems on the filled profile", async () => {
    mockedGetCurrentMemberProfile.mockResolvedValue({
      outcome: "loaded",
      profile: PROFILE,
    });
    const { container } = renderPage();

    await screen.findByRole("heading", { name: "Robin" });

    expect(await findAxeViolations(container)).toEqual([]);
  });
});
