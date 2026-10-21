import { describe, expect, it, vi } from "vitest";
import { NetworkError, RequestError, request } from "./client";

function response(status: number, body: unknown) {
  return new Response(JSON.stringify(body), {
    status,
    headers: { "Content-Type": "application/json" },
  });
}

describe("request", () => {
  it.each([201, 400, 409, 500, 502, 503])(
    "returns the body for a %s response",
    async (status) => {
      vi.stubGlobal("fetch", vi.fn().mockResolvedValue(response(status, { status })));

      if (status === 201) {
        await expect(request("/api/accounts")).resolves.toEqual({ status });
      } else {
        await expect(request("/api/accounts")).rejects.toEqual(
          expect.objectContaining({ status, body: { status } }),
        );
      }
    },
  );

  it("turns a failed fetch into a NetworkError", async () => {
    vi.stubGlobal("fetch", vi.fn().mockRejectedValue(new TypeError("offline")));

    await expect(request("/api/accounts")).rejects.toBeInstanceOf(NetworkError);
  });

  it("exposes the status and body on an HTTP failure", async () => {
    vi.stubGlobal("fetch", vi.fn().mockResolvedValue(response(400, { errors: {} })));

    await expect(request("/api/accounts")).rejects.toBeInstanceOf(RequestError);
  });
});
