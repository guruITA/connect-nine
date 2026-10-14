# Configuration and secrets

This document explains where configuration belongs in Connect. The rule is simple:
shared settings are committed, while values that identify or authenticate a
specific environment stay outside Git.

## Server configuration

The .NET server uses the standard `IConfiguration` pipeline. The application
reads configuration through `IConfiguration`; server code should not open
`appsettings` files directly.

| Location | Commit to Git? | Use it for |
|---|---|---|
| `appsettings.json` | Yes | Shared, non-secret defaults for every environment |
| `appsettings.Development.json` | Yes | Local development settings that are safe to share |
| .NET User Secrets | No | Local passwords, API keys and connection strings |
| `appsettings.Production.json` | Yes | Production settings that are safe to share |
| Kubernetes Secrets | No | Production passwords, API keys and connection strings |

Later configuration sources override earlier values. This means an environment
specific value can replace the shared value from `appsettings.json` without
changing the committed file.

### Local development

Use .NET User Secrets for values that must remain private on a developer's
machine:

```powershell
dotnet user-secrets init
dotnet user-secrets set "ConnectionStrings:DefaultConnection" "<local connection string>"
dotnet user-secrets list
```

User Secrets are stored outside the repository and are only loaded for the
Development environment. They are not encrypted, so protect access to the
development machine as well.

### Production

Production secrets are stored as Kubernetes Secrets and exposed to the server
as environment variables. Nested .NET configuration keys use double
underscores:

```text
ConnectionStrings__DefaultConnection
```

The environment variable above maps to the .NET key
`ConnectionStrings:DefaultConnection`. Do not put production credentials in
`appsettings.Production.json`, manifests committed to Git, logs or Terraform
state. Base64 encoding in a Kubernetes Secret is not encryption; access to the
production namespace must be limited appropriately.

## Frontend configuration

Frontend environment variables are loaded from `.env`. The file is ignored by
Git and is for local development only. The committed `.env.example` lists every
required variable with a placeholder instead of a real value:

```text
.env          # local values; never commit
.env.example  # variable names and placeholders; commit this
```

When adding a frontend variable, add its name to `.env.example` in the same
change. Frontend variables must be treated as public: the bundler includes
them in JavaScript that is downloaded by every visitor. API URLs and feature
flags are suitable frontend settings; private credentials belong in the
server's configuration instead.

## Quick decision guide

| If the value is... | Put it in... |
|---|---|
| Shared and non-secret | `appsettings.json` |
| Safe to share only for local development | `appsettings.Development.json` |
| A local server secret | .NET User Secrets |
| Safe to share only for production | `appsettings.Production.json` |
| A production server secret | Kubernetes Secrets |
| A public frontend setting | `.env`, documented in `.env.example` |

Never commit a real password, token, API key or credential-bearing connection
string.
