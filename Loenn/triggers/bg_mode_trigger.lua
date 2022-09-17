local bgModeTrigger = {}

bgModeTrigger.name = "bgSwitch/bgTrigger"
bgModeTrigger.placements = {
    {
        name = "on",
        data = {
            mode = true,
            persistent = false,
            playEffects = true
        }
    },
    {
        name = "off",
        data = {
            mode = false,
            persistent = false,
            playEffects = true
        }
    }
}

return bgModeTrigger