FROM mono:5.18.0
WORKDIR /app

RUN apt-get update && \
    apt-get install wget gpg apt-transport-https apt-utils dirmngr -y && \
    wget -qO- https://packages.microsoft.com/keys/microsoft.asc | gpg --dearmor > microsoft.asc.gpg && \
    mv microsoft.asc.gpg /etc/apt/trusted.gpg.d/  && \
    wget -q https://packages.microsoft.com/config/debian/9/prod.list  && \
    mv prod.list /etc/apt/sources.list.d/microsoft-prod.list && \
    chown root:root /etc/apt/trusted.gpg.d/microsoft.asc.gpg && \
    chown root:root /etc/apt/sources.list.d/microsoft-prod.list && \
    apt-get update && \
    apt-get install dotnet-sdk-2.2 -y 

# copy csproj and restore as distinct layers
COPY *.csproj ./
RUN dotnet restore

# copy and build everything else
COPY . ./
RUN chmod 777 ./Scripts/wait-for-it.sh && dotnet publish -c Release -o out
ENTRYPOINT [ "/bin/bash", "./Scripts/startup.sh" ]