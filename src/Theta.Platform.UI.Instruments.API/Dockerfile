FROM microsoft/dotnet:2.2-aspnetcore-runtime AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM microsoft/dotnet:2.2-sdk AS build
WORKDIR /src
COPY Theta.Platform.UI.Instruments.API/Theta.Platform.UI.Instruments.API.csproj Theta.Platform.UI.Instruments.API/
RUN dotnet restore Theta.Platform.UI.Instruments.API/Theta.Platform.UI.Instruments.API.csproj
COPY . .
WORKDIR /src/Theta.Platform.UI.Instruments.API
RUN dotnet build Theta.Platform.UI.Instruments.API.csproj -c Release -o /app

FROM build AS publish
ARG Version
RUN dotnet publish Theta.Platform.UI.Instruments.API.csproj /p:Version=$Version -c Release -o /app

FROM base AS final
WORKDIR /app
COPY --from=publish /app .
ENTRYPOINT ["dotnet", "Theta.Platform.UI.Instruments.API.dll"]
