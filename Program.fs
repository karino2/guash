open System

let debugInput () =
    let target = """#!/usr/bin/guash

ls -t ~/work/GitHub/karino2.github.io/_posts | guash_filter 1 "Src file"
guash_readtext 2 "Dest name"
guash_doquery SRC DEST

ln ~/work/GitHub/karino2.github.io/_posts/${SRC} ~/Google\ ドライブ/DriveText/TeFWiki/${DEST}.md
"""
    target.Split("\n", StringSplitOptions.TrimEntries)


[<EntryPoint>]
let main argv =
    // Console.ReadLine
    let lines = debugInput ()
    let dmsgs = GuashShell.doBeforeAndGetDataMessage lines
    let onFinish results =
        GuashShell.doAfterAndDeleteTemp lines results
        // GuashShell.doAfter lines results
        Environment.Exit 0
    let onCancel () =
        GuashShell.removeTempDir ()
        printfn "Cancel!"
        Environment.Exit 1
    GuashBrowser.launchBrowser dmsgs onFinish onCancel
    0 // return an integer exit code