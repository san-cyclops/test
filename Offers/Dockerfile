FROM mcr.microsoft.com/dotnet/sdk:5.0-alpine

WORKDIR /source

COPY . .

#RUN dotnet clean Offers.csproj

RUN dotnet restore Offers.csproj 

#--disable-parallel

RUN ls -al

RUN dotnet publish -c release -o /app

EXPOSE 51169

WORKDIR /app

ENV ASPNETCORE_URLS http://*:51169

CMD dotnet Offers.dll