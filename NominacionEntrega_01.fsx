#r "nuget: Flips,2.4.8"
fsi.ShowDeclarationValues <- false
#load "Contratos.fs"
open System
open Contratos
open Flips
open Flips.Types
open Flips.SliceMap

let nqn = Zona "NQN" 
let sal = Zona "SAL"
let tdf = Zona "TDF"
let lit = Zona "LIT"
let gba = Zona "GBA"

let zonasEntrega = [gba; lit]


let entregaZona =
    Map.empty.
     Add(gba, 1400).
     Add(lit, 2300)



let tf01 = ContratoTransporte  ( Contrato "TF01", lit, 1000.0, 1.0, 70) 
let tf02 = ContratoTransporte  (Contrato "TF02", lit, 2040.0, 2.0, 70) 
let tf03 = ContratoTransporte  (Contrato "TF03", gba , 1000.0, 1.0, 0 ) 
let tf04 = ContratoTransporte  (Contrato "TF04", gba, 2000.0, 2.0, 0) 

// los contratos como lista
let contratosTte = [tf01; tf02; tf03; tf04]

let contratos =
   Map.empty.
    Add(tf01.Nemonico, tf01).
    Add(tf02.Nemonico, tf02).
    Add(tf03.Nemonico, tf03).
    Add(tf04.Nemonico, tf04)



// Usando DecisionBuilder
let nomCtoZona =
    DecisionBuilder "Nominado" {
        for zona in zonasEntrega do
        // Filtrar la zona de entrega dle contrato
        let ctoZona = contratos.Values |> Seq.filter (fun x -> x.ZonaEntrega = zona) |> Seq.map (fun x -> x.Nemonico)
        for cto in ctoZona  ->
             Continuous (0, infinity )
    } |> SMap2.ofSeq


// Create the Linear Expression for the objective
let objectiveExpression = List.sum [for cto in contratos.Keys -> sum(contratos.[cto].Tarifa * nomCtoZona.[All, cto])]


let objective = Objective.create "Costo Tte" Minimize objectiveExpression


// Crear las constraints
// MaxNom
let nomCDC = ConstraintBuilder "HastaCDD" { for cto in contratos.Keys -> sum(nomCtoZona.[All, cto]) <== contratos.[cto].CDC }

let nomZona = ConstraintBuilder "EntregZona" { for zona in entregaZona.Keys  -> sum(1.0 * nomCtoZona.[zona, All]) == float entregaZona.[zona]}


let model = 
    Model.create objective
    |> Model.addConstraints nomCDC
    |> Model.addConstraints nomZona

    
let result = Solver.solve Settings.basic model
printfn "%A" result