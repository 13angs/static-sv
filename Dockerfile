# initial stage
FROM mcr.microsoft.com/dotnet/aspnet:6.0-alpine AS base
LABEL Maintainer="13angs" \
    VERSION=v0.0.1
WORKDIR /app
ENV ASPNETCORE_URLS http://+:5000
EXPOSE 5000

# build stage for dotnetcore
FROM mcr.microsoft.com/dotnet/sdk:6.0-alpine AS dotnet-build
WORKDIR /src
COPY ./static-sv.csproj ./
RUN dotnet restore "static-sv.csproj"
COPY ./ ./
RUN dotnet publish "static-sv.csproj" -c Release -o ./publish

# runtime stage
FROM base AS runtime
EXPOSE 5000
ENV TZ=Asia/Bangkok
RUN apk update && \
    apk add --no-cache tzdata && \
    mkdir -p wwwroot/images wwwroot/videos wwwroot/files 
COPY --from=dotnet-build /src/publish /app

CMD ["dotnet", "/app/static-sv.dll"]