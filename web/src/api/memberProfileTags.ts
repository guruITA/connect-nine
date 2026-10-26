// Calls POST /api/member-profiles/current/tags of #69 to add a tag to the own profile.

import { request } from "./http";
import type { Tag } from "./tags";

export type AddMemberProfileTagResult =
  | { outcome: "added"; tag: Tag }
  | { outcome: "fieldError"; message: string }
  | { outcome: "notLoggedIn" }
  | { outcome: "noProfile" }
  | { outcome: "notReachable" };

type AddMemberProfileTagResponseBody = {
  data: Tag;
};

type ValidationProblemBody = {
  errors?: Record<string, string[]>;
};

export async function addMemberProfileTag(
  name: string,
): Promise<AddMemberProfileTagResult> {
  let response: Response;
  try {
    response = await request("/api/member-profiles/current/tags", {
      method: "POST",
      headers: { "Content-Type": "application/json" },
      body: JSON.stringify({ name }),
    });
  } catch {
    return { outcome: "notReachable" };
  }

  try {
    if (response.status === 201 || response.status === 200) {
      const body = (await response.json()) as AddMemberProfileTagResponseBody;
      return { outcome: "added", tag: body.data };
    }

    if (response.status === 400) {
      const body = (await response.json()) as ValidationProblemBody;
      const messages = body.errors?.name ?? [];
      return { outcome: "fieldError", message: messages[0] ?? "Fill in a tag" };
    }
  } catch {
    // An answer that is not the JSON the server promised counts as not reachable.
    return { outcome: "notReachable" };
  }

  if (response.status === 401) {
    return { outcome: "notLoggedIn" };
  }

  if (response.status === 404) {
    return { outcome: "noProfile" };
  }

  // 503 and every other status the server might send are all treated as not reachable.
  return { outcome: "notReachable" };
}
