# ===================== Build stage =====================
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Копируем файлы проектов отдельно, чтобы кешировался restore
COPY InstallmentCRM.API/*.csproj InstallmentCRM.API/
COPY InstallmentCRM.Application/*.csproj InstallmentCRM.Application/
COPY InstallmentCRM.Domain/*.csproj InstallmentCRM.Domain/
COPY InstallmentCRM.Infrastructure/*.csproj InstallmentCRM.Infrastructure/
COPY InstallmentCRM.Persistence/*.csproj InstallmentCRM.Persistence/
COPY InstallmentCRM.Shared/*.csproj InstallmentCRM.Shared/

RUN dotnet restore InstallmentCRM.API/InstallmentCRM.API.csproj

# Копируем весь остальной код и публикуем
COPY . .
RUN dotnet publish InstallmentCRM.API/InstallmentCRM.API.csproj -c Release -o /app --no-restore

# ===================== Runtime stage =====================
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS runtime
WORKDIR /app
COPY --from=build /app .

ENV ASPNETCORE_ENVIRONMENT=Production
EXPOSE 8080

ENTRYPOINT ["dotnet", "InstallmentCRM.API.dll"]
