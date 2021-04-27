
module GuashBrowser

open System
open PhotinoNET
open System.IO
open System.Reflection
open System.Text.Json
open Common


type Message = {Type: string; Body: string}


let sendMessage (wnd:PhotinoWindow) (message:Message) =
    let msg = JsonSerializer.Serialize(message)
    wnd.SendWebMessage(msg) |> ignore


let onMessage dmsgs onFinish onCancel (wnd:Object) (message:string) =
    let pwnd = wnd :?>PhotinoWindow
    let msg = JsonSerializer.Deserialize<Message>(message)
    match msg.Type with
    | "notifyLoaded" -> sendMessage pwnd {Type="bindData"; Body=JsonSerializer.Serialize(dmsgs)}
    | "notifyCancel" ->
        onCancel ()
    | "notifySubmit" ->
        let results = JsonSerializer.Deserialize<string array>(msg.Body)
        onFinish results
    | _ -> failwithf "Unknown msg type %s" msg.Type

let winConfig (options: PhotinoWindowOptions) =
    let asm = Assembly.GetExecutingAssembly()

    let load (url:string) (prefix:string) =
        let fname = url.Substring(prefix.Length)
        asm.GetManifestResourceStream($"guash_doquery.assets.{fname}")

    
    options.CustomSchemeHandlers.Add("resjs",
         CustomSchemeDelegate(fun url contentType ->
                        contentType <- "text/javascript"
                        load url "resjs:"))
    options.CustomSchemeHandlers.Add("rescss",
         CustomSchemeDelegate(fun url contentType ->
                        contentType <- "text/css"
                        load url "rescss:"))

let launchBrowser (dmsgs : DataMessage array) onFinish onCancel =
    let win = new PhotinoWindow("Hello Photino", Action<PhotinoWindowOptions>(winConfig))
    let asm = Assembly.GetExecutingAssembly()
    use stream = asm.GetManifestResourceStream("guash_doquery.assets.index.html")
    use sr = new StreamReader(stream)
    let text = sr.ReadToEnd()
    // printfn "content: %s" text

    win.RegisterWebMessageReceivedHandler(System.EventHandler<string>(onMessage dmsgs onFinish onCancel))
        .Resize(80, 80, "%")
        .Center()
        .LoadRawString(text)
        .WaitForClose()
