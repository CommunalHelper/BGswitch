module BGswitchBGModeToggle

using ..Ahorn, Maple

@mapdef Entity "bgSwitch/bgModeToggle" BGModeToggle(x::Integer, y::Integer, onlyOff::Bool=false, onlyOn::Bool=false, persistent::Bool=false)

const placements = Ahorn.PlacementDict(
    "BG Mode Toggle (On, BGswitch)" => Ahorn.EntityPlacement(
        BGModeToggle,
        "point",
        Dict{String, Any}(
            "onlyOn" => true
        )
    ),
    "BG Mode Toggle (Off, BGswitch)" => Ahorn.EntityPlacement(
        BGModeToggle,
        "point",
        Dict{String, Any}(
            "onlyOff" => true
        )
    ),
    "BG Mode Toggle (Both, BGswitch)" => Ahorn.EntityPlacement(
        BGModeToggle,
    ),
)

function switchSprite(entity::BGModeToggle)
    onlyOff = get(entity.data, "onlyOff", false)
    onlyOn = get(entity.data, "onlyOn", false)
    
    if onlyOff
        return "objects/BGswitch/bgflipswitch/switch13.png"

    elseif onlyOn
        return "objects/BGswitch/bgflipswitch/switch15.png"

    else
        return "objects/BGswitch/bgflipswitch/switch01.png"
    end
end

function Ahorn.selection(entity::BGModeToggle)
    x, y = Ahorn.position(entity)
    sprite = switchSprite(entity)

    return Ahorn.getSpriteRectangle(sprite, x, y)
end

function Ahorn.render(ctx::Ahorn.Cairo.CairoContext, entity::BGModeToggle, room::Maple.Room)
    sprite = switchSprite(entity)

    Ahorn.drawSprite(ctx, sprite, 0, 0)
end

end
