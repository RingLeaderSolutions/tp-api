FROM microsoft/dotnet:2.1-aspnetcore-runtime AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM microsoft/dotnet:2.1-sdk AS build
WORKDIR /src
COPY Theta.Platform.UI.Pricing.Streaming/Theta.Platform.UI.Pricing.Streaming.csproj Theta.Platform.UI.Pricing.Streaming/
RUN dotnet restore Theta.Platform.UI.Pricing.Streaming/Theta.Platform.UI.Pricing.Streaming.csproj
COPY . .
WORKDIR /src/Theta.Platform.UI.Pricing.Streaming
RUN dotnet build Theta.Platform.UI.Pricing.Streaming.csproj -c Release -o /app

FROM build AS publish
RUN dotnet publish Theta.Platform.UI.Pricing.Streaming.csproj -c Release -o /app

FROM base AS final
WORKDIR /app
COPY --from=publish /app .
ENTRYPOINT ["dotnet", "Theta.Platform.UI.Pricing.Streaming.dll"]
