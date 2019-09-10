from flask import Flask,request,Response
from dataRepository import DataRepository
from flasgger import Swagger
from sklearn import svm
import numpy as np
from sklearn import model_selection
import pickle
from sklearn.externals import joblib
import json

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
    return Response("{}", status=200, mimetype='application/json')




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
    if len(labeled)==0:
      message={"message":"empty data"}
      return Response(json.dumps(message), status=400, mimetype='application/json')
    #print(labeled)
    # group by scope Id order by timeStamp
    # train
    # store model  
    #TODO
    preId=''
    x=[]
    y=[]
    Xcount=0
    for item in labeled:
      #print(item)
      if(preId==item['scopeId']):
        if( Xcount>=50):
          continue
        x.append(item['x'])
        x.append(item['y'])
        Xcount+=1
      else:
        Xcount=0
        if(item['type']=='Random'):
          y.append(0)
        else:
          y.append(1)
        x.append(item['x'])
        x.append(item['y'])
        preId=item['scopeId']
        Xcount+=1
    length = len(y)
    Ytype = np.array(y)
    position = np.array(x, ndmin = 2)
    Xtype= position.reshape(length,100)
    x_train, x_test, y_train, y_test =model_selection.train_test_split(Xtype, Ytype, random_state=1, train_size=0.6)
    clf = svm.SVC(C=0.1, kernel='linear', decision_function_shape='ovr')
    #clf = svm.SVC(C=0.8, kernel='rbf', gamma=20, decision_function_shape='ovr')
    clf.fit(x_train, y_train.ravel())
    print(clf.score(x_train, y_train)) # 精度
    y_hat = clf.predict(x_test)
    print(clf.score(x_test, y_test))
    value = pickle.dumps(clf)
    f = open('svm.model','wb+')
    f.write(value)
    f.close()
    #print(value)
    model={'model':'svm.model'}
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
    TrainModel= model['model']
     #print(TrainModel)
    f2 = open(TrainModel,'rb')
    s = f2.read()
    clf = pickle.loads(s)
    # get scope data
    print(scopeId)
    data=repo.getData(scopeId)
    print(data)
    if len(data)==0:
      message={"message":"empty data"}
      return Response(json.dumps(message), status=400, mimetype='application/json')

    # predict 
    #TODO
    x=[]
    count=0
    for item in data:
      x.append(item['x'])
      x.append(item['y'])
      count+=1
    print(count)
    position = np.array(x[0:100], ndmin = 2)
    Y = clf.predict(position)
    result=''
    if(Y==0):
      result = 'Random'
    elif(Y==1):
      result = 'circle'

    return result


if __name__ == '__main__':
    app.run( host='0.0.0.0',port = 5700,debug=True )