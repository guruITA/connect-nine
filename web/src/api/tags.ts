// Calls GET /api/tags/suggested, reused wherever a member picks tags (#70, #110).

import { request } from "./http";

export type Tag = {
  id: string;
  name: string;
};

type SuggestedTagsResponseBody = {
  data: Tag[];
};

// A suggestion list that fails to load is left empty, so the page it is on never breaks.
export async function getSuggestedTags(): Promise<Tag[]> {
  try {
    const response = await request("/api/tags/suggested?page=1&pageSize=50");
    if (response.status !== 200) {
      return [];
    }
    const body = (await response.json()) as SuggestedTagsResponseBody;
    return body.data;
  } catch {
    return [];
  }
}
