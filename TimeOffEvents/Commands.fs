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
  let stream = store.GetStream uid

  let result = Logic.handleCommand store ( RequestTimeOff request )
  match result with
  | Ok res -> stream.Append res
  | Error _ -> ()
  JsonConvert.JSON result

let cancelHoliday uid reqId =
  let guid = Guid.Parse reqId
  let stream = store.GetStream uid
  let result = Logic.handleCommand store ( CancelRequest(uid, guid) )
  match result with
  | Ok res -> stream.Append res
  | Error _ -> ()
  JsonConvert.JSON result

let acceptHoliday uid reqId =
  let guid = Guid.Parse reqId
  let stream = store.GetStream uid
  let result = Logic.handleCommand store ( ValidateRequest(uid, guid) )
  match result with
  | Ok res -> stream.Append res
  | Error _ -> ()
  JsonConvert.JSON result

let getHoliday uid =
  let stream = store.GetStream uid
  let result = stream.ReadAll()
  JsonConvert.JSON result
