package com.example.befit.health.caloriecalculator

import androidx.compose.foundation.background
import androidx.compose.foundation.layout.Arrangement
import androidx.compose.foundation.layout.Column
import androidx.compose.foundation.layout.Row
import androidx.compose.foundation.layout.Spacer
import androidx.compose.foundation.layout.fillMaxWidth
import androidx.compose.foundation.layout.height
import androidx.compose.foundation.layout.padding
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
import com.example.befit.constants.adaptiveWidth

@Composable
fun ResultRow(
    text: String,
    kcal: Int?,
    backgroundColor: Color,
    fontColor: Color = Color.White,
    modifier: Modifier = Modifier,
    bottomText: String? = null
) {
    Row(
        verticalAlignment = Alignment.CenterVertically,
        horizontalArrangement = Arrangement.SpaceBetween,
        modifier = modifier
            .fillMaxWidth()
            .clip(RoundedCornerShape(20.dp))
            .background(color = backgroundColor)
            .padding(14.dp)
    ) {
        Column {
            Text(
                text = text,
                color = fontColor,
                fontWeight = FontWeight.Bold,
                fontSize = adaptiveWidth(17).sp,
            )
            if (bottomText != null) {
                Text(
                    text = bottomText,
                    color = fontColor,
                    fontWeight = FontWeight.Normal,
                    fontSize = adaptiveWidth(15).sp
                )
            }
        }
        Text(
            text = "$kcal kcal",
            color = fontColor,
            fontWeight = FontWeight.ExtraBold,
            fontSize = adaptiveWidth(24).sp
        )
    }
    Spacer(Modifier.height(20.dp))
}