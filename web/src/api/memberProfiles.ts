// Calls POST /api/member-profiles of #321 to create a member profile.
// Moves to request() of web/src/api/client.ts when #264 adds it.

export type AgeBand = "From15To17" | "From18To21" | "From22To30";

export const AGE_BAND_LABELS: Record<AgeBand, string> = {
  From15To17: "15 to 17",
  From18To21: "18 to 21",
  From22To30: "22 to 30",
};

export type CreateMemberProfileForm = {
  firstName: string;
  ageBand: AgeBand | "";
  region: string;
  bio: string;
};

export type CreateMemberProfileResult =
  | { outcome: "saved"; id: string }
  | { outcome: "fieldErrors"; fieldErrors: Record<string, string> }
  | { outcome: "alreadyHasProfile" }
  | { outcome: "notLoggedIn" }
  | { outcome: "notReachable" };

type SavedResponseBody = {
  data: { id: string };
};

type ValidationProblemBody = {
  errors?: Record<string, string[]>;
};

export async function createMemberProfile(
  form: CreateMemberProfileForm,
): Promise<CreateMemberProfileResult> {
  const headers: Record<string, string> = {
    "Content-Type": "application/json",
  };

  // Temporary until log in (#15): the server only accepts this header in Development.
  const temporaryAccountId = import.meta.env.VITE_TEMPORARY_ACCOUNT_ID;
  if (typeof temporaryAccountId === "string" && temporaryAccountId.length > 0) {
    headers["X-Temporary-Account-Id"] = temporaryAccountId;
  }

  let response: Response;
  try {
    response = await fetch("/api/member-profiles", {
      method: "POST",
      headers,
      credentials: "include",
      body: JSON.stringify({
        firstName: form.firstName,
        ageBand: form.ageBand,
        region: form.region,
        bio: form.bio,
      }),
    });
  } catch {
    return { outcome: "notReachable" };
  }

  try {
    if (response.status === 201) {
      const body = (await response.json()) as SavedResponseBody;
      return { outcome: "saved", id: body.data.id };
    }

    if (response.status === 400) {
      const body = (await response.json()) as ValidationProblemBody;
      const fieldErrors: Record<string, string> = {};
      for (const [field, messages] of Object.entries(body.errors ?? {})) {
        fieldErrors[field] = messages[0];
      }
      return { outcome: "fieldErrors", fieldErrors };
    }
  } catch {
    // An answer that is not the JSON the server promised counts as not reachable.
    return { outcome: "notReachable" };
  }

  if (response.status === 401) {
    return { outcome: "notLoggedIn" };
  }

  if (response.status === 409) {
    return { outcome: "alreadyHasProfile" };
  }

  // 503 and every other status the server might send are all treated as not reachable.
  return { outcome: "notReachable" };
}
