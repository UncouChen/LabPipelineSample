from elasticsearch import Elasticsearch
from elasticsearch import helpers
# by default we connect to localhost:9200
es = Elasticsearch("http://elastic:changeme@localhost:9200")


class DataRepository(object):

    def storeData(self, data):
        # store data array
        actions = []
        for d in data:
            a = {
                "_index": "labdata",
                "_source": d
            }
            actions.append(a)
        success, _ = helpers.bulk(es, actions, raise_on_error=True)

    def getData(self, scopeId):
        # get data in scope
        query = {
            "size":10000,
            "query": {
                "bool": {
                    "must": [
                        {"term": {"scopeId": scopeId}}
                    ]
                }
            }
        }
        _searched=es.search(index="labdata", body=query)
        hits=_searched["hits"]["hits"]
        ret=[i["_source"] for i in hits]
        print(ret)
        return ret

    def getLabeledData(self):
        if es.indices.exists("labdata") == False:
            return []
        query = {
            #limit to 10000 records
            "size":10000,
            "query": {
                "bool": {
                    "must": [
                        {"exists": {"field": "type"}}
                    ],
                    "must_not": [
                        {"term": {"type": ""}}
                    ]
                }
            }
        }
        _searched= es.search(index="labdata", body=query)
        hits=_searched["hits"]["hits"]
        ret=[i["_source"] for i in hits]
        print(ret)
        return ret

    def getLatestModel(self):
        return es.get(index="labdata-model", id='latest')['_source']

    def setLatestModel(self, model):
        es.index(index="labdata-model", id='latest', body = model)
