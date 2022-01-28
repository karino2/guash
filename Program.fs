open System
open System.IO
open Argu

type Arguments =
    | [<AltCommandLine("-k")>] Keep
    | [<AltCommandLine("-dev")>] Dev_Mode

with
    interface IArgParserTemplate with
        member arg.Usage =
            match arg with
            | Keep -> "Keep GUASH_DIR on exit."
            | Dev_Mode -> "Development mode. Use hardcoded value instead of env value for F5 execution from VS Code."


let getGuashDir isDev =
    if isDev then
        FileInfo("test").FullName
    else
        Environment.GetEnvironmentVariable("GUASH_DIR")

let runGuash guashdir deleteOnExit =
    let dmsgs = GuashShell.createDataMessage guashdir
    let onFinish (results:string array) =
        results |> Array.iter Console.WriteLine
        if deleteOnExit then
            Directory.Delete(guashdir, true)
        Environment.Exit 0
    let onCancel () =
        if deleteOnExit then
            Directory.Delete(guashdir, true)
        Environment.Exit 1
    GuashBrowser.launchBrowser dmsgs onFinish onCancel

[<EntryPoint>]
let main argv =
    let parser = ArgumentParser.Create<Arguments>(programName = "guash")
    try
        let results = parser.Parse argv

        let isDev = results.Contains Dev_Mode
        let guashdir = getGuashDir isDev
        let deleteOnExit = not (isDev || (results.Contains Keep))

        runGuash guashdir deleteOnExit
        0 // return an integer exit code
    with
        | :? ArguParseException as ex ->
        printfn "%s" ex.Message
        1
