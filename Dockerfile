# Pobranie obrazu .NET SDK 8.0
FROM mcr.microsoft.com/dotnet/sdk:8.0

# Skopiowanie plik�w i folder�w z obecnej �cie�ki do folderu /app w powstaj�cym obrazie
COPY . /app

# Ustawienie folderu /app w powstaj�cym obrazie jako �cie�ki roboczej
WORKDIR /app

# Pobranie brakuj�cych paczek NuGet
RUN dotnet restore

# Wykonanie publisha do folderu /publish, ustawiaj�c Meble.Server.csproj jako projekt startowy aplikacji
RUN dotnet publish ./Meble.Server/Meble.Server.csproj -c Release -o /publish/

# Ustawienie folderu /publish jako �cie�ki roboczej
WORKDIR /publish

# Ustawienie komendy uruchamiaj�cej aplikacj� .NET
# Uwaga: Zmienna �rodowiskowa PORT musi by� ustawiona na poziomie uruchomienia kontenera, je�li chcesz u�ywa� dynamicznie przydzielanych port�w przez platform� hostingu, np. Heroku
CMD ASPNETCORE_URLS=http://*:$PORT dotnet Meble.Server.dll
