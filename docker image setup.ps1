#list container running
docker ps
#connect to sql simpleinventorydb in container in interactive mode run sqlcmd at location /opt/mssql-tools/bin with servername=localhost and username =sa
docker exec -it simpleinventorydb /opt/mssql-tools/bin/sqlcmd -S localhost -U sa
#run entityframework migration nuget package; have to run in the project folder
dotnet ef migrations initialmigration -o data/migration -c
#check passive docker images...images that are not running
docker ps -a
#to start a passive catalogdb
docker start cataglogdb
#check to see if it is started 
docker ps
#run update entity framework to create the database in code with the context namespace
dotnet ef dtabase update initialmigration -c shoesoncontainers.services.productcatalogapi.data.catalogcontext
#download 2 asp.net core to create docker image to dockerize newly created microservice
#1st iamage
docker pull microsoft/aspnetcore:2.0.0
#2nd package
docker pull microsoft/aspnetcore-build:2.0.0
#in vs create a file called Dockerfile
#create a docker image template
FROM microsoft/aspnetcore-build:2.0.0 AS build
#set the working directory..inside image if directory doesnot exist, the docker will create
WORKDIR /code
#copy from file from dev to docker image
COPY . .
#run dotnet restore to restore all packages from nuget package
RUN dotnet restore
#now publish and output to "out" folder with the configuration="release" but this image will have all your source code
RUN dotnet publish -output /out/ --configuration Release
#create new docker image with just binary files
FROM microsoft/aspnetcore:2.0.0
#copy file from release version...copy the frist image with alias="build" taken from first "FROM", copy from "Out" folder to new "app" folder
COPY --from=build /out /app/
#set working directory
WORKDIR /app
#this is what docker image is going to run
ENTRYPOINT["dotnet", "ProductCatalogApi.dll"]
#end first docker files is complete
#now go to project folder where the new docker file was created
#open power shell type the follow to build docker image with name shoe/catalog. (.) says to build in current location
docker build -t shoes/catalog .
 

#create docker.compose.yml in folder underneath solution called "solution Items"
version: "3.2"

networks:
    frontend:
    backend:

services:
    catalog:
        build:
            context: .\src\services\productCatalogApi
            dockerfile: Dockerfile
        image shoes/catalog
        environment:
            - DatabaseServer=mssqlserver
            - DatabaseName=CatalogDb
            - DatabaseUser=sa
            - DatabaseUserPassword=Password123
        container_name:catalogapi
        ports:
            - "5000:80" #map docker port 80 to localhost 5000
        networks:
            - backend
            - frontend
        depends_on:
            - mssqlserver

    mssqlserver:
        image: "microsoft/mssql-server-linux:latest"
        ports:
            - "1445:1433"

        container_name: mssqlcontainer
        environment:
            - ACCEPT_EULA=Y
            - SA_PASSWORD=Password123!
        networks:
            -backend

#there were some compatibility problem which can be fixed by adding a line to .csproj file
 <PropertyGroup>
    <PublishWithAspNetCoreTargetManifest>false</PublishWithAspNetCoreTargetManifest>
</PropertyGroup>


#now change dir to project file
#run docker image to check if image exist
docker images
#remove it to recreate
docker rmi shoes/catalog
#list again
docker images
#image with <none> inn repostiory are orphan
#get rid of them 
docker system prune
#then answer yes to remove orphan
#see if any docker passing images
docker ps -a
#see active images
docker ps
#now run compose build to build image
docker-compose build
# now run mssql image
docker-compose up mssqlserver
#to veriy sql server is up running
#one way connect using sql management tool
#
# from different powershell start up the catalog docker image
docker-compose up catalog

