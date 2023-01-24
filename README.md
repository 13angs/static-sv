# Staticfile services

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


