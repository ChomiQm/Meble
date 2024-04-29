# Pobranie obrazu .NET SDK 8.0
FROM mcr.microsoft.com/dotnet/sdk:8.0

# Skopiowanie plików i folderów z obecnej œcie¿ki do folderu /app w powstaj¹cym obrazie
COPY . /app

# Ustawienie folderu /app w powstaj¹cym obrazie jako œcie¿ki roboczej
WORKDIR /app

# Pobranie brakuj¹cych paczek NuGet
RUN dotnet restore

# Wykonanie publisha do folderu /publish, ustawiaj¹c Meble.Server.csproj jako projekt startowy aplikacji
RUN dotnet publish ./Meble.Server/Meble.Server.csproj -c Release -o /publish/

# Ustawienie folderu /publish jako œcie¿ki roboczej
WORKDIR /publish

# Ustawienie komendy uruchamiaj¹cej aplikacjê .NET
# Uwaga: Zmienna œrodowiskowa PORT musi byæ ustawiona na poziomie uruchomienia kontenera, jeœli chcesz u¿ywaæ dynamicznie przydzielanych portów przez platformê hostingu, np. Heroku
CMD ASPNETCORE_URLS=http://*:$PORT dotnet Meble.Server.dll
