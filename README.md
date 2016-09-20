# Akka.NET-Sample

使用 Akka .NET, Sample Code 是參考 [akka-sample](https://github.com/kigichang/akka-sample)，由 Scala 改寫成 C# 版本

## HelloWorld
Demo 最基本的用法。

## PenguinJoke
利用企鵝笑話當例子，demo Actor 間訊息的傳播，以下的 sample code 都會 reference 這個 project。
## Router1
最簡單的 Router 應用。
## Router2
客製化 Router sample。
## Supervisor
Demo Supervisor Strategy 用法。
## Remote - Lookup
Demo Akka Remote 中 Lookup 的用法, Lookup 的模式，類似 Client-Server 架構。

* LookupServer

    Server Side 程式，會產生企鵝 Actor 供 Client 使用。

* LookupClient

    Client Side 程式，偵測 Server Side 的企鵝是否 ready, 如果已經 ready，則開始傳遞訊息。

## Remote - Deploy
Demo Akka Remote 中 Deploy 的用法，類似在 Cluster 中，尋找可用的 Node，將運算工作派發到該 Node 上。

* DeployMaster

    將企鵝 Actor 派發到 DeployNode 上。
    
* DeployNode

    接受 DeployMaster 派發來的企鵝 Actor 並執行工作。
