// Calls POST /api/member-profiles of #321 to create a member profile, through request() of
// client.ts like every call to the server (frontend rule 14).
import { NetworkError, request, RequestError } from "./client";
import type { ValidationProblem } from "./client";

export type AgeBand = "Adolescent" | "YoungAdult" | "Adult";

export const AGE_BAND_LABELS: Record<AgeBand, string> = {
  Adolescent: "15 to 17",
  YoungAdult: "18 to 21",
  Adult: "22 to 30",
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
  | { outcome: "alreadyHasProfile"; message: string }
  | { outcome: "notLoggedIn" }
  | { outcome: "notReachable" };

type SavedResponseBody = {
  data?: { id?: string };
};

type ProblemBody = {
  title?: string;
};

function fieldErrorsOf(body: unknown): Record<string, string> {
  const fieldErrors: Record<string, string> = {};
  const errors = (body as ValidationProblem | undefined)?.errors ?? {};
  for (const [field, messages] of Object.entries(errors)) {
    if (messages[0]) fieldErrors[field] = messages[0];
  }
  return fieldErrors;
}

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

  try {
    const body = await request<SavedResponseBody | undefined>(
      "/api/member-profiles",
      {
        method: "POST",
        headers,
        credentials: "include",
        body: JSON.stringify({
          firstName: form.firstName,
          ageBand: form.ageBand,
          region: form.region,
          bio: form.bio,
        }),
      },
    );

    // An answer that is not the JSON the server promised counts as not reachable.
    const id = body?.data?.id;
    return id ? { outcome: "saved", id } : { outcome: "notReachable" };
  } catch (error) {
    if (error instanceof NetworkError) {
      return { outcome: "notReachable" };
    }

    if (error instanceof RequestError) {
      if (error.status === 400) {
        return {
          outcome: "fieldErrors",
          fieldErrors: fieldErrorsOf(error.body),
        };
      }
      if (error.status === 401) return { outcome: "notLoggedIn" };
      if (error.status === 409) {
        // The server names the conflict in the title, for example an account that already
        // belongs to an organisation (#67). Without a title it is the profile that already exists.
        const title = (error.body as ProblemBody | undefined)?.title;
        return {
          outcome: "alreadyHasProfile",
          message: title || "You already have a profile",
        };
      }
    }

    // 503 and every other status the server might send are all treated as not reachable.
    return { outcome: "notReachable" };
  }
}
