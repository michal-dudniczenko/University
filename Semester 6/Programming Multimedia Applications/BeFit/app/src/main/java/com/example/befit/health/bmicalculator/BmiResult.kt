package com.example.befit.health.bmicalculator

import androidx.compose.foundation.background
import androidx.compose.foundation.layout.Arrangement
import androidx.compose.foundation.layout.Column
import androidx.compose.foundation.layout.Row
import androidx.compose.foundation.layout.Spacer
import androidx.compose.foundation.layout.height
import androidx.compose.foundation.layout.padding
import androidx.compose.foundation.layout.width
import androidx.compose.foundation.shape.RoundedCornerShape
import androidx.compose.material3.Text
import androidx.compose.runtime.Composable
import androidx.compose.ui.Alignment
import androidx.compose.ui.Modifier
import androidx.compose.ui.draw.clip
import androidx.compose.ui.graphics.Color
import androidx.compose.ui.text.font.FontWeight
import androidx.compose.ui.unit.dp
import androidx.compose.ui.unit.sp
import com.example.befit.constants.Strings
import com.example.befit.constants.adaptiveWidth
import java.util.Locale

@Composable
fun BmiResult(
    bmi: Float,
    modifier: Modifier = Modifier
) {
    val backgroundColor: Color
    val bottomText: String

    if (bmi > 40) {
        backgroundColor = Color(119, 0, 0, 255)
        bottomText = Strings.OBESE_CLASS_III
    } else if (bmi > 35) {
        backgroundColor = Color(208, 37, 37, 255)
        bottomText = Strings.OBESE_CLASS_II
    } else if (bmi > 30) {
        backgroundColor = Color(234, 77, 22, 255)
        bottomText = Strings.OBESE_CLASS_I
    } else if (bmi > 25) {
        backgroundColor = Color(218, 134, 31, 255)
        bottomText = Strings.OVERWEIGHT
    } else if (bmi > 18.5) {
        backgroundColor = Color(70, 150, 43, 255)
        bottomText = Strings.NORMAL_WEIGHT
    } else if (bmi > 17) {
        backgroundColor = Color(218, 134, 31, 255)
        bottomText = Strings.MILD_UNDERWEIGHT
    } else if (bmi > 16) {
        backgroundColor = Color(234, 77, 22, 255)
        bottomText = Strings.MODERATE_UNDERWEIGHT
    } else {
        backgroundColor = Color(208, 37, 37, 255)
        bottomText = Strings.SEVERE_UNDERWEIGHT
    }

    Column(
        horizontalAlignment = Alignment.CenterHorizontally,
        modifier = modifier
            .clip(RoundedCornerShape(30.dp))
            .background(color = backgroundColor)
            .padding(horizontal = 30.dp, vertical = 16.dp)
    ) {
        Row(
            horizontalArrangement = Arrangement.Center,
            verticalAlignment = Alignment.CenterVertically,
        ) {
            Text(
                text = Strings.YOUR_BMI_IS,
                color = Color.White,
                fontWeight = FontWeight.Bold,
                fontSize = adaptiveWidth(20).sp,
            )
            Spacer(Modifier.width(8.dp))
            Text(
                text = String.format(Locale.US, "%.1f", bmi),
                color = Color.White,
                fontWeight = FontWeight.ExtraBold,
                fontSize = adaptiveWidth(24).sp
            )
        }
        Spacer(Modifier.height(8.dp))
        Text(
            text = bottomText,
            color = Color.White,
            fontWeight = FontWeight.Normal,
            fontSize = adaptiveWidth(20).sp
        )
    }
}