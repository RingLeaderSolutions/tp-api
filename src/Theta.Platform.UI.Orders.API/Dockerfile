ARG Version

FROM microsoft/dotnet:2.1-aspnetcore-runtime AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM microsoft/dotnet:2.1-sdk AS build
WORKDIR /src
COPY Theta.Platform.UI.Orders.API/Theta.Platform.UI.Orders.API.csproj Theta.Platform.UI.Orders.API/
RUN dotnet restore Theta.Platform.UI.Orders.API/Theta.Platform.UI.Orders.API.csproj
COPY . .
WORKDIR /src/Theta.Platform.UI.Orders.API
RUN dotnet build Theta.Platform.UI.Orders.API.csproj -c Release -o /app

FROM build AS publish
RUN dotnet publish Theta.Platform.UI.Orders.API.csproj /p:Version=$Version -c Release -o /app

FROM base AS final
WORKDIR /app
COPY --from=publish /app .
ENTRYPOINT ["dotnet", "Theta.Platform.UI.Orders.API.dll"]
