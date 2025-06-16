FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

COPY . .

ARG SENTRY_AUTH_TOKEN
ARG GITHUB_SHA
ENV GITHUB_SHA=$GITHUB_SHA

RUN --mount=type=secret,id=SENTRY_AUTH_TOKEN \
    SENTRY_AUTH_TOKEN=$(cat /run/secrets/SENTRY_AUTH_TOKEN) \
    dotnet publish "LoanRepaymentApi.csproj" -c Release -o /app

FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app
COPY --from=build /app .

USER $APP_UID

ENTRYPOINT ["dotnet", "LoanRepaymentApi.dll"]
