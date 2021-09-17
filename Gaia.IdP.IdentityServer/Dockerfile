FROM mcr.microsoft.com/dotnet/aspnet:3.1 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443
ENV ASPNETCORE_URLS=http://+:80
ENV ASPNETCORE_ENVIRONMENT=Production

FROM mcr.microsoft.com/dotnet/sdk:3.1 AS build
ENV DOTNET_SYSTEM_NET_HTTP_USESOCKETSHTTPHANDLER=0
ENV NUGET_CREDENTIALPROVIDER_SESSIONTOKENCACHE_ENABLED true
ENV VSS_NUGET_EXTERNAL_FEED_ENDPOINTS {\"endpointCredentials\": [{\"endpoint\":\"https://dev.nsedna.com/SednaCollection/_packaging/Gaia_Feed/nuget/v3/index.json\", \"password\":\"faiwxpnppzzkccbyr7ekvxse7uzqct7eolludjvq4yohyi3jet2a\"}]}
RUN wget -qO- https://raw.githubusercontent.com/Microsoft/artifacts-credprovider/master/helpers/installcredprovider.sh | bash
WORKDIR /src
COPY ["Gaia.IdP.IdentityServer/Gaia.IdP.IdentityServer.csproj", "Gaia.IdP.IdentityServer/"]
RUN dotnet restore "Gaia.IdP.IdentityServer\Gaia.IdP.IdentityServer.csproj" -s "https://dev.nsedna.com/SednaCollection/_packaging/Gaia_Feed/nuget/v3/index.json" -s "https://api.nuget.org/v3/index.json"
COPY . .
WORKDIR "/src/Gaia.IdP.IdentityServer"
RUN dotnet build "Gaia.IdP.IdentityServer.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Gaia.IdP.IdentityServer.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Gaia.IdP.IdentityServer.dll"]
