open System
open Windows.Globalization
open Windows.Media.SpeechRecognition
open System.Text


task {
    use recognizer = new SpeechRecognizer()
    let session = recognizer.ContinuousRecognitionSession

    let mutable keepGoing = true
    let recognized = StringBuilder()

    session.add_ResultGenerated (fun session args ->
        let text = args.Result.Text
        printfn $"Heard: %s{text}"

        recognized.Append($" {text}") |> ignore

        if text.Contains("ready") || text.Contains("done") then
            keepGoing <- false)

    printfn "Getting Started..."
    let! _ = recognizer.CompileConstraintsAsync().AsTask()
    do! session.StartAsync().AsTask()
    printfn "Listening..."

    while keepGoing do
        do! Async.Sleep 1000

    do! session.StopAsync().AsTask()

    printfn "You said: %s" (recognized.ToString())
}
|> Async.AwaitTask
|> Async.RunSynchronously
