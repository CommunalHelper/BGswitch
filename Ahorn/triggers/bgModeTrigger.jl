module BGswitchBGModeTrigger

using ..Ahorn, Maple

@mapdef Trigger "bgSwitch/bgTrigger" BGModeTrigger(x::Integer, y::Integer, width::Integer=Maple.defaultTriggerWidth, height::Integer=Maple.defaultTriggerHeight,
    mode::Bool=true, persistent::Bool=false, playEffects::Bool=true) 

const placements = Ahorn.PlacementDict(
    "BG Mode Trigger (On, BGswitch)" => Ahorn.EntityPlacement(
        BGModeTrigger,
        "rectangle",
        Dict{String, Any}(
            "mode" => true,
            "persistent" => false,
            "playEffects" => true
        )
    ),
    "BG Mode Trigger (Off, BGswitch)" => Ahorn.EntityPlacement(
        BGModeTrigger,
        "rectangle",
        Dict{String, Any}(
            "mode" => false,
            "persistent" => false,
            "playEffects" => true
        )
    )
)

end
