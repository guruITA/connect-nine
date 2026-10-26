import { describe, expect, it, vi } from "vitest";
import { getSuggestedTags } from "./tags";

const TAGS = [
  { id: "tag-1", name: "coding" },
  { id: "tag-2", name: "football" },
];

describe("getSuggestedTags", () => {
  it("asks page 1 with pageSize 50", async () => {
    const fetchSpy = vi.spyOn(globalThis, "fetch").mockResolvedValueOnce({
      status: 200,
      json: async () => ({
        data: TAGS,
        meta: { page: 1, pageSize: 50, total: 2 },
      }),
    } as Response);

    await getSuggestedTags();

    expect(fetchSpy).toHaveBeenCalledWith(
      "/api/tags/suggested?page=1&pageSize=50",
      {
        headers: {},
        credentials: "include",
      },
    );
  });

  it("returns the tags on 200", async () => {
    vi.spyOn(globalThis, "fetch").mockResolvedValueOnce({
      status: 200,
      json: async () => ({ data: TAGS, meta: {} }),
    } as Response);

    const tags = await getSuggestedTags();

    expect(tags).toEqual(TAGS);
  });

  it("returns an empty list on any status other than 200", async () => {
    vi.spyOn(globalThis, "fetch").mockResolvedValueOnce({
      status: 503,
      json: async () => ({}),
    } as Response);

    const tags = await getSuggestedTags();

    expect(tags).toEqual([]);
  });

  it("returns an empty list when the fetch call itself fails", async () => {
    vi.spyOn(globalThis, "fetch").mockRejectedValueOnce(
      new Error("network down"),
    );

    const tags = await getSuggestedTags();

    expect(tags).toEqual([]);
  });

  it("returns an empty list when a 200 answer is not JSON", async () => {
    vi.spyOn(globalThis, "fetch").mockResolvedValueOnce({
      status: 200,
      json: async () => {
        throw new SyntaxError("Unexpected end of JSON input");
      },
    } as unknown as Response);

    const tags = await getSuggestedTags();

    expect(tags).toEqual([]);
  });
});
