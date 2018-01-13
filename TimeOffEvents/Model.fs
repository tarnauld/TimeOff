namespace TimeOff

open System
open EventStorage
open System
open JsonConvert

type User =
    | Employee of int
    | Manager

type HalfDay = | AM | PM

type Boundary = {
    Date: DateTime
    HalfDay: HalfDay
}

type UserId = int

type TimeOffRequest = {
    UserId: UserId
    RequestId: Guid
    Start: Boundary
    End: Boundary
}

type Command =
    | RequestTimeOff of TimeOffRequest
    | CancelRequest of UserId * Guid
    | ValidateRequest of UserId * Guid with
    member this.UserId =
        match this with
        | RequestTimeOff request -> request.UserId
        | ValidateRequest (userId, _) -> userId
        | CancelRequest (userId, _) -> userId

type RequestEvent =
    | RequestCreated of TimeOffRequest
    | RequestCancelled of TimeOffRequest
    | RequestValidated of TimeOffRequest with
    member this.Request =
        match this with
        | RequestCreated request -> request
        | RequestValidated request -> request
        | RequestCancelled request -> request

module Logic =

    type RequestState =
        | NotCreated
        | PendingValidation of TimeOffRequest
        | Cancelled of TimeOffRequest
        | Validated of TimeOffRequest with
        member this.Request =
            match this with
            | NotCreated -> invalidOp "Not created"
            | PendingValidation request
            | Validated request -> request
            | Cancelled request -> request
        member this.IsActive =
            match this with
            | NotCreated -> false
            | PendingValidation _
            | Validated _ -> true
            | Cancelled _ -> false

    let evolve _ event = match event with
                            | RequestCreated request -> PendingValidation request
                            | RequestValidated request -> Validated request
                            | RequestCancelled request -> Cancelled request

    let getRequestState events =
        events |> Seq.fold evolve NotCreated

    let getAllRequests events =
        let folder requests (event: RequestEvent) =
            let state = defaultArg (Map.tryFind event.Request.RequestId requests) NotCreated
            let newState = evolve state event
            requests.Add (event.Request.RequestId, newState)

        events |> Seq.fold folder Map.empty

    let IsDateInPeriod date period =
        (
            date.Date.CompareTo period.Start.Date > 0 
            && date.Date.CompareTo period.End.Date < 0
        )
        || (
            date.Date.Equals period.Start.Date
            && (
                date.HalfDay.Equals period.Start.HalfDay
                || (
                    date.HalfDay.Equals PM
                    && period.Start.HalfDay.Equals AM
                )
            )
        )
        || (
            date.Date.Equals period.End.Date
            && (
                date.HalfDay.Equals period.End.HalfDay
                || (
                    date.HalfDay.Equals AM
                    && period.Start.HalfDay.Equals PM
                )
            )
        )

    let IsBoundaryOverlapping request currentRequest = 
        IsDateInPeriod request.Start currentRequest
        || IsDateInPeriod request.End currentRequest
        || IsDateInPeriod currentRequest.Start request
        || IsDateInPeriod currentRequest.End request

    let overlapWithAnyRequest (previousRequests: TimeOffRequest seq) request =
        previousRequests
        |> Seq.exists (fun currentRequest -> IsBoundaryOverlapping request currentRequest)

    let createRequest previousRequests request =
        if overlapWithAnyRequest previousRequests request then
            Error "Overlapping request"
        elif request.Start.Date <= DateTime.Today then
            Error "The request starts in the past"
        else
            Ok [RequestCreated request]

    let validateRequest requestState =
        match requestState with
        | PendingValidation request ->
            Ok [RequestValidated request]
        | _ ->
            Error "Request cannot be validated"

    let cancelRequest requestState =
        match requestState with
        | PendingValidation request ->
            Ok [RequestCancelled request]
        | _ ->
            Error "Request cannot be cancelled"

    let getActiveRequests =
        let userRequests = getAllRequests [] 
        userRequests
        |> Map.toSeq
        |> Seq.map (fun (_, state) -> state)
        |> Seq.where (fun state -> state.IsActive)
        |> Seq.map (fun state -> state.Request)

    let handleCommand (store: IStore<UserId, RequestEvent>) (command: Command) =
        let userId = command.UserId
        let stream = store.GetStream userId
        let events = stream.ReadAll()
        let userRequests = getAllRequests events

        match command with
        | RequestTimeOff request ->
            let activeRequests =
                userRequests
                |> Map.toSeq
                |> Seq.map (fun (_, state) -> state)
                |> Seq.where (fun state -> state.IsActive)
                |> Seq.map (fun state -> state.Request)

            createRequest activeRequests request

        | ValidateRequest (_, requestId) ->
            let requestState = defaultArg (userRequests.TryFind requestId) NotCreated
            validateRequest requestState
        | CancelRequest(_, requestId) ->
            let requestState = defaultArg (userRequests.TryFind requestId) NotCreated
            cancelRequest requestState