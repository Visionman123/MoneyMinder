#BUILD
FROM mcr.microsoft.com/dotnet/sdk:8.0-focal AS build
WORKDIR /src
COPY ./LifeChartAPI .
RUN dotnet restore "./LifeChartAPI.csproj" --disable-parallel
RUN dotnet publish "./LifeChartAPI.csproj" -c release -o /api --no-restore

#SERVE
FROM mcr.microsoft.com/dotnet/sdk:8.0-focal
ENV ASPNETCORE_ENVIRONMENT Production
WORKDIR /api
#copy app from build stage to current dir
COPY --from=build /api ./

EXPOSE 7147

ENTRYPOINT ["dotnet", "LifeChartAPI.dll"]