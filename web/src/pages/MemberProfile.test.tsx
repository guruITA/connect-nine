import { render, screen } from "@testing-library/react";
import { MemoryRouter, Route, Routes } from "react-router";
import { afterEach, describe, expect, it, vi } from "vitest";
import type { Mock } from "vitest";
import { getMemberProfile } from "../api/memberProfiles";
import { findAxeViolations } from "../test/axe";
import MemberProfile from "./MemberProfile";

vi.mock("../api/memberProfiles", async () => {
  const actual = await vi.importActual<typeof import("../api/memberProfiles")>(
    "../api/memberProfiles",
  );
  return {
    ...actual,
    getMemberProfile: vi.fn(),
  };
});

const mockedGetMemberProfile = getMemberProfile as Mock;

const PROFILE = {
  firstName: "Robin",
  ageBand: "YoungAdult" as const,
  region: "Amstelveen",
  bio: "Test member.",
  tags: ["football", "photography"],
};

function renderPage(id = "member-1") {
  return render(
    <MemoryRouter initialEntries={[`/members/${id}`]}>
      <Routes>
        <Route path="/members/:id" element={<MemberProfile />} />
      </Routes>
    </MemoryRouter>,
  );
}

describe("MemberProfile", () => {
  afterEach(() => {
    mockedGetMemberProfile.mockReset();
  });

  it("shows Loading while waiting", () => {
    mockedGetMemberProfile.mockReturnValue(new Promise(() => {}));
    renderPage();

    expect(screen.getByText("Loading...")).toBeInTheDocument();
  });

  it("shows the first name as the one heading and the profile details with their labels", async () => {
    mockedGetMemberProfile.mockResolvedValue({
      outcome: "loaded",
      profile: PROFILE,
    });
    renderPage();

    expect(
      await screen.findByRole("heading", { name: "Robin" }),
    ).toBeInTheDocument();
    expect(screen.getByText("Age band")).toBeInTheDocument();
    expect(screen.getByText("18 to 21")).toBeInTheDocument();
    expect(screen.getByText("Region")).toBeInTheDocument();
    expect(screen.getByText("Amstelveen")).toBeInTheDocument();
    expect(screen.getByText("About you")).toBeInTheDocument();
    expect(screen.getByText("Test member.")).toBeInTheDocument();
  });

  it("leaves out Region when the server left it out", async () => {
    mockedGetMemberProfile.mockResolvedValue({
      outcome: "loaded",
      profile: { firstName: "Sam", ageBand: "Adolescent" as const },
    });
    renderPage();

    await screen.findByRole("heading", { name: "Sam" });

    expect(screen.queryByText("Region")).not.toBeInTheDocument();
  });

  it("leaves out the About you card when the server left out the short line", async () => {
    mockedGetMemberProfile.mockResolvedValue({
      outcome: "loaded",
      profile: { firstName: "Sam", ageBand: "Adolescent" as const },
    });
    renderPage();

    await screen.findByRole("heading", { name: "Sam" });

    expect(
      screen.queryByRole("heading", { name: "About you" }),
    ).not.toBeInTheDocument();
  });

  it("shows Profile details as its own card beside About you", async () => {
    mockedGetMemberProfile.mockResolvedValue({
      outcome: "loaded",
      profile: PROFILE,
    });
    renderPage();

    await screen.findByRole("heading", { name: "Robin" });

    expect(
      screen.getByRole("heading", { name: "Profile details" }),
    ).toBeInTheDocument();
  });

  it("has no Edit button and no text field", async () => {
    mockedGetMemberProfile.mockResolvedValue({
      outcome: "loaded",
      profile: PROFILE,
    });
    renderPage();

    await screen.findByRole("heading", { name: "Robin" });

    expect(
      screen.queryByRole("button", { name: "Edit" }),
    ).not.toBeInTheDocument();
    expect(screen.queryAllByRole("textbox")).toHaveLength(0);
  });

  it("shows the tags of the member under Skills and interests", async () => {
    mockedGetMemberProfile.mockResolvedValue({
      outcome: "loaded",
      profile: PROFILE,
    });
    renderPage();

    await screen.findByRole("heading", { name: "Robin" });

    expect(screen.getByText("Skills and interests")).toBeInTheDocument();
    expect(screen.getByText("football")).toBeInTheDocument();
    expect(screen.getByText("photography")).toBeInTheDocument();
  });

  it("shows no label when the member has no tags", async () => {
    mockedGetMemberProfile.mockResolvedValue({
      outcome: "loaded",
      profile: { ...PROFILE, tags: [] },
    });
    renderPage();

    await screen.findByRole("heading", { name: "Robin" });

    expect(screen.queryByText("Skills and interests")).not.toBeInTheDocument();
  });

  it("shows no label when the tags are left out by the rule for minors", async () => {
    mockedGetMemberProfile.mockResolvedValue({
      outcome: "loaded",
      profile: { firstName: "Sam", ageBand: "From15To17" as const },
    });
    renderPage();

    await screen.findByRole("heading", { name: "Sam" });

    expect(screen.queryByText("Skills and interests")).not.toBeInTheDocument();
  });

  it("has no Remove button and no field Add your own on the tags", async () => {
    mockedGetMemberProfile.mockResolvedValue({
      outcome: "loaded",
      profile: PROFILE,
    });
    renderPage();

    await screen.findByRole("heading", { name: "Robin" });

    expect(
      screen.queryByRole("button", { name: /Remove/ }),
    ).not.toBeInTheDocument();
    expect(screen.queryByLabelText("Add your own")).not.toBeInTheDocument();
  });

  it("shows This member does not exist with a working link on 404", async () => {
    mockedGetMemberProfile.mockResolvedValue({ outcome: "notFound" });
    renderPage();

    expect(
      await screen.findByRole("heading", {
        name: "This member does not exist",
      }),
    ).toBeInTheDocument();
    expect(
      screen.getByRole("link", { name: "Go to My account" }),
    ).toHaveAttribute("href", "/account");
  });

  it("shows the Log in message instead of the member when not logged in", async () => {
    mockedGetMemberProfile.mockResolvedValue({ outcome: "notLoggedIn" });
    renderPage("00000000-0000-4000-8000-000000000141");

    expect(
      await screen.findByRole("heading", { name: "Log in to see members" }),
    ).toBeInTheDocument();
    expect(
      screen.getByText(
        "Member pages on Connect are only for people who are logged in, to keep the young people on Connect safe.",
      ),
    ).toBeInTheDocument();
    expect(screen.getByRole("link", { name: "Log in" })).toHaveAttribute(
      "href",
      "/login?next=%2Fmembers%2F00000000-0000-4000-8000-000000000141",
    );
    expect(
      screen.queryByText("00000000-0000-4000-8000-000000000141"),
    ).not.toBeInTheDocument();
  });

  it("shows Connect is not reachable right now in an alert", async () => {
    mockedGetMemberProfile.mockResolvedValue({ outcome: "notReachable" });
    renderPage();

    expect(await screen.findByRole("alert")).toHaveTextContent(
      "Connect is not reachable right now, try again later",
    );
  });

  it("shows Connect is not reachable right now when the fetch call fails", async () => {
    mockedGetMemberProfile.mockResolvedValue({ outcome: "notReachable" });
    renderPage();

    expect(await screen.findByRole("alert")).toHaveTextContent(
      "Connect is not reachable right now, try again later",
    );
  });

  it("sets the browser tab title without the member's name", async () => {
    mockedGetMemberProfile.mockResolvedValue({
      outcome: "loaded",
      profile: PROFILE,
    });
    renderPage();

    await screen.findByRole("heading", { name: "Robin" });

    expect(document.title).toBe("Connect, Member profile");
  });

  it("has no accessibility problems on the filled profile", async () => {
    mockedGetMemberProfile.mockResolvedValue({
      outcome: "loaded",
      profile: PROFILE,
    });
    const { container } = renderPage();

    await screen.findByRole("heading", { name: "Robin" });

    expect(await findAxeViolations(container)).toEqual([]);
  });
});
