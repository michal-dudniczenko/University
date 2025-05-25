package com.example.befit.health.waterintakecalculator

import androidx.compose.foundation.background
import androidx.compose.foundation.layout.Column
import androidx.compose.foundation.layout.Spacer
import androidx.compose.foundation.layout.height
import androidx.compose.foundation.layout.padding
import androidx.compose.foundation.shape.RoundedCornerShape
import androidx.compose.material3.Text
import androidx.compose.runtime.Composable
import androidx.compose.ui.Alignment
import androidx.compose.ui.Modifier
import androidx.compose.ui.draw.clip
import androidx.compose.ui.graphics.Color
import androidx.compose.ui.text.SpanStyle
import androidx.compose.ui.text.buildAnnotatedString
import androidx.compose.ui.text.font.FontWeight
import androidx.compose.ui.text.withStyle
import androidx.compose.ui.unit.dp
import androidx.compose.ui.unit.sp
import com.example.befit.constants.Strings
import com.example.befit.constants.adaptiveWidth
import java.util.Locale

@Composable
fun WaterIntakeResult(
    waterIntake: Float,
    modifier: Modifier = Modifier
) {
    Column(
        horizontalAlignment = Alignment.CenterHorizontally,
        modifier = modifier
            .clip(RoundedCornerShape(30.dp))
            .background(color = Color(33, 150, 243, 255))
            .padding(horizontal = 24.dp, vertical = 16.dp)
    ) {
        Text(
            text = buildAnnotatedString {
                withStyle(style = SpanStyle(fontWeight = FontWeight.Normal)) {
                    append(Strings.YOU_SHOULD_DRINK + " ")
                }
                withStyle(style = SpanStyle(fontWeight = FontWeight.ExtraBold)) {
                    append(Strings.AT_LEAST)
                }
            },
            color = Color.White,
            fontSize = adaptiveWidth(18).sp
        )
        Spacer(Modifier.height(8.dp))
        Text(
            text = String.format(Locale.US, "%.1f", waterIntake) + " " + Strings.LITERS,
            color = Color.White,
            fontWeight = FontWeight.ExtraBold,
            fontSize = adaptiveWidth(28).sp
        )
        Spacer(Modifier.height(8.dp))
        Text(
            text = buildAnnotatedString {
                withStyle(style = SpanStyle(fontWeight = FontWeight.Normal)) {
                    append(Strings.OF_WATER + " ")
                }
                withStyle(style = SpanStyle(fontWeight = FontWeight.ExtraBold)) {
                    append(Strings.EVERY_DAY)
                }
                withStyle(style = SpanStyle(fontWeight = FontWeight.Normal)) {
                    append(".")
                }
            },
            color = Color.White,
            fontSize = adaptiveWidth(18).sp
        )

    }
}