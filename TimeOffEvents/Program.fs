module TimeOff.Server

open Suave
open Routes

[<EntryPoint>]
let main args=
  startWebServer defaultConfig app
  0