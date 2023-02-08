# Staticfile services

[![docker](https://github.com/13angs/static-sv/actions/workflows/build.yml/badge.svg)](https://github.com/13angs/static-sv/actions/workflows/build.yml)

## Run the project

1. copy and rename .env.sample to copy .env

2. change the env to

```bash
ASPNETCORE_DOMAIN_URL=http://localhost:5000 # because I expose the api on port 5000
STATIC_SIGNATURE=<your_secret> # get this on the internet
```

3. run the service

```bash
docker compose up
```

## To develop the project


- build the project

```bash
dotnet build
```

- copy and rename `appsetting.json` the `appsetting.Development.json`
- update the env

```json
{
    ...
    "ASPNETCORE_DOMAIN_URL": "<api_url>",
    "Static": {
        ...
        "Secret": "<secret_key>",
        ...
    }
}
```

- try running the project

```bash
dotnet run
```

- continue developing


