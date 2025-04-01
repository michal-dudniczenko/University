package com.example.gymtools.common

import androidx.compose.foundation.background
import androidx.compose.foundation.clickable
import androidx.compose.foundation.layout.Arrangement
import androidx.compose.foundation.layout.Row
import androidx.compose.foundation.layout.Spacer
import androidx.compose.foundation.layout.fillMaxWidth
import androidx.compose.foundation.layout.height
import androidx.compose.foundation.layout.padding
import androidx.compose.foundation.shape.RoundedCornerShape
import androidx.compose.runtime.Composable
import androidx.compose.ui.Modifier
import androidx.compose.ui.unit.dp

@Composable
fun Heading(
    text: String,
    isEditMode: Boolean = false,
    onClick: () -> Unit = {},
    modifier: Modifier = Modifier,
) {
    Row(
        horizontalArrangement = Arrangement.Center,
        modifier = modifier
            .fillMaxWidth()
            .background(
                color = if (isEditMode) editColor else bright,
                shape = RoundedCornerShape(adaptiveWidth(16).dp)
            )
            .clickable(
                onClick = onClick,
                enabled = isEditMode
            )
            .padding(adaptiveWidth(16).dp)
    ) {
        CustomText(
            text = text,
            fontSize = bigFontSize,
            color = if (isEditMode) bright else darkBackground
        )
    }
    Spacer(modifier = Modifier.height(adaptiveWidth(32).dp))
}