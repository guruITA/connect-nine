import { render, screen } from "@testing-library/react";
import userEvent from "@testing-library/user-event";
import { MemoryRouter, Route, Routes } from "react-router";
import { afterEach, describe, expect, it, vi } from "vitest";
import type { Mock } from "vitest";
import { addMemberProfileTag } from "../api/memberProfileTags";
import { getCurrentMemberProfile } from "../api/memberProfiles";
import { getSuggestedTags } from "../api/tags";
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

vi.mock("../api/tags", async () => {
  const actual =
    await vi.importActual<typeof import("../api/tags")>("../api/tags");
  return {
    ...actual,
    getSuggestedTags: vi.fn(),
  };
});

vi.mock("../api/memberProfileTags", async () => {
  const actual = await vi.importActual<
    typeof import("../api/memberProfileTags")
  >("../api/memberProfileTags");
  return {
    ...actual,
    addMemberProfileTag: vi.fn(),
  };
});

const mockedGetCurrentMemberProfile = getCurrentMemberProfile as Mock;
const mockedGetSuggestedTags = getSuggestedTags as Mock;
const mockedAddMemberProfileTag = addMemberProfileTag as Mock;

const PROFILE = {
  firstName: "Robin",
  ageBand: "YoungAdult" as const,
  region: "Amstelveen",
  bio: "I like film and music.",
  tags: [{ id: "tag-1", name: "film" }],
};

const SUGGESTED = [
  { id: "tag-2", name: "photography" },
  { id: "tag-3", name: "football" },
];

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
    mockedGetSuggestedTags.mockReset();
    mockedAddMemberProfileTag.mockReset();
  });

  it("shows Loading your profile while waiting", () => {
    mockedGetCurrentMemberProfile.mockReturnValue(new Promise(() => {}));
    mockedGetSuggestedTags.mockReturnValue(new Promise(() => {}));
    renderPage();

    expect(screen.getByText("Loading your profile...")).toBeInTheDocument();
  });

  it("shows the first name as the one heading and the profile details", async () => {
    mockedGetCurrentMemberProfile.mockResolvedValue({
      outcome: "loaded",
      profile: PROFILE,
    });
    mockedGetSuggestedTags.mockResolvedValue(SUGGESTED);
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
    mockedGetSuggestedTags.mockResolvedValue(SUGGESTED);
    renderPage();

    await screen.findByRole("heading", { name: "Robin" });

    expect(screen.queryByText("About you")).not.toBeInTheDocument();
  });

  it("goes to Create your profile when there is no profile yet", async () => {
    mockedGetCurrentMemberProfile.mockResolvedValue({ outcome: "noProfile" });
    mockedGetSuggestedTags.mockResolvedValue(SUGGESTED);
    renderPage();

    expect(await screen.findByText("Create your profile")).toBeInTheDocument();
  });

  it("shows Log in first to see this page in an alert when not logged in", async () => {
    mockedGetCurrentMemberProfile.mockResolvedValue({ outcome: "notLoggedIn" });
    mockedGetSuggestedTags.mockResolvedValue(SUGGESTED);
    renderPage();

    expect(await screen.findByRole("alert")).toHaveTextContent(
      "Log in first to see this page",
    );
  });

  it("shows Connect is not reachable right now in an alert", async () => {
    mockedGetCurrentMemberProfile.mockResolvedValue({
      outcome: "notReachable",
    });
    mockedGetSuggestedTags.mockResolvedValue(SUGGESTED);
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
    mockedGetSuggestedTags.mockResolvedValue(SUGGESTED);
    renderPage();

    await screen.findByRole("heading", { name: "Robin" });

    expect(document.title).toBe("Connect, My profile");
  });

  it("shows the parent consent banner above the heading while pending", async () => {
    mockedGetCurrentMemberProfile.mockResolvedValue({
      outcome: "loaded",
      profile: { ...PROFILE, parentConsent: "pending" },
    });
    mockedGetSuggestedTags.mockResolvedValue(SUGGESTED);
    const { container } = renderPage();

    const heading = await screen.findByRole("heading", { name: "Robin" });
    const banner = await screen.findByRole("status");

    expect(banner).toHaveTextContent(/Only you can see your profile/);
    expect(
      container.innerHTML.indexOf(banner.outerHTML) <
        container.innerHTML.indexOf(heading.outerHTML),
    ).toBe(true);
  });

  it("shows no parent consent banner when the server sends no value", async () => {
    mockedGetCurrentMemberProfile.mockResolvedValue({
      outcome: "loaded",
      profile: PROFILE,
    });
    mockedGetSuggestedTags.mockResolvedValue(SUGGESTED);
    renderPage();

    await screen.findByRole("heading", { name: "Robin" });

    expect(
      screen.queryByText(/Only you can see your profile/),
    ).not.toBeInTheDocument();
  });

  it("shows no parent consent banner while loading or when there is no answer", async () => {
    mockedGetCurrentMemberProfile.mockResolvedValue({
      outcome: "notReachable",
    });
    mockedGetSuggestedTags.mockResolvedValue(SUGGESTED);
    renderPage();

    await screen.findByRole("alert");

    expect(
      screen.queryByText(/Only you can see your profile/),
    ).not.toBeInTheDocument();
  });

  it("shows no email address and no Edit button", async () => {
    mockedGetCurrentMemberProfile.mockResolvedValue({
      outcome: "loaded",
      profile: PROFILE,
    });
    mockedGetSuggestedTags.mockResolvedValue(SUGGESTED);
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
    mockedGetSuggestedTags.mockResolvedValue(SUGGESTED);
    renderPage();

    await screen.findByRole("heading", { name: "Robin" });

    expect(screen.getByText("R", { selector: "span" })).toHaveAttribute(
      "aria-hidden",
      "true",
    );
  });

  it("shows Loading your tags while the suggested tags are on their way", async () => {
    mockedGetCurrentMemberProfile.mockResolvedValue({
      outcome: "loaded",
      profile: PROFILE,
    });
    mockedGetSuggestedTags.mockReturnValue(new Promise(() => {}));
    renderPage();

    await screen.findByRole("heading", { name: "Robin" });

    expect(screen.getByText("Loading your tags...")).toBeInTheDocument();
  });

  it("shows You have no tags yet when the member has none", async () => {
    mockedGetCurrentMemberProfile.mockResolvedValue({
      outcome: "loaded",
      profile: { ...PROFILE, tags: [] },
    });
    mockedGetSuggestedTags.mockResolvedValue(SUGGESTED);
    renderPage();

    expect(await screen.findByText("You have no tags yet")).toBeInTheDocument();
  });

  it("shows the chosen tags and the suggested tags", async () => {
    mockedGetCurrentMemberProfile.mockResolvedValue({
      outcome: "loaded",
      profile: PROFILE,
    });
    mockedGetSuggestedTags.mockResolvedValue(SUGGESTED);
    renderPage();

    await screen.findByRole("heading", { name: "Robin" });

    expect(screen.getByText("film")).toBeInTheDocument();
    expect(
      screen.getByRole("button", { name: "photography" }),
    ).toBeInTheDocument();
  });

  it("adds a tapped suggested tag and shows Saved", async () => {
    const user = userEvent.setup();
    mockedGetCurrentMemberProfile.mockResolvedValue({
      outcome: "loaded",
      profile: PROFILE,
    });
    mockedGetSuggestedTags.mockResolvedValue(SUGGESTED);
    mockedAddMemberProfileTag.mockResolvedValue({
      outcome: "added",
      tag: { id: "tag-3", name: "football" },
    });
    renderPage();

    await screen.findByRole("heading", { name: "Robin" });
    await user.click(screen.getByRole("button", { name: "football" }));

    expect(mockedAddMemberProfileTag).toHaveBeenCalledWith("football");
    expect(await screen.findByRole("status")).toHaveTextContent("Saved");
    expect(screen.getByRole("button", { name: "football" })).toHaveAttribute(
      "aria-pressed",
      "true",
    );
  });

  it("shows the field message below Add your own on 400", async () => {
    const user = userEvent.setup();
    mockedGetCurrentMemberProfile.mockResolvedValue({
      outcome: "loaded",
      profile: PROFILE,
    });
    mockedGetSuggestedTags.mockResolvedValue(SUGGESTED);
    mockedAddMemberProfileTag.mockResolvedValue({
      outcome: "fieldError",
      message: "A tag has at most 30 characters",
    });
    renderPage();

    await screen.findByRole("heading", { name: "Robin" });
    await user.type(screen.getByLabelText("Add your own"), "x".repeat(31));
    await user.click(screen.getByRole("button", { name: "Add" }));

    expect(
      await screen.findByText("A tag has at most 30 characters"),
    ).toBeInTheDocument();
    expect(screen.getByLabelText("Add your own")).toHaveValue("x".repeat(31));
  });

  it("shows Fill in a tag below Add your own when Add is pressed with an empty field", async () => {
    const user = userEvent.setup();
    mockedGetCurrentMemberProfile.mockResolvedValue({
      outcome: "loaded",
      profile: PROFILE,
    });
    mockedGetSuggestedTags.mockResolvedValue(SUGGESTED);
    mockedAddMemberProfileTag.mockResolvedValue({
      outcome: "fieldError",
      message: "Fill in a tag",
    });
    renderPage();

    await screen.findByRole("heading", { name: "Robin" });
    await user.click(screen.getByRole("button", { name: "Add" }));

    expect(await screen.findByText("Fill in a tag")).toBeInTheDocument();
    expect(mockedAddMemberProfileTag).toHaveBeenCalledWith("");
  });

  it("goes to Create your profile when adding a tag finds no profile", async () => {
    const user = userEvent.setup();
    mockedGetCurrentMemberProfile.mockResolvedValue({
      outcome: "loaded",
      profile: PROFILE,
    });
    mockedGetSuggestedTags.mockResolvedValue(SUGGESTED);
    mockedAddMemberProfileTag.mockResolvedValue({ outcome: "noProfile" });
    renderPage();

    await screen.findByRole("heading", { name: "Robin" });
    await user.click(screen.getByRole("button", { name: "football" }));

    expect(await screen.findByText("Create your profile")).toBeInTheDocument();
  });

  it("shows the tags are kept when adding fails because Connect is not reachable", async () => {
    const user = userEvent.setup();
    mockedGetCurrentMemberProfile.mockResolvedValue({
      outcome: "loaded",
      profile: PROFILE,
    });
    mockedGetSuggestedTags.mockResolvedValue(SUGGESTED);
    mockedAddMemberProfileTag.mockResolvedValue({ outcome: "notReachable" });
    renderPage();

    await screen.findByRole("heading", { name: "Robin" });
    await user.click(screen.getByRole("button", { name: "football" }));

    expect(await screen.findByRole("alert")).toHaveTextContent(
      "Connect is not reachable right now, your tags are kept",
    );
    expect(screen.getByRole("button", { name: "football" })).toHaveAttribute(
      "aria-pressed",
      "false",
    );
  });

  it("has no accessibility problems on the filled profile", async () => {
    mockedGetCurrentMemberProfile.mockResolvedValue({
      outcome: "loaded",
      profile: PROFILE,
    });
    mockedGetSuggestedTags.mockResolvedValue(SUGGESTED);
    const { container } = renderPage();

    await screen.findByRole("heading", { name: "Robin" });

    expect(await findAxeViolations(container)).toEqual([]);
  });
});
