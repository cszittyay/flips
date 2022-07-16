
module Contratos 
open System


type Zona =  Zona of string

type ContratoTransporte (nemonico, zonaEntrega, CDC, tarifa ) = 
        member this.Nemonico = nemonico
        member this.ZonaEntrega:Zona = zonaEntrega
        member this.CDC: float = CDC
        member this.Tarifa:double = tarifa



type EntregaZona (zona, entrega) =
        member this.Zona:Zona  = zona
        member this.Entrega: float = entrega
    