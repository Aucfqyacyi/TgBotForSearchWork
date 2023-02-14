#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

#Depending on the operating system of the host machines(s) that will build or run the containers, the image specified in the FROM statement may need to be changed.
#For more information, please see https://aka.ms/containercompat

FROM mcr.microsoft.com/dotnet/runtime:6.0 AS base
WORKDIR /app

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src
COPY ["TgBotForSearchWork.csproj", "."]
RUN dotnet restore "./TgBotForSearchWork.csproj"
COPY . .
WORKDIR "/src/."
RUN dotnet build "TgBotForSearchWork.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "TgBotForSearchWork.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
CMD ["dotnet", "TgBotForSearchWork.dll", "--token", "2128837514:AAEJVAT5VUFQcXGo03tsCj0KobKns8HQRgU", "--time", "60"]