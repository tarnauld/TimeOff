module TimeOff.Server

open Suave

[<EntryPoint>]
let main args=
  startWebServer defaultConfig Routes.app
  0