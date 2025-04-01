package com.example.gymtools.common

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
    modifier: Modifier = Modifier
) {
    Text(
        text = text,
        color = color,
        fontWeight = FontWeight.Bold,
        fontSize = adaptiveWidth(fontSize).sp,
        modifier = modifier
    )
}