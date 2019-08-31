from flask import Flask,request,Response
from dataRepository import DataRepository
from flasgger import Swagger

app = Flask(__name__)
swagger = Swagger(app)

repo = DataRepository()

@app.route('/')
def hello():
    '''health check
    ---
    responses:
      200:
        description: Ok
    '''
    return 'Hello World!'

@app.route('/uploadData',methods=['POST'])
def uploadData():
    '''upload data for unity program
    ---
    parameters:
      - name: Data
        in: body
        type: array
        items:
          $ref: '#/definitions/Pos'
    definitions:
      Pos:
        type: object
        properties:
          x:
            type: number
            format: double
          y:
            type: number
            format: double
          timeStamp:
            type: string
            format: date-time
          scopeId:
            type: string
          type:
            type: string     
    responses:
      200:
        description: Ok
    '''   
    repo.storeData(request.json)
    return Response("{}", status=201, mimetype='application/json')

@app.route('/startTrain',methods=['GET'])
def startTrain():
    '''start train and store model
    ---
    responses:
      200:
        description: Ok
    ''' 
    #get labeled data
    labeled=repo.getLabeledData()
    print(labeled)
    # group by scope Id order by timeStamp
    # train
    # store model  
    #TODO
    model={}
    repo.setLatestModel(model)
    return 'Hello World!'


@app.route('/predictResult/<scopeId>',methods=['GET'])
def predictResult(scopeId):
    '''get predict result of scope
    ---
    parameters:
      - name: scopeId
        in: path
        type: string
        required: true
    responses:
      200:
        description: circle/triangle
    '''
    #TODO
    # get model
    model=repo.getLatestModel()
    print(model)
    # get scope data
    data=repo.getData(scopeId)
    print(data)
    # predict 
    #TODO
    return 'Hello World!'+scopeId


if __name__ == '__main__':
    app.run(debug=True)