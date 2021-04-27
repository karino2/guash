module GuashShell

open System.IO
open Common

let txt2dmsg (lines: string array) =
    if lines.[0].StartsWith("TEXT") then
        {Type=COLUMN_TYPE_TEXT; Title=lines.[1]; Args=[||]}
    else
        {Type=COLUMN_TYPE_FILTER; Title=lines.[1]; Args=lines.[2..]}

let createDataMessage tempDirPath =
    let pathOne = Path.Combine(tempDirPath, "1.txt")
    let pathTwo = Path.Combine(tempDirPath, "2.txt")
    if File.Exists(pathTwo) then
        let dmsg1 = File.ReadAllLines(pathOne) |> txt2dmsg
        let dmsg2 = File.ReadAllLines(pathTwo) |> txt2dmsg
        [|dmsg1; dmsg2|]
    else
        let dmsg1 = File.ReadAllLines(pathOne) |> txt2dmsg
        [|dmsg1|]

