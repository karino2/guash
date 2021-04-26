open System
open System.IO
open Argu

type Arguments =
    | [<AltCommandLine("-d")>] Debug
    | [<MainCommand; ExactlyOnce; Last>] File of file:string
with
    interface IArgParserTemplate with
        member arg.Usage =
            match arg with
            | Debug -> "Debug mode. Show log and keep intemediate tmp folder."
            | File _-> "Guash script file."


let getInput path =
    File.ReadAllLines(path)


let runGuash lines =
    let dmsgs = GuashShell.doBeforeAndGetDataMessage lines
    let onFinish results =
        GuashShell.doAfter lines results
        |> Console.WriteLine
        if not Common.debugMode then
            GuashShell.removeTempDir()
        Environment.Exit 0
    let onCancel () =
        GuashShell.removeTempDir ()
        printfn "Cancel!"
        Environment.Exit 1
    GuashBrowser.launchBrowser dmsgs onFinish onCancel

[<EntryPoint>]
let main argv =
    let parser = ArgumentParser.Create<Arguments>(programName = "guash")
    try
        let results = parser.Parse argv
        let arg = results.GetAllResults()

        let mutable path = ""

        match arg with
        |Debug::_ -> Common.debugMode <- true
        |(File f)::_ -> path <- f
        |_->()

        let lines = getInput path
        runGuash lines
        0 // return an integer exit code
    with
        | :? ArguParseException as ex ->
        printfn "%s" ex.Message
        1
