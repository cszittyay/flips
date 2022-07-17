#r "nuget: Flips,2.4.8"
fsi.ShowDeclarationValues <- false
#load "Contratos.fs"
open System
open Contratos
open Flips
open Flips.Types


let nqn = Zona "NQN" 
let sal = Zona "SAL"
let tdf = Zona "TDF"
let lit = Zona "LIT"
let gba = Zona "GBA"

let zonasEntrega = [gba; lit]

let entregaLit = EntregaZona (lit, 2400)
let entregaGba = EntregaZona (gba, 1400)

let tf01 = ContratoTransporte  ("TF01", lit, 1000.0, 1.0, 70) 
let tf02 = ContratoTransporte  ("TF02", lit, 2040.0, 2.0, 70) 
let tf03 = ContratoTransporte  ("TF03", gba , 1000.0, 1.0, 0 ) 
let tf04 = ContratoTransporte  ("TF03", gba, 2000.0, 2.0, 0) 

// los contratos como lista
let contratosTte = [tf01; tf02; tf03; tf04]

let contratos =
   Map.empty.
    Add(tf01.Nemonico, tf01).
    Add(tf02.Nemonico, tf02).
    Add(tf03.Nemonico, tf03).
    Add(tf04.Nemonico, tf04)




let nomCtoTte =
    [for cto in contratos.Values do
        cto.Nemonico, Decision.createContinuous (sprintf "Nominado%s: " cto.Nemonico) 0.0 infinity]
    |> Map.ofList


// Create the Linear Expression for the objective
let objectiveExpression = List.sum [for cto in contratos.Keys -> contratos.[cto].Tarifa * nomCtoTte.[cto]]


let objective = Objective.create "Costo Tte" Minimize objectiveExpression


// Crear las constraints
let nomCDC =
    [for cto in contratos.Keys ->
        Constraint.create (sprintf "HastaCDC%s" cto) (nomCtoTte.[cto] <== contratos.[cto].CDC)]


// La entrega de los contratos igua a la de la zona
//let entregaZona =
//    [for zona in zonasEntrega ->
        

let model = 
    Model.create objective
    |> Model.addConstraints nomCDC

    
let result = Solver.solve Settings.basic model
printfn "%A" result