FROM ubuntu:20.04

RUN apt-get update && \
    DEBIAN_FRONTEND=noninteractive apt-get install -y \
    sqlite3 \
    libsqlite3-dev \
    ca-certificates \
    curl \
    libunwind8 \
    gnupg2 \
    lsb-release && \
    rm -rf /var/lib/apt/lists/*

RUN curl https://packages.microsoft.com/keys/microsoft.asc | apt-key add - && \
    curl https://packages.microsoft.com/config/ubuntu/20.04/prod.list > /etc/apt/sources.list.d/microsoft-prod.list && \
    apt-get update && \
    apt-get install -y dotnet-sdk-9.0

WORKDIR /rest-api-cs

COPY . /rest-api-cs

RUN dotnet restore
RUN dotnet publish -c Release -o /rest-api-cs/out

EXPOSE 8080

ENTRYPOINT ["dotnet", "/rest-api-cs/out/rest-api-cs.dll"]