#Depending on the operating system of the host machines(s) that will build or run the containers, the image specified in the FROM statement may need to be changed.
#For more information, please see https://aka.ms/containercompat

FROM microsoft/dotnet:2.2-aspnetcore-runtime-nanoserver-1803 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM microsoft/dotnet:2.2-sdk-nanoserver-1803 AS build
WORKDIR /src
COPY ["Theta.Paltform.Order.Read.Service/Theta.Paltform.Order.Read.Service.csproj", "Theta.Paltform.Order.Read.Service/"]
RUN dotnet restore "Theta.Paltform.Order.Read.Service/Theta.Paltform.Order.Read.Service.csproj"
COPY . .
WORKDIR "/src/Theta.Paltform.Order.Read.Service"
RUN dotnet build "Theta.Paltform.Order.Read.Service.csproj" -c Release -o /app

FROM build AS publish
RUN dotnet publish "Theta.Paltform.Order.Read.Service.csproj" -c Release -o /app

FROM base AS final
WORKDIR /app
COPY --from=publish /app .
ENTRYPOINT ["dotnet", "Theta.Paltform.Order.Read.Service.dll"]