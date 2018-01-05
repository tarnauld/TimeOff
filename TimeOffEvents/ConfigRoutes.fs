module TimeOff.Routes

open Suave
open Suave.Filters
open Suave.Operators
open Suave.Successful
open Suave.Writers
let app = 
    choose
      [ GET >=> choose 
      [ path "/hello" >=> OK "Hello GET" >=> setMimeType "application/json;charset=utf-8"
        path "/test" >=> OK "It fucking works GET" >=> setMimeType "application/json;charset=utf-8"
        path "/goodbye" >=> OK "Goodbye GET" >=> setMimeType "application/json;charset=utf-8" ]]