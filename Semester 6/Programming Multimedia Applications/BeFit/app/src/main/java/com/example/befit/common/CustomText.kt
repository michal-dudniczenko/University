package com.example.befit.common

import androidx.compose.material3.Text
import androidx.compose.runtime.Composable
import androidx.compose.ui.Modifier
import androidx.compose.ui.graphics.Color
import androidx.compose.ui.text.font.FontWeight
import androidx.compose.ui.unit.sp

@Composable
fun CustomText(
    text: String,
    color: Color = bright,
    fontSize: Int = mediumFontSize,
    isBoldFont: Boolean = true,
    modifier: Modifier = Modifier
) {
    Text(
        text = text,
        color = color,
        fontWeight = if (isBoldFont) FontWeight.Bold else FontWeight.Normal,
        fontSize = adaptiveWidth(fontSize).sp,
        modifier = modifier
    )
}