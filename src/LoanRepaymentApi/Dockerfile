﻿FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY ["LoanRepaymentApi.csproj", "."]
RUN dotnet restore "LoanRepaymentApi.csproj"
COPY . .
WORKDIR "/src"
RUN dotnet build "LoanRepaymentApi.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "LoanRepaymentApi.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .

ENTRYPOINT ["dotnet", "LoanRepaymentApi.dll"]
