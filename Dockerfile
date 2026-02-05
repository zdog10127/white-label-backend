FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 5000

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

COPY ["WhiteLabel/WhiteLabel.csproj", "WhiteLabel/"]
COPY ["WhiteLabel.Domain/WhiteLabel.Domain.csproj", "WhiteLabel.Domain/"]
COPY ["WhiteLabel.Infra/WhiteLabel.Infra.csproj", "WhiteLabel.Infra/"]
COPY ["WhiteLabel.Service/WhiteLabel.Service.csproj", "WhiteLabel.Service/"]
COPY ["WhiteLabel.Utility/WhiteLabel.Utility.csproj", "WhiteLabel.Utility/"]

RUN dotnet restore "WhiteLabel/WhiteLabel.csproj"

COPY . .
WORKDIR "/src/WhiteLabel"
RUN dotnet build "WhiteLabel.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "WhiteLabel.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .

ENV ASPNETCORE_ENVIRONMENT=Production
ENV ASPNETCORE_URLS=http://+:5000

ENTRYPOINT ["dotnet", "WhiteLabel.dll"]