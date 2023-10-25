FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY ["Organization/Organization.csproj", "Organization/"]
RUN dotnet restore "Organization/Organization.csproj"
COPY . .
WORKDIR "/src/Organization"
RUN dotnet build "Organization.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Organization.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Organization.dll"]
