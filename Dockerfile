# Используйте базовый образ от Microsoft
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build-env
WORKDIR /app
# Копируйте csproj и восстанавливайте зависимости
COPY *.sln ./
COPY UserService.WebApi/*.csproj ./UserService.WebApi/
COPY UserService.Infrastructure/*.csproj ./UserService.Infrastructure/
COPY UserService.Core/*.csproj ./UserService.Core/
COPY UserService.Application/*.csproj ./UserService.Application/
COPY UserService.DataAccess/*.csproj ./UserService.DataAccess/
RUN dotnet restore



# Копируйте остальной проект и собирайте
COPY . ./
RUN dotnet publish UserService.WebApi/*.csproj -c Release -o out

# Генерируйте runtime образ
FROM mcr.microsoft.com/dotnet/aspnet:8.0
WORKDIR /app
COPY --from=build-env /app/out .
# Копирование сертификатов в образ
COPY ["/certs/UserService.WebApi.pfx", "/https/"]
ENV ASPNETCORE_ENVIRONMENT=InDocker
ENTRYPOINT ["dotnet", "UserService.WebApi.dll"]
