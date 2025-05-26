package com.example.befit.common

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
import androidx.compose.ui.draw.clip
import androidx.compose.ui.graphics.Color
import androidx.compose.ui.res.painterResource
import androidx.compose.ui.unit.dp
import androidx.compose.ui.zIndex
import com.example.befit.constants.FLOATING_BUTTON_SIZE
import com.example.befit.constants.Themes
import com.example.befit.constants.adaptiveWidth

@Composable
fun CustomFloatingButton(
    icon: Int,
    description: String,
    color: Color = Themes.SECONDARY,
    onClick: () -> Unit = {},
    modifier: Modifier = Modifier
) {
    Box(
        modifier = modifier
            .size(FLOATING_BUTTON_SIZE.dp)
            .clip(RoundedCornerShape(10.dp))
            .background(color = color)
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