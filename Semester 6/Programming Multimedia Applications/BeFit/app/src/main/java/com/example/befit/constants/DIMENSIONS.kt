package com.example.befit.constants

import android.util.Log
import androidx.compose.runtime.Composable
import androidx.compose.ui.platform.LocalContext
import androidx.compose.ui.platform.LocalDensity


object ScreenDimensions {
    var currentHeightRatio: Float = 0f
    var currentWidthRatio: Float = 0f
}

@Composable
fun InitScreenDimensions() {
    val context = LocalContext.current
    val density = LocalDensity.current

    val displayMetrics = context.resources.displayMetrics
    val widthPx = displayMetrics.widthPixels
    val heightPx = displayMetrics.heightPixels

    // Konwersja px → dp
    val widthDp = with(density) { widthPx.toDp() }
    val heightDp = with(density) { heightPx.toDp() }

    Log.i("test", "$widthDp, $heightDp")

    // Obliczenie proporcji względem wymiarów projektowych
    ScreenDimensions.currentWidthRatio = widthDp.value / developmentDpWidth
    ScreenDimensions.currentHeightRatio = heightDp.value / developmentDpHeight
}

const val developmentDpHeight = 890f
const val developmentDpWidth = 411f

fun adaptiveHeight(height: Int): Int {
    return (ScreenDimensions.currentHeightRatio * height).toInt()
}

fun adaptiveWidth(width: Int): Int {
    return (ScreenDimensions.currentWidthRatio * width).toInt()
}


fun adaptiveHeight(height: Double): Int {
    return (ScreenDimensions.currentHeightRatio * height).toInt()
}

fun adaptiveWidth(width: Double): Int {
    return (ScreenDimensions.currentWidthRatio * width).toInt()
}


