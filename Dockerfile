# The images of Connect (task 20.1, #352). Build context is the root of the repository.
# The pipeline builds them with kaniko (task 20.3, #354, decision 008), a laptop with
# docker build --target server . or --target migrations .
# The versions follow docs/team/onboarding.md and the pipeline: Node.js 22 and .NET 10.

# frontend-build: the screen, built once into web/dist.
FROM node:22-alpine AS frontend-build
WORKDIR /src/web
COPY web/package.json web/package-lock.json ./
RUN npm ci
COPY web/ ./
RUN npm run build

# server-build: the server published to /app/publish and the migrations bundled into /app/efbundle,
# the same steps as deploy-test of #267. A new project gets one COPY line for its .csproj below.
FROM mcr.microsoft.com/dotnet/sdk:10.0 AS server-build
WORKDIR /src
COPY .config/dotnet-tools.json .config/
COPY api/Connect.slnx api/
COPY api/Connect/Connect.csproj api/Connect/
COPY api/Connect.Presentation/Connect.Presentation.csproj api/Connect.Presentation/
COPY api/Connect.Application/Connect.Application.csproj api/Connect.Application/
COPY api/Connect.Domain/Connect.Domain.csproj api/Connect.Domain/
COPY api/Connect.Infrastructure/Connect.Infrastructure.csproj api/Connect.Infrastructure/
COPY api/Directory.Build.props api/
RUN dotnet restore api/Connect/Connect.csproj && dotnet tool restore
COPY api/ api/
RUN dotnet publish api/Connect/Connect.csproj --configuration Release --no-restore --output /app/publish
# dotnet ef only makes the DbContext while it builds the bundle and connects with nothing, so this
# value is not a secret and never reaches an image: it lives for this one command.
RUN ConnectionStrings__DefaultConnection="Host=bundle;Database=bundle" \
    dotnet ef migrations bundle --project api/Connect.Infrastructure --startup-project api/Connect \
    --configuration Release --self-contained --target-runtime linux-x64 --output /app/efbundle --force

# migrations: only the bundle. It brings the tables up to date once and stops. The bundle carries its
# own .NET, so it needs only runtime-deps, 160 MB smaller than aspnet (measured 22 October 2026).
FROM mcr.microsoft.com/dotnet/runtime-deps:10.0 AS migrations
WORKDIR /app
COPY --from=server-build /app/efbundle ./efbundle
USER $APP_UID
# The connection string comes in when the container starts, never from the image.
CMD ./efbundle --connection "$ConnectionStrings__DefaultConnection"

# server: the server with the screen in wwwroot, where UseStaticFiles() of #267 finds it. The default.
FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS server
WORKDIR /app
RUN test -f /usr/share/zoneinfo/Europe/Amsterdam \
    || (apt-get update && apt-get install -y --no-install-recommends tzdata && rm -rf /var/lib/apt/lists/*)
COPY --from=server-build /app/publish ./
COPY --from=frontend-build /src/web/dist ./wwwroot
ENV ASPNETCORE_HTTP_PORTS=8080
EXPOSE 8080
USER $APP_UID
ENTRYPOINT ["dotnet", "Connect.dll"]
