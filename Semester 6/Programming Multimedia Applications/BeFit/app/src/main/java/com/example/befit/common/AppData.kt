package com.example.befit.common

import androidx.compose.runtime.mutableIntStateOf
import com.example.befit.R

val TOP_LEVEL_SCREENS = listOf(
    "Training programs",
    "Stopwatches",
    "Health",
    "Settings"
)
const val topLevelStartScreen = 0
var currentTopLevelScreen = mutableIntStateOf(topLevelStartScreen)

val HEALTH_SCREENS = listOf(
    "Health tools list",
    "Diet plans",
    "Weight history",
    "Calorie calculator",
    "Protein calculator",
    "BMI calculator",
    "Hydration calculator"
)
const val healthStartScreen = 0
var currentHealthScreen = mutableIntStateOf(healthStartScreen)

val SETTINGS_SCREENS = listOf(
    "Settings list"
)
const val settingsStartScreen = 0
var currentSettingsScreen = mutableIntStateOf(settingsStartScreen)


val SCREEN_ICON_FILE_NAME_WHITE = listOf(
    R.drawable.dumbbell_white,
    R.drawable.stopwatch_white,
    R.drawable.health_white,
    R.drawable.settings_white,
)

val SCREEN_ICON_FILE_NAME_BLACK = listOf(
    R.drawable.dumbbell_black,
    R.drawable.stopwatch_black,
    R.drawable.health_black,
    R.drawable.settings_black,
)

const val NAVBAR_HEIGHT = 64
const val STOPWATCH_BUTTON_SIZE = 60
const val FLOATING_BUTTON_SIZE = 75
const val STOPWATCH_FONT_SIZE = 64

const val mediumFontSize = 24
const val bigFontSize = 28
const val smallFontSize = 20

