# Play Inventoty

### Build docker image - linux
```s
VERSION=1.0.0
export GH_OWNER="playhuborg"
export GH_PAT="[GithubPersonalToken]"
docker build --no-cache --progress=plain --secret id=GH_OWNER --secret id=GH_PAT   -t play.inventory:$VERSION .
```

### Run the docker image
```s
docker run -it --rm -p 5004:5004 --name inventory -e MongoDbSettings__Host=mongo -e RabbitMQSettings__Host=rabbitmq -e ServiceSettings__Authority=identity:5002 -e ClientUrls__CatalogService=catagory:5000 --network playinfra_default play.inventory:$VERSION
```