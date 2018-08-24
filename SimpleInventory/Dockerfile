FROM microsoft/aspnetcore:2.0 AS base
WORKDIR /app
EXPOSE 80

FROM microsoft/aspnetcore-build:2.0 AS build
WORKDIR /src
COPY SimpleInventory.sln ./
COPY SimpleInventory/SimpleInventory.csproj SimpleInventory/
COPY Functional.Lib/Functional.Lib.csproj Functional.Lib/
COPY SimpleInventory.BL/SimpleInventory.BL.csproj SimpleInventory.BL/
COPY SimpleInventory.DL/SimpleInventory.DL.csproj SimpleInventory.DL/
RUN dotnet restore -nowarn:msb3202,nu1503
COPY . .
WORKDIR /src/SimpleInventory
RUN dotnet build -c Release -o /app

FROM build AS publish
RUN dotnet publish -c Release -o /app

FROM base AS final
WORKDIR /app
COPY --from=publish /app .
ENTRYPOINT ["dotnet", "SimpleInventory.dll"]
