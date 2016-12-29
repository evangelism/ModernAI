open System.IO

let train_set = @"d:\TEMP\DigitReco\Data\trainingsample.csv"
let read_file fn = File.ReadAllLines(fn)
let data = read_file train_set
            |> Seq.skip 1
            |> Seq.map (fun s -> s.Split(','))
            |> Seq.map (fun a -> (a.[0]|>int,Array.map int a.[1..]))
let sqr x = x*x
let dist a b = Array.map2 (fun a b -> sqr(a-b)) a b |> Array.sum
let classify x = data |> Seq.minBy(fun (d,a) -> dist a x) |> fst

