// The body of a 400 answer of the server: the messages of each wrong field, under the key of
// that field (backend rule 19). Every file in src/api reads it through this one type.
export type ValidationProblem = {
  errors?: Record<string, string[]>;
};

export class RequestError extends Error {
  public readonly status: number;
  public readonly body: unknown;

  constructor(status: number, body: unknown) {
    super(`Request failed with status ${status}`);
    this.name = "RequestError";
    this.status = status;
    this.body = body;
  }
}

export class NetworkError extends Error {
  constructor() {
    super("The request could not reach Connect");
    this.name = "NetworkError";
  }
}

export async function request<T>(
  input: RequestInfo | URL,
  init?: RequestInit,
): Promise<T> {
  let response: Response;

  try {
    response = await fetch(input, init);
  } catch {
    throw new NetworkError();
  }

  const body = await response.json().catch(() => undefined);

  if (!response.ok) {
    throw new RequestError(response.status, body);
  }

  return body as T;
}
