module TimeOff.RestAPI

open Suave.Successful

let JsonReturn str method = 
  OK (str + " " + method)