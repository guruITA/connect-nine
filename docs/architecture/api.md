# API

## `GET /api/health`

Returns HTTP `200 OK` with the exact JSON body:

```json
{"status":"healthy"}
```

The endpoint is anonymous. It does not query the database or include version,
error, or build information.

## `POST /api/accounts`

Creates an account from a JSON request body:

```json
{"email":"user@example.com","password":"example-password"}
```

The endpoint has no explicit authorization requirement. It is anonymous unless
an application-wide authorization policy requires authentication.

Returns:

- HTTP `201 Created` with the account response produced by
  `CreateAccountResponseMapper` and a `Location` header of `/api/accounts/{id}`.
- HTTP `400 Bad Request` when validation fails. Request validation returns
  validation Problem Details with field errors; command validation returns
  Problem Details with the error description as its title.
- HTTP `409 Conflict` when the command reports a conflict, with Problem Details
  containing the error description as its title.
- HTTP `503 Service Unavailable` when an exception reaching the exception-handling
  middleware is, or contains, a database exception (`DbException`). Returns
  Problem Details titled `Connect is not reachable right now`.
- HTTP `500 Internal Server Error` for other mapped errors or unhandled exceptions,
  with Problem Details titled `An unexpected error occurred.`
