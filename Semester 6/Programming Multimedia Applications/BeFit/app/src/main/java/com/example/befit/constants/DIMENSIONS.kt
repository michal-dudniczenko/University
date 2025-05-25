package com.example.befit.constants

import androidx.compose.runtime.Composable
import androidx.compose.ui.platform.LocalContext
import kotlin.math.roundToInt


object ScreenDimensions {
    var currentHeightRatio: Float = 0f
    var currentWidthRatio: Float = 0f
}

@Composable
fun InitScreenDimensions() {
    val displayMetrics = LocalContext.current.resources.displayMetrics

    val width = displayMetrics.widthPixels
    val height = displayMetrics.heightPixels

    ScreenDimensions.currentHeightRatio = height / developmentHeight.toFloat()
    ScreenDimensions.currentWidthRatio = width / developmentWidth.toFloat()

}

const val developmentHeight = 2337
const val developmentWidth = 1080

fun adaptiveHeight(height: Int): Int {
    return (ScreenDimensions.currentHeightRatio * height).roundToInt()
}

fun adaptiveWidth(width: Int): Int {
    return (ScreenDimensions.currentWidthRatio * width).roundToInt()
}


fun adaptiveHeight(height: Double): Int {
    return (ScreenDimensions.currentHeightRatio * height).roundToInt()
}

fun adaptiveWidth(width: Double): Int {
    return (ScreenDimensions.currentWidthRatio * width).roundToInt()
}


