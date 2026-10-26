// Calls GET /api/tags/suggested, reused wherever a member picks tags (#70, #110), through
// request() of client.ts (frontend rule 14).
import { request } from "./client";
import { withTemporaryAccount } from "./temporaryAccount";

export type Tag = {
  id: string;
  name: string;
};

type SuggestedTagsResponseBody = {
  data?: Tag[];
};

// A suggestion list that fails to load is left empty, so the page it is on never breaks.
export async function getSuggestedTags(): Promise<Tag[]> {
  try {
    const body = await request<SuggestedTagsResponseBody | undefined>(
      "/api/tags/suggested?page=1&pageSize=50",
      withTemporaryAccount(),
    );
    return body?.data ?? [];
  } catch {
    return [];
  }
}
