package com.example.befit.stopwatches

import androidx.compose.foundation.Image
import androidx.compose.foundation.background
import androidx.compose.foundation.clickable
import androidx.compose.foundation.layout.Arrangement
import androidx.compose.foundation.layout.Box
import androidx.compose.foundation.layout.Column
import androidx.compose.foundation.layout.Row
import androidx.compose.foundation.layout.Spacer
import androidx.compose.foundation.layout.fillMaxHeight
import androidx.compose.foundation.layout.fillMaxWidth
import androidx.compose.foundation.layout.height
import androidx.compose.foundation.layout.offset
import androidx.compose.foundation.layout.padding
import androidx.compose.foundation.layout.size
import androidx.compose.foundation.layout.width
import androidx.compose.foundation.shape.CircleShape
import androidx.compose.foundation.shape.RoundedCornerShape
import androidx.compose.runtime.Composable
import androidx.compose.runtime.getValue
import androidx.compose.ui.Alignment
import androidx.compose.ui.Modifier
import androidx.compose.ui.draw.clip
import androidx.compose.ui.graphics.Color
import androidx.compose.ui.res.painterResource
import androidx.compose.ui.unit.dp
import androidx.navigation.NavHostController
import com.example.befit.R
import com.example.befit.common.CustomText
import com.example.befit.constants.STOPWATCH_BUTTON_SIZE
import com.example.befit.constants.STOPWATCH_FONT_SIZE
import com.example.befit.constants.StopwatchesRoutes
import com.example.befit.constants.adaptiveHeight
import com.example.befit.constants.adaptiveWidth
import com.example.befit.constants.bright
import com.example.befit.constants.darkBackground
import com.example.befit.constants.editColor
import com.example.befit.common.formatTime
import com.example.befit.constants.Strings
import com.example.befit.constants.lightRed
import com.example.befit.constants.smallFontSize

@Composable
fun Stopwatch(
    stopwatchState: StopwatchState,
    viewModel: StopwatchesViewModel,
    navController: NavHostController,
    modifier: Modifier = Modifier
) {
    val time by stopwatchState.time
    val isRunning by stopwatchState.isRunning
    val isEditMode by viewModel.isEditMode

    Column(
        horizontalAlignment = Alignment.CenterHorizontally
    ) {
        Row(
            horizontalArrangement = Arrangement.SpaceBetween,
            modifier = Modifier.fillMaxWidth(0.99f)
        ) {
            Row(
                horizontalArrangement = Arrangement.Center,
                modifier = Modifier
                    .clip(RoundedCornerShape(adaptiveWidth(10).dp))
                    .background(color = if (isEditMode) editColor else bright)
                    .padding(horizontal = adaptiveWidth(12).dp, vertical = adaptiveWidth(8).dp)
                    .clickable(
                        enabled = isEditMode,
                        onClick = { navController.navigate(StopwatchesRoutes.EDIT(stopwatchState.id)) }
                    )
            ) {
                CustomText(
                    text = stopwatchState.name,
                    fontSize = smallFontSize,
                    color = if (isEditMode) bright else darkBackground
                )
            }
            if (isEditMode) {
                Box(
                    contentAlignment = Alignment.Center,
                    modifier = modifier
                        .clip(RoundedCornerShape(adaptiveWidth(10).dp))
                        .background(color = lightRed)
                        .clickable(
                            onClick = {
                                if (viewModel.stopwatches.value.size == 1) {
                                    viewModel.isEditMode.value = false
                                }
                                viewModel.deleteStopwatch(stopwatchState.id)
                            }
                        )
                        .padding(horizontal = adaptiveWidth(16).dp, vertical = adaptiveWidth(8).dp)
                ) {
                    CustomText(
                        text = Strings.DELETE,
                        fontSize = smallFontSize,
                        color = Color.White,
                    )
                }
            }
        }
        Spacer(Modifier.height(adaptiveHeight(8).dp))
        Row(
            horizontalArrangement = Arrangement.SpaceBetween,
            verticalAlignment = Alignment.CenterVertically,
            modifier = Modifier
                .fillMaxWidth()
                .clip(RoundedCornerShape(adaptiveWidth(24).dp))
                .background(darkBackground)
                .padding(adaptiveWidth(16).dp)

        ) {
            CustomText(
                text = formatTime(time),
                fontSize = STOPWATCH_FONT_SIZE,
            )
            Spacer(Modifier.width(adaptiveWidth(20).dp))
            Row {
                Box(
                    contentAlignment = Alignment.Center,
                    modifier = Modifier
                        .size(adaptiveWidth(STOPWATCH_BUTTON_SIZE).dp)
                        .clip(CircleShape)
                        .background(bright)
                        .clickable { stopwatchState.startPause() }
                ) {
                    Image(
                        painter = painterResource(id = if (isRunning) R.drawable.pause else R.drawable.play),
                        contentDescription = if (isRunning) "Pause stopwatch" else "Start stopwatch",
                        modifier = Modifier
                            .fillMaxHeight(0.4f)
                            .offset(x = if (isRunning) 0.dp else adaptiveWidth(3).dp)
                    )
                }
                Spacer(Modifier.width(adaptiveWidth(16).dp))
                Box(
                    contentAlignment = Alignment.Center,
                    modifier = Modifier
                        .size(adaptiveWidth(STOPWATCH_BUTTON_SIZE).dp)
                        .clip(CircleShape)
                        .background(bright)
                        .clickable { stopwatchState.reset() }
                ) {
                    Image(
                        painter = painterResource(id = R.drawable.reset),
                        contentDescription = "Reset stopwatch",
                        modifier = Modifier
                            .fillMaxHeight(0.45f)
                    )
                }
            }
        }
    }
}



