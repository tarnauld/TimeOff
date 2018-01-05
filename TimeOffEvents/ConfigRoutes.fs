module TimeOff.Routes

open Suave
open Suave.Filters
open Suave.Operators
open Suave.Writers

open RestAPI

let app = 
    choose
      [ GET >=> choose
          [ path "/hello" >=> (JsonReturn "hello" "GET") >=> setMimeType "application/json;charset=utf-8"
            path "/test" >=> (JsonReturn "It fucking works" "GET") >=> setMimeType "application/json;charset=utf-8"
            path "/goodbye" >=> (JsonReturn "Goodbye" "GET") >=> setMimeType "application/json;charset=utf-8" ]
        POST >=> choose
          [ path "/hello" >=> (JsonReturn "hello" "POST") >=> setMimeType "application/json;charset=utf-8"
            path "/test" >=> (JsonReturn "It fucking works" "POST") >=> setMimeType "application/json;charset=utf-8"
            path "/goodbye" >=> (JsonReturn "Goodbye" "POST") >=> setMimeType "application/json;charset=utf-8" ] ]