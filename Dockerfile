FROM mcr.microsoft.com/dotnet/aspnet:9.0

RUN apt-get update && \
    DEBIAN_FRONTEND=noninteractive apt-get install -y \
    sqlite3 \
    && rm -rf /var/lib/apt/lists/*

WORKDIR /rest-api-cs

COPY bin/Debug/net9.0/ /rest-api-cs

EXPOSE 8080

CMD ["dotnet", "/rest-api-cs/rest-api-cs.dll"]
