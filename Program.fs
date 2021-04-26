open System
open System.IO
open Argu

type Arguments =
    | [<AltCommandLine("-d")>] Debug
    | [<AltCommandLine("-dev")>] Dev_Mode
    | [<MainCommand; ExactlyOnce; Last>] File of file:string
with
    interface IArgParserTemplate with
        member arg.Usage =
            match arg with
            | Debug -> "Debug mode. Show log and keep intemediate tmp folder."
            | Dev_Mode -> "Development mode. Use when launch from VS Code with F5."
            | File _-> "Guash script file."

let debugInput () =
    let target = """#!~/bin/guash

ls -t ~/work/GitHub/karino2.github.io/_posts | guash_filter 1 "Src file"
guash_readtext 2 "Dest name"
guash_doquery SRC DEST

ln ~/work/GitHub/karino2.github.io/_posts/${SRC} ~/Google\ ドライブ/DriveText/TeFWiki/${DEST}.md
"""
    target.Split("\n", StringSplitOptions.TrimEntries)


let getInput isDev path =
    if isDev then
        debugInput ()
    else
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

        let mutable isDev = false
        let mutable path = ""

        match arg with
        |Debug::_ -> Common.debugMode <- true
        |Dev_Mode::_ -> isDev <- true
        |(File f)::_ -> path <- f
        |_->()

        let lines = getInput isDev path
        runGuash lines
        0 // return an integer exit code
    with
        | :? ArguParseException as ex ->
        printfn "%s" ex.Message
        1
