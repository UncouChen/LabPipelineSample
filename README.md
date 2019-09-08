# LabPipelineSample

## check out codes

```
git clone https://github.com/UncouChen/LabPipelineSample 
git submodule update --init --recursive
```

## install docker

 https://docs.docker.com/docker-for-windows/install/


## install unity
 TODO Unity版本号？



## 实验步骤

- 搭建ElasticSearch 服务
  - 进入 `docker-elk`目录，运行 ```docker-compose up```
  - 稍等片刻，进入kibana  http://localhost:5601 ,账号elastic，密码changeme

- 搭建后端服务
  - 安装依赖项 ```pip install -r requirements.txt```
  - 运行后端服务 ```python index.py```
  - 进入 http://127.0.0.1:6000/apidocs 查看文档

- 打开Unity程序
  - 标记数据
  - 开始训练
  - 开始预测