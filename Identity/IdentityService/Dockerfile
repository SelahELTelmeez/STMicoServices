#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src
COPY ["AuthenticationService/IdentityService.csproj", "AuthenticationService/"]
COPY ["IdentityInfrastructure/IdentityInfrastructure.csproj", "IdentityInfrastructure/"]
COPY ["IdentityEntities/IdentityEntities.csproj", "IdentityEntities/"]
COPY ["SharedModule/SharedModule.csproj", "SharedModule/"]
COPY ["IdentityDomain/IdentityDomain.csproj", "IdentityDomain/"]
RUN dotnet restore "AuthenticationService/IdentityService.csproj"
COPY . .
WORKDIR "/src/AuthenticationService"
RUN dotnet build "IdentityService.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "IdentityService.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "IdentityService.dll"]
