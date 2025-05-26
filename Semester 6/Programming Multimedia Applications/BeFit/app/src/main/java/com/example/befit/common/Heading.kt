package com.example.befit.common

import androidx.compose.foundation.background
import androidx.compose.foundation.layout.Arrangement
import androidx.compose.foundation.layout.Row
import androidx.compose.foundation.layout.Spacer
import androidx.compose.foundation.layout.fillMaxWidth
import androidx.compose.foundation.layout.height
import androidx.compose.foundation.layout.padding
import androidx.compose.foundation.shape.RoundedCornerShape
import androidx.compose.runtime.Composable
import androidx.compose.ui.Modifier
import androidx.compose.ui.draw.clip
import androidx.compose.ui.unit.dp
import com.example.befit.constants.Themes
import com.example.befit.constants.adaptiveWidth
import com.example.befit.constants.bigFontSize

@Composable
fun Heading(
    text: String,
    modifier: Modifier = Modifier
) {
    Row(
        horizontalArrangement = Arrangement.Center,
        modifier = modifier
            .fillMaxWidth()
            .clip(RoundedCornerShape(adaptiveWidth(16).dp))
            .background(Themes.SECONDARY)
            .padding(adaptiveWidth(16).dp)
    ) {
        CustomText(
            text = text,
            fontSize = bigFontSize,
            color = Themes.ON_SECONDARY
        )
    }
    Spacer(modifier = Modifier.height(adaptiveWidth(32).dp))
}