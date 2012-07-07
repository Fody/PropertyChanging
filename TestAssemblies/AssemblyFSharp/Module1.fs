// Learn more about F# at http://fsharp.net

namespace Namespace

open System.ComponentModel
 
type ClassWithProperties() =
    let mutable propval =""

    let event = new Event<_,_>()
    interface INotifyPropertyChanging with
        [<CLIEvent>]
        member x.PropertyChanging = event.Publish
 
    member x.OnPropertyChanging(name)=
         event.Trigger(x, new PropertyChangingEventArgs(name))

    member this.Property1
        with get() = propval
        and  set(v) = propval <- v