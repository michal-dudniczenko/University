package com.example.gymtools.common

import androidx.compose.runtime.mutableIntStateOf
import com.example.gymtools.R

val SCREENS = listOf(
    "Stopwatches",
    "Training programs",
    "Weight manager"
)

val SCREEN_ICON_FILE_NAME_WHITE = listOf(
    R.drawable.stopwatch_white,
    R.drawable.notepad_white,
    R.drawable.scale_white,
)

val SCREEN_ICON_FILE_NAME_BLACK = listOf(
    R.drawable.stopwatch_black,
    R.drawable.notepad_black,
    R.drawable.scale_black,
)

const val startScreen = 1
var currentScreen = mutableIntStateOf(startScreen)

const val NAVBAR_HEIGHT = 64
const val STOPWATCH_BUTTON_SIZE = 85
const val FLOATING_BUTTON_SIZE = 75
const val STOPWATCH_FONT_SIZE = 110

const val mediumFontSize = 24
const val bigFontSize = 32
const val smallFontSize = 20

