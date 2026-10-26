import { afterEach, describe, expect, it, vi } from "vitest";
import { createMemberProfile, getCurrentMemberProfile } from "./memberProfiles";

function mockFetchOnce(status: number, body?: unknown) {
  return vi.spyOn(globalThis, "fetch").mockResolvedValueOnce({
    ok: status >= 200 && status < 300,
    status,
    json: async () => body,
  } as Response);
}

const FORM = {
  firstName: "Robin",
  ageBand: "Adolescent" as const,
  region: "Amstelveen",
  bio: "Hello there",
};

describe("createMemberProfile", () => {
  afterEach(() => {
    vi.unstubAllEnvs();
  });

  it("posts the four fields as JSON with the session cookie included and no temporary header by default", async () => {
    vi.stubEnv("VITE_TEMPORARY_ACCOUNT_ID", "");
    const fetchSpy = mockFetchOnce(201, {
      data: { id: "profile-1" },
      meta: {},
    });

    await createMemberProfile(FORM);

    expect(fetchSpy).toHaveBeenCalledWith("/api/member-profiles", {
      method: "POST",
      headers: { "Content-Type": "application/json" },
      credentials: "include",
      body: JSON.stringify(FORM),
    });
  });

  it("adds the temporary account id header when it is set", async () => {
    vi.stubEnv("VITE_TEMPORARY_ACCOUNT_ID", "account-9");
    const fetchSpy = mockFetchOnce(201, {
      data: { id: "profile-1" },
      meta: {},
    });

    await createMemberProfile(FORM);

    expect(fetchSpy).toHaveBeenCalledWith("/api/member-profiles", {
      method: "POST",
      headers: {
        "Content-Type": "application/json",
        "X-Temporary-Account-Id": "account-9",
      },
      credentials: "include",
      body: JSON.stringify(FORM),
    });
  });

  it("reports saved with the id on 201", async () => {
    mockFetchOnce(201, { data: { id: "profile-1" }, meta: {} });

    const result = await createMemberProfile(FORM);

    expect(result).toEqual({ outcome: "saved", id: "profile-1" });
  });

  it("reports the first message of every field on 400", async () => {
    mockFetchOnce(400, {
      title: "One or more validation errors occurred.",
      status: 400,
      errors: {
        firstName: ["Fill in your first name"],
        region: [
          "Fill in only your town or area, not your street, house number or postcode",
          "A second message the UI should ignore",
        ],
      },
    });

    const result = await createMemberProfile(FORM);

    expect(result).toEqual({
      outcome: "fieldErrors",
      fieldErrors: {
        firstName: "Fill in your first name",
        region:
          "Fill in only your town or area, not your street, house number or postcode",
      },
    });
  });

  it("reports not logged in on 401", async () => {
    mockFetchOnce(401);

    const result = await createMemberProfile(FORM);

    expect(result).toEqual({ outcome: "notLoggedIn" });
  });

  it("reports already has a profile on 409 with the title of the server", async () => {
    mockFetchOnce(409, {
      title: "This account already belongs to an organisation",
      status: 409,
    });

    const result = await createMemberProfile(FORM);

    expect(result).toEqual({
      outcome: "alreadyHasProfile",
      message: "This account already belongs to an organisation",
    });
  });

  it("falls back to You already have a profile on a 409 without a title", async () => {
    mockFetchOnce(409, {});

    const result = await createMemberProfile(FORM);

    expect(result).toEqual({
      outcome: "alreadyHasProfile",
      message: "You already have a profile",
    });
  });

  it("reports not reachable on 503", async () => {
    mockFetchOnce(503, {
      title: "Connect is not reachable right now",
      status: 503,
    });

    const result = await createMemberProfile(FORM);

    expect(result).toEqual({ outcome: "notReachable" });
  });

  it("reports not reachable on any other status", async () => {
    mockFetchOnce(500, { title: "Something else", status: 500 });

    const result = await createMemberProfile(FORM);

    expect(result).toEqual({ outcome: "notReachable" });
  });

  it("reports not reachable when the fetch call itself fails", async () => {
    vi.spyOn(globalThis, "fetch").mockRejectedValueOnce(
      new Error("network down"),
    );

    const result = await createMemberProfile(FORM);

    expect(result).toEqual({ outcome: "notReachable" });
  });

  it("reports not reachable when a 201 answer is not JSON", async () => {
    vi.spyOn(globalThis, "fetch").mockResolvedValueOnce({
      ok: true,
      status: 201,
      json: async () => {
        throw new SyntaxError("Unexpected end of JSON input");
      },
    } as unknown as Response);

    const result = await createMemberProfile(FORM);

    expect(result).toEqual({ outcome: "notReachable" });
  });
});

const PROFILE = {
  firstName: "Sam",
  ageBand: "YoungAdult" as const,
  region: "Amstelveen",
  bio: null,
};

describe("getCurrentMemberProfile", () => {
  it("asks GET /api/member-profiles/current with the session cookie included", async () => {
    const fetchSpy = mockFetchOnce(200, { data: PROFILE, meta: {} });

    await getCurrentMemberProfile();

    expect(fetchSpy).toHaveBeenCalledWith("/api/member-profiles/current", {
      headers: {},
      credentials: "include",
    });
  });

  it("reports the profile on 200", async () => {
    mockFetchOnce(200, { data: PROFILE, meta: {} });

    const result = await getCurrentMemberProfile();

    expect(result).toEqual({ outcome: "loaded", profile: PROFILE });
  });

  it("reports no profile on 404", async () => {
    mockFetchOnce(404, { title: "You have no profile yet", status: 404 });

    const result = await getCurrentMemberProfile();

    expect(result).toEqual({ outcome: "noProfile" });
  });

  it("reports not logged in on 401", async () => {
    mockFetchOnce(401);

    const result = await getCurrentMemberProfile();

    expect(result).toEqual({ outcome: "notLoggedIn" });
  });

  it("reports not reachable on 503", async () => {
    mockFetchOnce(503, {
      title: "Connect is not reachable right now",
      status: 503,
    });

    const result = await getCurrentMemberProfile();

    expect(result).toEqual({ outcome: "notReachable" });
  });

  it("reports not reachable on any other status", async () => {
    mockFetchOnce(500, { title: "Something else", status: 500 });

    const result = await getCurrentMemberProfile();

    expect(result).toEqual({ outcome: "notReachable" });
  });

  it("reports not reachable when the fetch call itself fails", async () => {
    vi.spyOn(globalThis, "fetch").mockRejectedValueOnce(
      new Error("network down"),
    );

    const result = await getCurrentMemberProfile();

    expect(result).toEqual({ outcome: "notReachable" });
  });

  it("reports not reachable when a 200 answer is not JSON", async () => {
    vi.spyOn(globalThis, "fetch").mockResolvedValueOnce({
      ok: true,
      status: 200,
      json: async () => {
        throw new SyntaxError("Unexpected end of JSON input");
      },
    } as unknown as Response);

    const result = await getCurrentMemberProfile();

    expect(result).toEqual({ outcome: "notReachable" });
  });
});
