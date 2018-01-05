module TimeOff.Routes

open Suave
open Suave.Filters
open Suave.Operators
open System
open Commands

let app = 
    choose
      [ GET >=> choose 
      [ 
        //* User *
        //Route: user ask for a paid leave.
        path "/userPaidLeaveAsk" >=> 
          addHoliday 1 { Date = DateTime(2018, 12, 30); HalfDay = AM } { Date = DateTime(2018, 12, 30); HalfDay = PM }
        //Route: user can list his own paid leave.
        path "/userPaidLeaves" >=> 
          getHoliday
        //Route: user cancel a pay leave.
        path "/userPaidLeaveCancel" >=> 
          cancelHoliday 1 { Date = DateTime(2018, 12, 30); HalfDay = AM } { Date = DateTime(2018, 12, 30); HalfDay = PM }
        
        //* Admin *
        //Route: admin accepts a pay leave for a user.
        path "/adminPaidLeaveAccept" >=> 
          acceptHoliday 1 { Date = DateTime(2018, 12, 30); HalfDay = AM } { Date = DateTime(2018, 12, 30); HalfDay = PM }
        //Route: admin refuses a pay leave for a user.
        path "/adminPaidLeavesRefuse" >=> 
          refuseHoliday
        //Route: admin cancel a pay leave for a user.
        path "/adminPaidLeavesCancel" >=> 
          cancelHoliday 1 { Date = DateTime(2018, 12, 30); HalfDay = AM } { Date = DateTime(2018, 12, 30); HalfDay = PM }
        //Route: admin lists all pay leaves
        path "/adminPaidLeaves" >=> 
          listHoliday 1 { Date = DateTime(2018, 12, 30); HalfDay = AM } { Date = DateTime(2018, 12, 30); HalfDay = PM }]]