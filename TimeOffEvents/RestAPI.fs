module TimeOff.RestAPI

open Suave
open Suave.Json
open Suave.Successful
open System.Runtime.Serialization

let JsonReturn str method = 
  OK (str + " " + method)