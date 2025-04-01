package com.example.gymtools.common

import androidx.compose.foundation.Image
import androidx.compose.foundation.background
import androidx.compose.foundation.clickable
import androidx.compose.foundation.layout.Box
import androidx.compose.foundation.layout.fillMaxSize
import androidx.compose.foundation.layout.size
import androidx.compose.foundation.shape.RoundedCornerShape
import androidx.compose.runtime.Composable
import androidx.compose.ui.Alignment
import androidx.compose.ui.Modifier
import androidx.compose.ui.graphics.Color
import androidx.compose.ui.res.painterResource
import androidx.compose.ui.unit.dp
import androidx.compose.ui.zIndex

@Composable
fun CustomFloatingButton(
    icon: Int,
    description: String,
    color: Color = bright,
    onClick: () -> Unit = {},
    modifier: Modifier = Modifier
) {
    Box(
        modifier = modifier
            .size(adaptiveWidth(FLOATING_BUTTON_SIZE).dp)
            .background(
                color = color,
                shape = RoundedCornerShape(adaptiveWidth(10).dp)
            )
            .zIndex(1f)
            .clickable(onClick = onClick)
    ) {
        Image(
            painter = painterResource(id = icon),
            contentDescription = description,
            modifier = Modifier
                .align(Alignment.Center)
                .fillMaxSize(0.5f)
        )
    }
}