module GuashShell

open System.Diagnostics
open System.IO
open Common

let createTempDirPath temppath pidstr =
    Path.Combine(temppath,  $"guash_{pidstr}")

let tempDirPath =  createTempDirPath (Path.GetTempPath())  (Process.GetCurrentProcess().Id.ToString())

let createTempDir () =
    Directory.CreateDirectory(tempDirPath)

let removeTempDir () =
    Directory.Delete(tempDirPath, true)


// assume no indent
let findDoQueryIndex (lines: string array) =
    lines |> Array.findIndex (fun x-> x.StartsWith("guash_doquery"))

let toBeforeShell (lines:string array) (doqueryIndex:int) dirpath =
    Array.append [|"#!/usr/bin/env bash"; ""; $"export GUASH_DIR=\"{dirpath}\""|] lines.[1..(doqueryIndex-1)]


let shellExecute cmdname args =
    use proc = new Process()
    proc.StartInfo.FileName <- cmdname
    proc.StartInfo.Arguments <- args
    proc.StartInfo.RedirectStandardOutput <- true
    proc.Start() |> ignore
    let ret = proc.StandardOutput.ReadToEnd()
    proc.WaitForExit()
    ret

// Depend on chmod, but we need bash anyway!
let chmod path =
    shellExecute "chmod" $"+x {path}"

let runSh path = 
    shellExecute path ""

let doBeforeShell (lines: string array) =
    let qindex = findDoQueryIndex lines
    let beforeShLines = toBeforeShell lines qindex tempDirPath
    let beforeShPath = Path.Combine(tempDirPath, "before.sh")
    File.WriteAllLines(beforeShPath, beforeShLines)
    chmod beforeShPath |> ignore
    runSh beforeShPath |> ignore



let pathOne = Path.Combine(tempDirPath, "1.txt")
let pathTwo = Path.Combine(tempDirPath, "2.txt")

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

let doBeforeAndGetDataMessage (lines: string array) =
    createTempDir () |> ignore
    doBeforeShell lines
    createDataMessage pathOne pathTwo

//
// doAfter
//

// queryline="guash_doquery SRC DEST"
let queryToExports (queryline:string) (results:string array) =
    let args = queryline.Split(" ")
    if args.Length = 2 then
        [|$"export {args.[1]}=\"{results.[0]}\""|]
    else
        [|$"export {args.[1]}=\"{results.[0]}\""; $"export {args.[2]}=\"{results.[1]}\""|]

let toAfterShell (lines:string array) (doqueryIndex:int) (results: string array)=
    let exports = queryToExports lines.[doqueryIndex] results
    Array.concat [[|"#!/usr/bin/env bash"; ""|]; exports; lines.[(doqueryIndex+1)..]]


let doAfter (lines:string array) (results: string array) =
    let qindex = findDoQueryIndex lines
    let afterShPath = Path.Combine(tempDirPath, "after.sh")
    let afterShLines = toAfterShell lines qindex results
    File.WriteAllLines(afterShPath, afterShLines) 
    chmod afterShPath |> ignore
    runSh afterShPath |> ignore

let doAfterAndDeleteTemp (lines:string array) (results: string array) =
    doAfter lines results
    removeTempDir ()
