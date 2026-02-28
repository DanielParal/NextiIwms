FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS base

# Switch to root for tzdata installation
USER root
# Install tzdata to manage timezones
RUN apt-get update && apt-get install -y tzdata
# Set a default timezone (can be overridden in docker-compose)
ENV TZ=UTC
RUN ln -snf /usr/share/zoneinfo/$TZ /etc/localtime && echo $TZ > /etc/timezone

#Install powershell to playwright scripts
RUN apt-get update -yq \
    && apt-get install wget -yq \
    && wget -q https://packages.microsoft.com/config/ubuntu/20.04/packages-microsoft-prod.deb \
    && dpkg -i packages-microsoft-prod.deb \
    && apt-get update -yq \
    && apt-get install powershell -yq

USER $APP_UID
WORKDIR /app
EXPOSE 80

FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
ARG BUILD_CONFIGURATION=Release

WORKDIR /src
COPY ["Projects/Nexty/Tenants/Jipocar/Nexticz.Nexty.Jipocar.Api/Nexticz.Nexty.Jipocar.Api.csproj", "Projects/Nexty/Tenants/Jipocar/Nexticz.Nexty.Jipocar.Api/"]
COPY ["Modules/Auth/Nexticz.Module.Auth.Presentation/Nexticz.Module.Auth.Presentation.csproj", "Modules/Auth/Nexticz.Module.Auth.Presentation/"]
COPY ["Libs/Nexticz.Lib.Shared/Nexticz.Lib.Shared.csproj", "Libs/Nexticz.Lib.Shared/"]
COPY ["Modules/Cuzk/Nexticz.Module.Cuzk/Nexticz.Module.Cuzk.csproj", "Modules/Cuzk/Nexticz.Module.Cuzk/"]
COPY ["Modules/EmailSender/Nexticz.Module.EmailSender/Nexticz.Module.EmailSender.csproj", "Modules/EmailSender/Nexticz.Module.EmailSender/"]
COPY ["Modules/Lang/Nexticz.Module.Lang.Presentation/Nexticz.Module.Lang.Presentation.csproj", "Modules/Lang/Nexticz.Module.Lang.Presentation/"]
COPY ["Modules/Mmo/Nexticz.Module.Mmo.ModuleRegistrator/Nexticz.Module.Mmo.ModuleRegistrator.csproj", "Modules/Mmo/Nexticz.Module.Mmo.ModuleRegistrator/"]
COPY ["Modules/Notifications/Nexticz.Module.Notifications/Nexticz.Module.Notifications.csproj", "Modules/Notifications/Nexticz.Module.Notifications/"]
COPY ["Modules/Portal/Nexticz.Module.Portal/Nexticz.Module.Portal.csproj", "Modules/Portal/Nexticz.Module.Portal/"]
COPY ["Modules/Vh/Nexticz.Module.Vh.Presentation/Nexticz.Module.Vh.Presentation.csproj", "Modules/Vh/Nexticz.Module.Vh.Presentation/"]
COPY ["Modules/Sign/Nexticz.Module.Sign.ModuleRegistrator/Nexticz.Module.Sign.ModuleRegistrator.csproj", "Modules/Sign/Nexticz.Module.Sign.ModuleRegistrator/"]

RUN dotnet restore "Projects/Nexty/Tenants/Jipocar/Nexticz.Nexty.Jipocar.Api/Nexticz.Nexty.Jipocar.Api.csproj"
COPY . .
WORKDIR "/src/Projects/Nexty/Tenants/Jipocar/Nexticz.Nexty.Jipocar.Api"
RUN dotnet build "./Nexticz.Nexty.Jipocar.Api.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./Nexticz.Nexty.Jipocar.Api.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false
COPY --from=build /app/build/playwright.ps1 /app/publish

FROM base AS final
USER root
WORKDIR /app
COPY --from=publish /app/publish .

# Install playwright dependencies and cleanup
RUN pwsh playwright.ps1 install chromium
RUN pwsh playwright.ps1 install-deps chromium
RUN rm -rf playwright.ps1
RUN apt remove wget powershell -yq

ENTRYPOINT ["dotnet", "Nexticz.Nexty.Jipocar.Api.dll"]
