FROM mcr.microsoft.com/dotnet/sdk:5.0

COPY . /app
WORKDIR /app/TotuusBotti
CMD dotnet run