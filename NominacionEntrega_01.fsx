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

let entregaLit = EntregaZona (lit, 2400)
let entregaGba = EntregaZona (gba, 1400)

let tf01 = ContratoTransporte  ("TF01", lit, 1000, 1.0) 
let tf02 = ContratoTransporte  ("TF02", lit, 2040, 1.0) 
let tf03 = ContratoTransporte  ("TF03", gba , 3000, 1.0) 
let tf04 = ContratoTransporte  ("TF03", gba, 2000, 1.0) 


let ntf01 = Decision.createContinuous "x1" 0.0 infinity
let ntf02 = Decision.createContinuous "x2" 0.0 infinity
let ntf03 = Decision.createContinuous "x3" 0.0 infinity
let ntf04 = Decision.createContinuous "x4" 0.0 infinity

// costo nominacion
let objExpr = ntf01 * tf01.Tarifa + ntf02 * tf02.Tarifa + ntf03 * tf03.Tarifa + ntf04 * tf04.Tarifa

let objective = Objective.create "Costo Tte" Minimize objExpr

let model = 
    Model.create objective
    |> Model.addConstraint (Constraint.create "CDC TF01" (ntf01 <== tf01.CDC))
    |> Model.addConstraint (Constraint.create "CDC TF01" (ntf02 <== tf01.CDC))
    