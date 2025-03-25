package com.example.gymtools.temp

import androidx.compose.animation.core.animateDpAsState
import androidx.compose.animation.core.tween
import androidx.compose.foundation.background
import androidx.compose.foundation.clickable
import androidx.compose.foundation.layout.Box
import androidx.compose.foundation.layout.Column
import androidx.compose.foundation.layout.Spacer
import androidx.compose.foundation.layout.fillMaxHeight
import androidx.compose.foundation.layout.fillMaxSize
import androidx.compose.foundation.layout.fillMaxWidth
import androidx.compose.foundation.layout.height
import androidx.compose.foundation.layout.offset
import androidx.compose.foundation.layout.padding
import androidx.compose.foundation.layout.size
import androidx.compose.foundation.shape.CircleShape
import androidx.compose.foundation.shape.RoundedCornerShape
import androidx.compose.runtime.Composable
import androidx.compose.runtime.getValue
import androidx.compose.runtime.mutableStateOf
import androidx.compose.runtime.remember
import androidx.compose.runtime.setValue
import androidx.compose.ui.Alignment
import androidx.compose.ui.Modifier
import androidx.compose.ui.unit.dp
import androidx.navigation.NavHostController
import com.example.gymtools.R
import com.example.gymtools.common.CustomFloatingButton
import com.example.gymtools.common.CustomText
import com.example.gymtools.common.Heading
import com.example.gymtools.common.adaptiveWidth
import com.example.gymtools.common.bright
import com.example.gymtools.common.darkBackground

@Composable
fun AnimationScreen(
    navController: NavHostController,
    modifier: Modifier = Modifier
) {
    var isMoved by remember { mutableStateOf(false) }

    // Animate horizontal position
    val offsetX by animateDpAsState(
        targetValue = if (isMoved) 200.dp else 0.dp,
        animationSpec = tween(durationMillis = 1000) // Animation duration
    )

    Box(
        modifier = modifier
            .fillMaxSize()
    ) {
        CustomFloatingButton(
            icon = R.drawable.back,
            description = "Back button",
            onClick = {
                navController.navigate("Settings")
            },
            modifier = Modifier
                .align(Alignment.BottomStart)
                .offset(x = adaptiveWidth(32).dp, y = adaptiveWidth(-32).dp)
        )
        Column(
            horizontalAlignment = Alignment.CenterHorizontally,
            modifier = modifier
                .fillMaxWidth(0.8f)
                .fillMaxHeight(0.9f)
                .align(Alignment.Center)
        ) {
            Heading("Animation")
            Column(
                modifier = Modifier
                    .fillMaxWidth()
            ) {
                Box(
                    modifier = Modifier
                        .offset(x = offsetX) // Apply horizontal offset
                        .size(75.dp)
                        .background(bright, CircleShape)
                )
            }

            Spacer(modifier = Modifier.height(16.dp))

            // Button to trigger animation
            Box(
                contentAlignment = Alignment.Center,
                modifier = Modifier
                    .fillMaxWidth()
                    .background(
                        color = darkBackground,
                        shape = RoundedCornerShape(16.dp)
                    )
                    .clickable {
                        isMoved = !isMoved
                    }
                    .padding(16.dp)

            ) {
                CustomText(if (isMoved) "Reset" else "Move")
            }
        }
    }
}