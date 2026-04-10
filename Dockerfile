FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /app

COPY . ./
RUN dotnet publish UserSite/UserSite.csproj -c Release -o out

FROM mcr.microsoft.com/dotnet/aspnet:10.0

RUN apt-get update && apt-get install -y libgssapi-krb5-2 && rm -rf /var/lib/apt/lists/*

WORKDIR /app
COPY --from=build /app/out .

ENV ASPNETCORE_URLS=http://+:10000

ENTRYPOINT ["dotnet", "UserSite.dll"]