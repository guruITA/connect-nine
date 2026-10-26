import { afterEach, describe, expect, it, vi } from "vitest";
import { addMemberProfileTag } from "./memberProfileTags";

function mockFetchOnce(status: number, body?: unknown) {
  return vi.spyOn(globalThis, "fetch").mockResolvedValueOnce({
    ok: status >= 200 && status < 300,
    status,
    json: async () => body,
  } as Response);
}

describe("addMemberProfileTag", () => {
  afterEach(() => {
    vi.unstubAllEnvs();
  });

  it("posts the name as JSON with the session cookie included", async () => {
    const fetchSpy = mockFetchOnce(201, {
      data: { id: "tag-1", name: "drone video" },
      meta: {},
    });

    await addMemberProfileTag("Drone video");

    expect(fetchSpy).toHaveBeenCalledWith("/api/member-profiles/current/tags", {
      method: "POST",
      headers: { "Content-Type": "application/json" },
      credentials: "include",
      body: JSON.stringify({ name: "Drone video" }),
    });
  });

  it("reports the added tag on 201", async () => {
    mockFetchOnce(201, {
      data: { id: "tag-1", name: "drone video" },
      meta: {},
    });

    const result = await addMemberProfileTag("Drone video");

    expect(result).toEqual({
      outcome: "added",
      tag: { id: "tag-1", name: "drone video" },
    });
  });

  it("reports the added tag on 200 when the member already had it", async () => {
    mockFetchOnce(200, { data: { id: "tag-1", name: "football" }, meta: {} });

    const result = await addMemberProfileTag("football");

    expect(result).toEqual({
      outcome: "added",
      tag: { id: "tag-1", name: "football" },
    });
  });

  it("reports the field message on 400", async () => {
    mockFetchOnce(400, {
      title: "One or more validation errors occurred.",
      status: 400,
      errors: { name: ["Fill in a tag"] },
    });

    const result = await addMemberProfileTag("");

    expect(result).toEqual({ outcome: "fieldError", message: "Fill in a tag" });
  });

  it("reports not logged in on 401", async () => {
    mockFetchOnce(401);

    const result = await addMemberProfileTag("football");

    expect(result).toEqual({ outcome: "notLoggedIn" });
  });

  it("reports no profile on 404", async () => {
    mockFetchOnce(404, { title: "You have no profile yet", status: 404 });

    const result = await addMemberProfileTag("football");

    expect(result).toEqual({ outcome: "noProfile" });
  });

  it("reports not reachable on 503", async () => {
    mockFetchOnce(503, {
      title: "Connect is not reachable right now",
      status: 503,
    });

    const result = await addMemberProfileTag("football");

    expect(result).toEqual({ outcome: "notReachable" });
  });

  it("reports not reachable when the fetch call itself fails", async () => {
    vi.spyOn(globalThis, "fetch").mockRejectedValueOnce(
      new Error("network down"),
    );

    const result = await addMemberProfileTag("football");

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

    const result = await addMemberProfileTag("football");

    expect(result).toEqual({ outcome: "notReachable" });
  });
});
