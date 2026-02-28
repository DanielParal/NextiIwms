# How to run Cuzk module

## 1. Add Elasticsearch to the docker compose

You need to add elasticsearch to the docker compose. It is already added locally.

```
  elasticsearch:
    image: docker.elastic.co/elasticsearch/elasticsearch:9.2.2
    container_name: elasticsearch
    environment:
      - discovery.type=single-node
      - xpack.security.enabled=false
      - ES_JAVA_OPTS=-Xms1g -Xmx1g
    ports:
      - "9200:9200"
      - "9300:9300"
    volumes:
      - ./data/elasticsearch:/usr/share/elasticsearch/data
```

## 2. You need to enable cuzk module in Features.json

Locally it is already enabled. In Staging in production you need to add this line in Azure Key Vault

```
"CuzkIsEnabled": "true"
```

## 3. Import data

You need to import municipalities first.
After that you need to import all address locations.
The first import is very slow. It takes about 7 hours to finish. Next imports are fast.