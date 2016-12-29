namespace DigitReco

open System
open System.IO

type IClassifier =
    abstract member Classify : int[] -> int

type Classifier() = 
    let train_set = @"d:\TEMP\DigitReco\Data\train.csv"
    let read_file fn = File.ReadAllLines(fn)
    let data = read_file train_set
               |> Seq.skip 1
               |> Seq.map (fun s -> s.Split(','))
               |> Seq.map (fun a -> (a.[0]|>int,Array.map int a.[1..]))
               // |> Seq.filter (fun (a,b) -> a>0) // Array.averageBy (float) b > 10.0)
               |> Seq.toArray
    let sqr x = x*x
    let dist a b = Array.map2 (fun a b -> sqr(a-b)) a b |> Array.sum
    let classify x = data |> Array.minBy(fun (d,a) -> dist a x) |> fst
    member this.GetData() = data
    interface IClassifier with
        member this.Classify(x : int[]) = classify x
