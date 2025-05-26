package com.example.befit.common

import androidx.compose.foundation.background
import androidx.compose.foundation.clickable
import androidx.compose.foundation.horizontalScroll
import androidx.compose.foundation.layout.Arrangement
import androidx.compose.foundation.layout.Row
import androidx.compose.foundation.layout.Spacer
import androidx.compose.foundation.layout.fillMaxWidth
import androidx.compose.foundation.layout.height
import androidx.compose.foundation.layout.padding
import androidx.compose.foundation.rememberScrollState
import androidx.compose.foundation.shape.RoundedCornerShape
import androidx.compose.runtime.Composable
import androidx.compose.ui.Modifier
import androidx.compose.ui.draw.clip
import androidx.compose.ui.unit.dp
import com.example.befit.constants.Themes
import com.example.befit.constants.adaptiveWidth
import com.example.befit.constants.mediumFontSize

@Composable
fun FunctionalityRow(
    text: String,
    onClick: () -> Unit,
    modifier: Modifier = Modifier
) {
    Row(
        horizontalArrangement = Arrangement.Center,
        modifier = modifier
            .fillMaxWidth()
            .clip(RoundedCornerShape(adaptiveWidth(16).dp))
            .background(color = Themes.PRIMARY)
            .clickable(onClick = onClick)
            .padding(adaptiveWidth(16).dp)
    ) {
        Row(
            modifier = Modifier
                .fillMaxWidth()
                .horizontalScroll(rememberScrollState())
        ) {
            CustomText(
                text = text,
                fontSize = mediumFontSize,
            )
        }
    }
    Spacer(modifier = Modifier.height(adaptiveWidth(32).dp))
}