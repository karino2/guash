
open System.Diagnostics
open System.IO

Process.GetCurrentProcess().Id

Path.GetTempFileName()
Path.GetTempPath()

Path.Combine(Path.GetTempPath(),  Process.GetCurrentProcess().Id.ToString())

let createTempDirPath temppath pidstr =
    Path.Combine(temppath,  $"guash_{pidstr}")

let tempDirPath =  createTempDirPath (Path.GetTempPath())  (Process.GetCurrentProcess().Id.ToString())


// assume no indent
let findDoQueryIndex (lines: string array) =
    lines |> Array.findIndex (fun x-> x.StartsWith("guash_doquery"))

let toBeforeShell (lines:string array) (doqueryIndex:int) dirpath =
    Array.append [|"#!/usr/bin/env bash"; ""; $"GUASH_DIR=\"{dirpath}\""|] lines.[1..(doqueryIndex-1)]


let target = """#!/usr/bin/guash

ls -t ~/work/GitHub/karino2.github.io/_posts | guash_filter "Src file"
guash_readtext "Dest name"
guash_doquery SRC DEST

ln ~/work/GitHub/karino2.github.io/_posts/${SRC} ~/Google\ ドライブ/DriveText/TeFWiki/${DEST}.md
"""

target.Split('\n')

let lines = target.Split('\n')

findDoQueryIndex(lines)

lines.[4]


lines.[0..(4-1)]




toBeforeShell lines 4 tempDirPath |> String.concat "\n"


let shellExecute cmdname args=
    use proc = new Process()
    proc.StartInfo.FileName <- cmdname
    proc.StartInfo.Arguments <- args
    proc.StartInfo.RedirectStandardOutput <- true
    proc.Start() |> ignore
    let ret = proc.StandardOutput.ReadToEnd()
    proc.WaitForExit()
    ret

shellExecute "chmod" "+x /var/folders/rc/pfqbwsf5091fkry8yqybpkwr0000gn/T/guash_16602/test.sh"


shellExecute "/var/folders/rc/pfqbwsf5091fkry8yqybpkwr0000gn/T/guash_16602/test.sh" ""


let runShellScript path =
    use proc = new Process()
    proc.StartInfo.FileName <- "/var/folders/rc/pfqbwsf5091fkry8yqybpkwr0000gn/T/guash_16602/test.sh"
    proc.StartInfo.RedirectStandardOutput <- true
    proc.Start() |> ignore
    let ret = proc.StandardOutput.ReadToEnd()
    proc.WaitForExit()


let proc = new Process()
proc.StartInfo.FileName <- "/var/folders/rc/pfqbwsf5091fkry8yqybpkwr0000gn/T/guash_16602/test.sh"
proc.StartInfo.RedirectStandardOutput <- true
proc.Start() |> ignore
let ret = proc.StandardOutput.ReadToEnd()
proc.WaitForExit()

// result was "/Users/arinokazuma/work/GitHub/guash", perfect!


#load "Common.fs"

open Common

let pathOne = Path.Combine(tempDirPath, "1.txt")
let pathTwo = Path.Combine(tempDirPath, "2.txt")

File.Exists(pathTwo)

let txt2dmsg (lines: string array) =
    if lines.[0].StartsWith("TEXT") then
        {Type=COLUMN_TYPE_TEXT; Title=lines.[1]; Args=[||]}
    else
        {Type=COLUMN_TYPE_FILTER; Title=lines.[1]; Args=lines.[2..]}

let createDataMessage pathOne pathTwo =
    if File.Exists(pathTwo) then
        let dmsg1 = File.ReadAllLines(pathOne) |> txt2dmsg
        let dmsg2 = File.ReadAllLines(pathTwo) |> txt2dmsg
        [|dmsg1; dmsg2|]
    else
        let dmsg1 = File.ReadAllLines(pathOne) |> txt2dmsg
        [|dmsg1|]

createDataMessage pathOne pathTwo


File.ReadAllLines(pathOne) |> txt2dmsg
File.ReadAllLines(pathTwo) |> txt2dmsg


lines.[4]

let target = """#!/usr/bin/guash

ls -t ~/work/GitHub/karino2.github.io/_posts | guash_filter 1 "Src file"
guash_readtext 2 "Dest name"
guash_doquery SRC DEST

ln ~/work/GitHub/karino2.github.io/_posts/${SRC} ~/Google\ ドライブ/DriveText/TeFWiki/${DEST}.md
"""

target.Split("\n")



queryToExports "guash_doquery SRC DEST" [|"hoge";"ika"|]



toAfterShell lines 4 [|"hoge";"ika"|] |> String.concat "\n"



let queryArgs = lines.[4].Split(" ")
if queryArgs.Length = 2 then





open System.Text
open System.Text.Json.Serialization
open System.Text.Json

type Message = {Type: string; Args: string array}

JsonSerializer.Serialize({Type="Hoge"; Args=[|"1"; "2"; "3"|]})


JsonSerializer.Deserialize<Message>("{\"Type\":\"Hoge\",\"Args\":[\"1\",\"2\",\"3\"]}")

#r "nuget: Photino.NET, 1.1.6"

open PhotinoNET

let win = new PhotinoWindow("HelloPhotino")

let onMessage _ message =
    printfn "On message %s" message

win.RegisterWebMessageReceivedHandler(System.EventHandler<string>(onMessage))
win.Load("index.html")
win.WaitForClose()


"app:bulma.css".Substring("app:".Length)