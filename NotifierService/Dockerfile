#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src
COPY ["NotifierService/NotifierService.csproj", "NotifierService/"]
COPY ["NotifierInfrastructure/NotifierInfrastructure.csproj", "NotifierInfrastructure/"]
COPY ["NotifierEntities/NotifierEntities.csproj", "NotifierEntities/"]
COPY ["SharedModule/SharedModule.csproj", "SharedModule/"]
COPY ["NotifierDomain/NotifierDomain.csproj", "NotifierDomain/"]
RUN dotnet restore "NotifierService/NotifierService.csproj"
COPY . .
WORKDIR "/src/NotifierService"
RUN dotnet build "NotifierService.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "NotifierService.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "NotifierService.dll"]
