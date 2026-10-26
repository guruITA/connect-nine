// Temporary until log in (#15): the server only accepts the header X-Temporary-Account-Id in
// Development. Every call of src/api that needs an account passes its options through here, so
// the header is added in one place and goes with the log in.
export function withTemporaryAccount(init: RequestInit = {}): RequestInit {
  const headers: Record<string, string> = {
    ...(init.headers as Record<string, string> | undefined),
  };

  const temporaryAccountId = import.meta.env.VITE_TEMPORARY_ACCOUNT_ID;
  if (typeof temporaryAccountId === "string" && temporaryAccountId.length > 0) {
    headers["X-Temporary-Account-Id"] = temporaryAccountId;
  }

  return { ...init, headers, credentials: "include" };
}
