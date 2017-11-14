Allow creating observable over eventstore connection, and add some reactive extensions to handle events. Created observables are hot and refcount, so that subscription stay alive until their is subscribers.

nuget id : 

``` EventStore.Client.Reactive ```

Create observable :

```csharp
var observable = connection.CreateObservable("stream-id"); //subribe from end of stream
```

Then deserialize events :

```csharp
var subscription = connection.CreateObservable(StreamId)
                             .Deserialize<MyEvent>()
                             .Subscribe(e => Console.WriteLine("event handled : " + e.ToString())
```

If position matter (eg to handle checkpoints) :

```csharp
 var subscription = connection.CreateObservable(StreamId)
                              .DeserializeWithPosition<MyEvent>()
                              .HandleEvent(e => Console.WriteLine("handled : " + e.ToString()))
                              .Subscribe(pos => Console.WriteLine("last position handled : " + pos));
```

If you want to subscribe from the beginning of stream :

```csharp
var observable = connection.CreateObservable("stream-id", 0);
```

Custom serializer

```csharp
var subscription = connection.CreateObservable(StreamId)
                             .Deserialize<MyEvent>(new CustomEventSerializer())
			     .Subscribe(e => Console.WriteLine("event handled : " + e.ToString())
```
