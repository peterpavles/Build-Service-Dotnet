#!/usr/bin/env bash

set -e

>&2 echo "Waiting for FactionDB to start..."
bash ./Scripts/wait-for-it.sh db:5432 -- echo "FactionDB Started.."

>&2 echo "Waiting for RabbitMQ to start..."
bash ./Scripts/wait-for-it.sh mq:5672 -- echo "RabbitMQ Started.."

dotnet run ../out/dotnet-build.dll