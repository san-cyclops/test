FROM mcr.microsoft.com/dotnet/sdk:5.0

WORKDIR /source

COPY . .

RUN dotnet restore

RUN dotnet publish -c release -o /app

EXPOSE 80

WORKDIR /app

ENV ASPNETCORE_URLS https://*:443

CMD dotnet UpgradeEngine.dll