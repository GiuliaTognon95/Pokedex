FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY ["Pokedex.csproj", "./"]
RUN dotnet restore "Pokedex.csproj"
COPY . .
RUN dotnet build "Pokedex.csproj" -c Release -o /app/build

FROM mcr.microsoft.com/dotnet/aspnet:8.0
WORKDIR /app
COPY --from=build /app/build .

EXPOSE 5094

ENV ASPNETCORE_URLS=http://0.0.0.0:5094

ENTRYPOINT ["dotnet", "Pokedex.dll"]