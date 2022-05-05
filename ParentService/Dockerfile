#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src
COPY ["ParentService/ParentService.csproj", "ParentService/"]
COPY ["ParentInfrastructure/ParentInfrastructure.csproj", "ParentInfrastructure/"]
COPY ["SharedModule/SharedModule.csproj", "SharedModule/"]
COPY ["ParentDomain/ParentDomain.csproj", "ParentDomain/"]
RUN dotnet restore "ParentService/ParentService.csproj"
COPY . .
WORKDIR "/src/ParentService"
RUN dotnet build "ParentService.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "ParentService.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "ParentService.dll"]
