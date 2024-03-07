FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /App

# Copy everything
COPY . ./

# Restore as distinct layers
RUN dotnet restore
# Build and publish a release
RUN dotnet publish -c Release -o out

# Build runtime image
FROM mcr.microsoft.com/dotnet/aspnet:8.0 as base
WORKDIR /App
RUN apt-get update && apt-get install -y xorg openbox libnss3 libasound2
COPY --from=build /App/out .
EXPOSE 80
ENTRYPOINT ["dotnet", "Pdferr.dll"]