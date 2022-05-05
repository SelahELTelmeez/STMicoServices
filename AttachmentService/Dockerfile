#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src
COPY ["AttachmentService/AttachmentService.csproj", "AttachmentService/"]
COPY ["AttachmentInfrastructure/AttachmentInfrastructure.csproj", "AttachmentInfrastructure/"]
COPY ["AttachmentEntity/AttachmentEntity.csproj", "AttachmentEntity/"]
COPY ["SharedModule/SharedModule.csproj", "SharedModule/"]
COPY ["AttachmentDomain/AttachmentDomain.csproj", "AttachmentDomain/"]
RUN dotnet restore "AttachmentService/AttachmentService.csproj"
COPY . .
WORKDIR "/src/AttachmentService"
RUN dotnet build "AttachmentService.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "AttachmentService.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "AttachmentService.dll"]
