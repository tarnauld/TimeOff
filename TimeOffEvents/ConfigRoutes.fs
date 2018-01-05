module TimeOff.Routes

open Suave
open Suave.Filters
open Suave.Operators
open Suave.Successful
open Suave.Writers
open EventStorage
open System
open Timeoff

let store = InMemoryStore.Create<UserId, RequestEvent>()

let executeAllCommands (events: RequestEvent list, command) = 
  for event in events do
    let stream = store.GetStream event.Request.UserId
    stream.Append [event]
  Logic.handleCommand store command

let reverse command events = events, command
  
let addHoliday uid leftBound rightBound =
  let request = {
        UserId = uid
        RequestId = Guid.Empty
        Start = leftBound
        End = rightBound }

  let result = [] |> reverse ( RequestTimeOff request ) |> executeAllCommands
  JsonConvert.JSON result

let getHoliday =
  let result = Logic.getRequestState [] 
  JsonConvert.JSON result



let app = 
    choose
      [ GET >=> choose 
      [ path "/hello" >=> OK "Hello GET" >=> setMimeType "application/json;charset=utf-8"
        path "/test" >=> OK "It fucking works GET" >=> setMimeType "application/json;charset=utf-8"
        path "/goodbye" >=> OK "Goodbye GET" >=> setMimeType "application/json;charset=utf-8"
        path "/add" >=> addHoliday 1 { Date = DateTime(2018, 12, 30); HalfDay = AM } { Date = DateTime(2018, 12, 30); HalfDay = PM }
        path "/get" >=> getHoliday ]]