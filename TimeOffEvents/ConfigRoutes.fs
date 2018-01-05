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
        pathScan "/userPaidLeaveAsk/%d/%d/%d/%d/%d/%d/%d" (fun(id, yb, mb, db, ye, me, de) -> addHoliday id { Date = DateTime(yb, mb, db); HalfDay = AM } { Date = DateTime(ye, me, de); HalfDay = PM })
        //Route: user can list his own paid leave.
        path "/userPaidLeaves" >=> 
          getHoliday
        //Route: user cancel a pay leave.
        pathScan "/userPaidLeaveCancel/%d/%d/%d/%d/%d/%d/%d" (fun(id, yb, mb, db, ye, me, de) -> cancelHoliday id { Date = DateTime(yb, mb, db); HalfDay = AM } { Date = DateTime(ye, me, de); HalfDay = PM }) 
        
        //* Admin *
        //Route: admin accepts a pay leave for a user.
        pathScan "/adminPaidLeaveAccept/%d/%d/%d/%d/%d/%d/%d" (fun(id, yb, mb, db, ye, me, de) -> acceptHoliday id { Date = DateTime(yb, mb, db); HalfDay = AM } { Date = DateTime(ye, me, de); HalfDay = PM }) 
        //Route: admin refuses a pay leave for a user.
        pathScan "/adminPaidLeavesRefuse/%d/%d/%d/%d/%d/%d/%d" (fun(id, yb, mb, db, ye, me, de) -> refuseHoliday id { Date = DateTime(yb, mb, db); HalfDay = AM } { Date = DateTime(ye, me, de); HalfDay = PM }) 
        //Route: admin cancel a pay leave for a user.
        pathScan "/adminPaidLeavesCancel/%d/%d/%d/%d/%d/%d/%d" (fun(id, yb, mb, db, ye, me, de) -> cancelHoliday id { Date = DateTime(yb, mb, db); HalfDay = AM } { Date = DateTime(ye, me, de); HalfDay = PM }) 
        //Route: admin lists all pay leaves
        pathScan "/adminPaidLeaves/%d/%d/%d/%d/%d/%d/%d" (fun(id, yb, mb, db, ye, me, de) -> listHoliday id { Date = DateTime(yb, mb, db); HalfDay = AM } { Date = DateTime(ye, me, de); HalfDay = PM }) ]]