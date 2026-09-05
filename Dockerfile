# Build stage
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src

COPY ["SimpleProject/SimpleProject.csproj", "SimpleProject/"]
RUN dotnet restore "SimpleProject/SimpleProject.csproj"

COPY . .
WORKDIR "/src/SimpleProject"

RUN dotnet build "SimpleProject.csproj" -c Release -o /app/build

# Publish stage
FROM build AS publish
RUN dotnet publish "SimpleProject.csproj" -c Release -o /app/publish /p:UseAppHost=false

# Runtime stage
FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS final
WORKDIR /app

COPY --from=publish /app/publish .

RUN mkdir -p /app/wwwroot/images

EXPOSE 8080

ENTRYPOINT ["dotnet", "SimpleProject.dll"]