module TimeOff.Commands

open System
open JsonConvert
open EventStorage

let store = InMemoryStore.Create<UserId, RequestEvent>()

let executeAllCommands (events: RequestEvent list, command) = 
  for event in events do
    let stream = store.GetStream event.Request.UserId
    stream.Append [event]
    Logic.handleCommand store command |> 
    ignore

let reverse command events = events, command
  
let addHoliday uid leftBound rightBound =
  let request = {
        UserId = uid
        RequestId = Guid.Empty
        Start = leftBound
        End = rightBound }

  let result = [] |> reverse ( RequestTimeOff request ) |> executeAllCommands
  JsonConvert.JSON result

let cancelHoliday uid leftBound rightBound =
  let request = {
        UserId = uid
        RequestId = Guid.Empty
        Start = leftBound
        End = rightBound }

  let result = [] |> reverse ( RequestTimeOff request ) |> executeAllCommands
  JsonConvert.JSON result

let acceptHoliday uid leftBound rightBound =
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

let refuseHoliday =
  let result = Logic.getRequestState [] 
  JsonConvert.JSON result

let listHoliday uid leftBound rightBound =
  let request = {
        UserId = uid
        RequestId = Guid.Empty
        Start = leftBound
        End = rightBound }

  let result = [] |> reverse ( RequestTimeOff request ) |> executeAllCommands
  JsonConvert.JSON result