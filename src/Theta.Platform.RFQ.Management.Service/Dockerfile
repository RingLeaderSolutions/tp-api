FROM microsoft/dotnet:2.2-aspnetcore-runtime AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM microsoft/dotnet:2.2-sdk AS build
WORKDIR /src
COPY Theta.Platform.RFQ.Management.Service/Theta.Platform.RFQ.Management.Service.csproj Theta.Platform.RFQ.Management.Service/
RUN dotnet restore Theta.Platform.RFQ.Management.Service/Theta.Platform.RFQ.Management.Service.csproj
COPY . .
WORKDIR /src/Theta.Platform.RFQ.Management.Service
RUN dotnet build Theta.Platform.RFQ.Management.Service.csproj -c Release -o /app

FROM build AS publish
ARG Version
RUN dotnet publish Theta.Platform.RFQ.Management.Service.csproj -c Release -o /app

FROM base AS final
WORKDIR /app
COPY --from=publish /app .
ENTRYPOINT ["dotnet", "Theta.Platform.RFQ.Management.Service.dll"]