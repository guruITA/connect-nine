import { NetworkError, request, RequestError } from "./client";

export type CreateAccountResult =
  | { kind: "made" }
  | { kind: "invalid"; fields: { email?: string; password?: string } }
  | { kind: "taken" }
  | { kind: "not reachable" };

type ValidationProblem = {
  errors?: Record<string, string[]>;
};

function firstMessage(messages: string[] | undefined): string | undefined {
  return messages?.[0];
}

function invalidResult(body: unknown): CreateAccountResult {
  const problem = body as ValidationProblem;
  const errors = problem?.errors ?? {};

  return {
    kind: "invalid",
    fields: {
      email: firstMessage(errors.Email ?? errors.email),
      password: firstMessage(errors.Password ?? errors.password),
    },
  };
}

export async function createAccount(
  email: string,
  password: string,
): Promise<CreateAccountResult> {
  try {
    await request("/api/accounts", {
      method: "POST",
      headers: { "Content-Type": "application/json" },
      body: JSON.stringify({ email, password }),
    });

    return { kind: "made" };
  } catch (error) {
    if (error instanceof NetworkError) {
      return { kind: "not reachable" };
    }

    if (error instanceof RequestError) {
      if (error.status === 400) return invalidResult(error.body);
      if (error.status === 409) return { kind: "taken" };
      if (error.status >= 500) return { kind: "not reachable" };
    }

    return { kind: "not reachable" };
  }
}
