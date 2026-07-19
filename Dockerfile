FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

COPY backend/SupportFlow.Domain/SupportFlow.Domain.csproj backend/SupportFlow.Domain/
COPY backend/SupportFlow.Application/SupportFlow.Application.csproj backend/SupportFlow.Application/
COPY backend/SupportFlow.Infrastructure/SupportFlow.Infrastructure.csproj backend/SupportFlow.Infrastructure/
COPY backend/SupportFlow.Api/SupportFlow.Api.csproj backend/SupportFlow.Api/

RUN dotnet restore backend/SupportFlow.Api/SupportFlow.Api.csproj

COPY backend/ backend/

RUN dotnet publish backend/SupportFlow.Api/SupportFlow.Api.csproj \
    -c Release \
    -o /app/publish \
    /p:UseAppHost=false

FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS final
WORKDIR /app

ENV ASPNETCORE_URLS=http://+:8080
EXPOSE 8080

COPY --from=build /app/publish .

ENTRYPOINT ["dotnet", "SupportFlow.Api.dll"]