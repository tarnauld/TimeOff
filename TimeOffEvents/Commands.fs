module TimeOff.Commands

open System
open JsonConvert
open EventStorage
open System

let storeVar = Lazy.Create(fun() -> InMemoryStore.Create<UserId, RequestEvent>())
let store = storeVar.Value

let executeAllCommands (events: RequestEvent list, command) = 
  for event in events do
    let stream = store.GetStream event.Request.UserId
    stream.Append [event]
  Logic.handleCommand store command

let reverse command events = events, command
  
let addHoliday uid leftBound rightBound =
  let request = {
        UserId = uid
        RequestId = Guid.NewGuid()
        Start = leftBound
        End = rightBound }

  let result = [RequestCreated request] |> reverse ( RequestTimeOff request ) |> executeAllCommands
  JsonConvert.JSON result

let cancelHoliday uid reqId =
  let guid = Guid.Parse reqId
  let stream = store.GetStream uid
  let request = stream.ReadAll() 
                |> Array.ofSeq
                |> Array.filter (fun req -> match req with
                                            | RequestCreated command -> guid.Equals command.RequestId && uid.Equals command.UserId
                                            | _ -> false )
                |> Array.exactlyOne

  let result = [request] |> reverse ( CancelRequest (uid, guid) ) |> executeAllCommands
  JsonConvert.JSON result

let acceptHoliday uid reqId =
  let guid = Guid.Parse reqId
  let stream = store.GetStream uid
  let request = stream.ReadAll() 
                |> Array.ofSeq
                |> Array.filter (fun req -> match req with
                                            | RequestCreated command -> guid.Equals command.RequestId && uid.Equals command.UserId
                                            | _ -> false )
                |> Array.exactlyOne


  let result = [request] |> reverse ( ValidateRequest (uid, Guid.Empty) ) |> executeAllCommands
  JsonConvert.JSON result

let getHoliday uid =
  let stream = store.GetStream uid
  let result = stream.ReadAll()
  JsonConvert.JSON result
