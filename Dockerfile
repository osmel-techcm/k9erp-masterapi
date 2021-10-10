#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:5.0-buster-slim AS base
RUN apt-get update && apt-get install -y libgdiplus
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:5.0-buster-slim AS build
WORKDIR /src
COPY ["masterApi/masterApi.csproj", "masterApi/"]
COPY ["masterCore/masterCore.csproj", "masterCore/"]
COPY ["masterInfrastructure/masterInfrastructure.csproj", "masterInfrastructure/"]
RUN dotnet restore "masterApi/masterApi.csproj"
COPY . .
WORKDIR "/src/masterApi"
RUN dotnet build "masterApi.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "masterApi.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "masterApi.dll"]
