FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

COPY PRN232.LMS.sln ./
COPY PRN232.LMS.API/PRN232.LMS.API.csproj PRN232.LMS.API/
COPY PRN232.LMS.Services/PRN232.LMS.Services.csproj PRN232.LMS.Services/
COPY PRN232.LMS.Repositories/PRN232.LMS.Repositories.csproj PRN232.LMS.Repositories/
RUN dotnet restore PRN232.LMS.sln

COPY . ./
WORKDIR /src/PRN232.LMS.API
RUN dotnet publish -c Release -o /app/publish

FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS runtime
WORKDIR /app
EXPOSE 8080
COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "PRN232.LMS.API.dll"]

