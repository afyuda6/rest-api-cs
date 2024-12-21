FROM mcr.microsoft.com/dotnet/sdk:9.0

RUN apt-get update && \
    DEBIAN_FRONTEND=noninteractive apt-get install -y \
    sqlite3 \
    curl && \
    apt-get clean

WORKDIR /rest-api-cs

COPY . /rest-api-cs

RUN dotnet restore
RUN dotnet publish -c Release -o /rest-api-cs/out

EXPOSE 8080

ENTRYPOINT ["dotnet", "/rest-api-cs/out/rest-api-cs.dll"]