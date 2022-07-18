#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src
COPY ["TeacherService/TeacherService.csproj", "TeacherService/"]
COPY ["TeacherInfrastructure/TeacherInfrastructure.csproj", "TeacherInfrastructure/"]
COPY ["TeacherEntities/TeacherEntities.csproj", "TeacherEntities/"]
COPY ["SharedModule/SharedModule.csproj", "SharedModule/"]
COPY ["TeacherDomain/TeacherDomain.csproj", "TeacherDomain/"]
RUN dotnet restore "TeacherService/TeacherService.csproj"
COPY . .
WORKDIR "/src/TeacherService"
RUN dotnet build "TeacherService.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "TeacherService.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "TeacherService.dll"]
