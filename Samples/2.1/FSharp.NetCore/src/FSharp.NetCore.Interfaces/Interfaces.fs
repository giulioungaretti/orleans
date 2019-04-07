namespace FSharp.NetCore
module Interfaces =

    open System.Threading.Tasks

    type Sample = {
        Value: string
    }

    type SampleHolder (s:Sample option) =
        new() = SampleHolder None
        member val Sample:Sample option = None with get,set

    type IHello = 
        inherit Orleans.IGrainWithGuidKey
        abstract member SayHello : string -> Task<Sample option>
        abstract member GetSample : System.Guid -> Task<Sample option>
        abstract member SetSample : Sample -> Task
