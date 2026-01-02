#See https://aka.ms/customizecontainer to learn how to customize your debug container and how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS base
WORKDIR /app
EXPOSE 8080

FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src
COPY ["PrintBed/PrintBed.csproj", "PrintBed/"]
RUN dotnet restore "PrintBed/PrintBed.csproj"
COPY . .
WORKDIR "/src/PrintBed"
RUN dotnet build "PrintBed.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "PrintBed.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
ENV ASPNETCORE_URLS=http://*:8080
ENV ASPNETCORE_HTTP_PORTS=8080
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "PrintBed.dll"]
