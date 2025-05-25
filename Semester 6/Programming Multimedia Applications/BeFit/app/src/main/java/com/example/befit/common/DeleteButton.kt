package com.example.befit.common

import androidx.compose.foundation.Image
import androidx.compose.foundation.background
import androidx.compose.foundation.clickable
import androidx.compose.foundation.layout.Box
import androidx.compose.foundation.layout.Spacer
import androidx.compose.foundation.layout.aspectRatio
import androidx.compose.foundation.layout.fillMaxHeight
import androidx.compose.foundation.layout.width
import androidx.compose.foundation.shape.CircleShape
import androidx.compose.runtime.Composable
import androidx.compose.ui.Alignment
import androidx.compose.ui.Modifier
import androidx.compose.ui.draw.clip
import androidx.compose.ui.res.painterResource
import androidx.compose.ui.unit.dp
import com.example.befit.R

@Composable
fun DeleteButton(
    modifier: Modifier = Modifier,
    onClick: () -> Unit
) {
    Spacer(Modifier.width(12.dp))
    Box(
        contentAlignment = Alignment.Center,
        modifier = modifier
            .fillMaxHeight(0.95f)
            .aspectRatio(1f)
            .clip(CircleShape)
            .background(color = lightRed)
            .clickable(onClick = onClick)
    ) {
        Image(
            painter = painterResource(id = R.drawable.trash),
            contentDescription = "Delete icon",
            modifier = Modifier.fillMaxHeight(0.5f)
        )
    }
}