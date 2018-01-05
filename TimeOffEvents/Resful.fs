namespace Resful.Rest

[<AutoOpen>]
module Restful = 
    type ResResource<'a> = {
        GetAll : unit -> 'a seq
    }