module TimeOff.Routes

open Suave
open Suave.Filters
open Suave.Operators
open Commands
open System

let app = 
    choose
      [ GET >=> choose 
      [ path "/add" >=> 
          addHoliday 1 { Date = DateTime(2018, 12, 30); HalfDay = AM } { Date = DateTime(2018, 12, 30); HalfDay = PM }
        path "/get" >=> 
          getHoliday ]]