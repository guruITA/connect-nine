# API

## `GET /api/health`

Returns HTTP `200 OK` with the exact JSON body:

```json
{"status":"healthy"}
```

The endpoint is anonymous. It does not query the database or include version,
error, or build information.
