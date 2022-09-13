module bgModeToggle

using ..Ahorn, Maple

@mapdef Entity "bgSwitch/bgModeToggle" BGflag(x::Integer, y::Integer, onlyOff::Bool=false, onlyOn::Bool=false, persistent::Bool=false)

const placements = Ahorn.PlacementDict(
    "BG Mode Toggle (On)" => Ahorn.EntityPlacement(
        BGflag,
        "point",
        Dict{String, Any}(
            "onlyOn" => true
        )
    ),
    "BG Mode Toggle (Off)" => Ahorn.EntityPlacement(
        BGflag,
        "point",
        Dict{String, Any}(
            "onlyOff" => true
        )
    ),
    "BG Mode Toggle (Both)" => Ahorn.EntityPlacement(
        BGflag,
    ),
)

function switchSprite(entity::BGflag)
    onlyOff = get(entity.data, "onlyOff", false)
    onlyOn = get(entity.data, "onlyOn", false)
    
    if onlyOff
        return "objects/coreFlipSwitch/switch13.png"

    elseif onlyOn
        return "objects/coreFlipSwitch/switch15.png"

    else
        return "objects/coreFlipSwitch/switch01.png"
    end
end

function Ahorn.selection(entity::BGflag)
    x, y = Ahorn.position(entity)
    sprite = switchSprite(entity)

    return Ahorn.getSpriteRectangle(sprite, x, y)
end

function Ahorn.render(ctx::Ahorn.Cairo.CairoContext, entity::BGflag, room::Maple.Room)
    sprite = switchSprite(entity)

    Ahorn.drawSprite(ctx, sprite, 0, 0)
end

end
