// Calls POST /api/member-profiles/current/tags of #69 to add a tag to the own profile, through
// request() of client.ts (frontend rule 14).
import { RequestError, request } from "./client";
import type { ValidationProblem } from "./client";
import type { Tag } from "./tags";
import { withTemporaryAccount } from "./temporaryAccount";

export type AddMemberProfileTagResult =
  | { outcome: "added"; tag: Tag }
  | { outcome: "fieldError"; message: string }
  | { outcome: "notLoggedIn" }
  | { outcome: "noProfile" }
  | { outcome: "notReachable" };

type AddMemberProfileTagResponseBody = {
  data?: Tag;
};

export async function addMemberProfileTag(
  name: string,
): Promise<AddMemberProfileTagResult> {
  try {
    const body = await request<AddMemberProfileTagResponseBody | undefined>(
      "/api/member-profiles/current/tags",
      withTemporaryAccount({
        method: "POST",
        headers: { "Content-Type": "application/json" },
        body: JSON.stringify({ name }),
      }),
    );
    // An answer that is not the JSON the server promised counts as not reachable.
    return body?.data
      ? { outcome: "added", tag: body.data }
      : { outcome: "notReachable" };
  } catch (error) {
    if (error instanceof RequestError) {
      if (error.status === 400) {
        const messages =
          (error.body as ValidationProblem | undefined)?.errors?.name ?? [];
        return {
          outcome: "fieldError",
          message: messages[0] ?? "Fill in a tag",
        };
      }
      if (error.status === 401) return { outcome: "notLoggedIn" };
      if (error.status === 404) return { outcome: "noProfile" };
    }

    // 503, every other status and a server that cannot be reached are all not reachable.
    return { outcome: "notReachable" };
  }
}
