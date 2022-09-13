module BGmodeTrigger

using ..Ahorn, Maple

BGToggleTrigger(x::Integer, y::Integer, width::Integer=8, height::Integer=8, mode::Bool=true, persistant::Bool=true) = Trigger("bgSwitch/bgTrigger", x=x, y=y, width=width, height=height, mode=mode, persistant=persistant)

const placements = Dict{String, Ahorn.EntityPlacement}(
    "BG Switch (On)" => Ahorn.EntityPlacement(
        BGToggleTrigger,
        "rectangle",
        Dict{String, Any}(
            "mode" => true,
            "persistant" => true
        )
    ),
    "BG Switch (Off)" => Ahorn.EntityPlacement(
        BGToggleTrigger,
        "rectangle",
        Dict{String, Any}(
            "mode" => false,
            "persistant" => true
        )
    )
)

end
