package com.example.befit.constants

import androidx.compose.runtime.getValue
import androidx.compose.runtime.mutableStateOf
import androidx.compose.runtime.setValue

object AppThemes {
    const val DARK = "Dark"
    const val LIGHT = "Light"
    const val IDK = "Idk"
}

var appTheme by mutableStateOf<String>(AppThemes.DARK)