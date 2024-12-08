#BUILD
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS BUILD

WORKDIR /src

COPY ./LifeChart .

COPY ./LifeChartServices .

RUN dotnet restore "./LifeChart.csproj" --disable-parallel

RUN dotnet publish "./LifeChart.csproj" -c release -o /app --no-restore

#SERVE
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS SERVE

ENV ASPNETCORE_ENVIRONMENT Production

WORKDIR /app

#copy app from build stage to current dir
COPY --from=build /app ./

EXPOSE 7010

ENTRYPOINT ["dotnet", "LifeChart.dll"]