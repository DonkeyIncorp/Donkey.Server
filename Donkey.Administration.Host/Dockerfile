#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src
COPY ["Donkey.Administration.Host/Donkey.Administration.Host.csproj", "Donkey.Administration.Host/"]
COPY ["Donkey.Administration.BusinessLogic/Donkey.Administration.BusinessLogic.csproj", "Donkey.Administration.BusinessLogic/"]
COPY ["Donkey.Administration.Data/Donkey.Administration.Data.csproj", "Donkey.Administration.Data/"]
COPY ["Donkey.Administration.Environment/Donkey.Administration.Environment.csproj", "Donkey.Administration.Environment/"]
COPY ["Donkey.Administration.WebApi/Donkey.Administration.WebApi.csproj", "Donkey.Administration.WebApi/"]
RUN dotnet restore "Donkey.Administration.Host/Donkey.Administration.Host.csproj"
COPY . .
WORKDIR "/src/Donkey.Administration.Host"
RUN dotnet build "Donkey.Administration.Host.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Donkey.Administration.Host.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Donkey.Administration.Host.dll"]