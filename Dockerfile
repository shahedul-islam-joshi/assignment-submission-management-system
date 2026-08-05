FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

COPY *.slnx ./
COPY AssignmentManagement.API/*.csproj AssignmentManagement.API/
COPY AssignmentManagement.Application/*.csproj AssignmentManagement.Application/
COPY AssignmentManagement.Domain/*.csproj AssignmentManagement.Domain/
COPY AssignmentManagement.Infrastructure/*.csproj AssignmentManagement.Infrastructure/
COPY AssignmentManagement.Tests/*.csproj AssignmentManagement.Tests/
RUN dotnet restore AssignmentManagement.API/AssignmentManagement.API.csproj

COPY . .
WORKDIR /src/AssignmentManagement.API
RUN dotnet publish -c Release -o /app

FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS runtime
WORKDIR /app
COPY --from=build /app .

ENV ASPNETCORE_URLS=http://+:10000
EXPOSE 10000

ENTRYPOINT ["dotnet", "AssignmentManagement.API.dll"]