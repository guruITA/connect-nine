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
