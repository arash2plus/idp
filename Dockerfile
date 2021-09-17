#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:3.1 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:3.1 AS build
WORKDIR /src
COPY ["Gaia.IdP.IdentityServer/Gaia.IdP.IdentityServer.csproj", "Gaia.IdP.IdentityServer/"]
COPY ["Gaia.IdP.Infrastructure/Gaia.IdP.Infrastructure.csproj", "Gaia.IdP.Infrastructure/"]
COPY ["Gaia.IdP.Message/Gaia.IdP.Message.csproj", "Gaia.IdP.Message/"]
COPY ["Gaia.IdP.Data/Gaia.IdP.Data.csproj", "Gaia.IdP.Data/"]
COPY ["Gaia.IdP.DomainModel/Gaia.IdP.DomainModel.csproj", "Gaia.IdP.DomainModel/"]
RUN dotnet restore "Gaia.IdP.IdentityServer/Gaia.IdP.IdentityServer.csproj"
COPY . .
WORKDIR "/src/Gaia.IdP.IdentityServer"
RUN dotnet build "Gaia.IdP.IdentityServer.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Gaia.IdP.IdentityServer.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Gaia.IdP.IdentityServer.dll"]