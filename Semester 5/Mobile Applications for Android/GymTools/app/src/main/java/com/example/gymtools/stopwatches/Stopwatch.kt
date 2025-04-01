package com.example.gymtools.stopwatches

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
import androidx.compose.foundation.layout.size
import androidx.compose.foundation.layout.width
import androidx.compose.foundation.shape.CircleShape
import androidx.compose.runtime.Composable
import androidx.compose.runtime.MutableLongState
import androidx.compose.runtime.MutableState
import androidx.compose.runtime.getValue
import androidx.compose.ui.Alignment
import androidx.compose.ui.Modifier
import androidx.compose.ui.draw.clip
import androidx.compose.ui.platform.testTag
import androidx.compose.ui.res.painterResource
import androidx.compose.ui.unit.dp
import com.example.gymtools.R
import com.example.gymtools.common.CustomText
import com.example.gymtools.common.STOPWATCH_BUTTON_SIZE
import com.example.gymtools.common.STOPWATCH_FONT_SIZE
import com.example.gymtools.common.adaptiveHeight
import com.example.gymtools.common.adaptiveWidth
import com.example.gymtools.common.darkBackground
import com.example.gymtools.common.formatTime

@Composable
fun Stopwatch(
    stopwatchId: Int,
    viewModelTime: MutableLongState,
    viewModelIsRunning: MutableState<Boolean>,
    viewModelStartPause: () -> Unit,
    viewModelReset: () -> Unit,
    modifier: Modifier = Modifier
) {
    val time by viewModelTime
    val isRunning by viewModelIsRunning

    Column(
        horizontalAlignment = Alignment.CenterHorizontally,
        modifier = modifier
            .fillMaxWidth()
    ) {
        CustomText(
            text = formatTime(time),
            fontSize = STOPWATCH_FONT_SIZE,
            modifier = Modifier
                .testTag("stopwatch${stopwatchId} text")
        )
        Spacer(Modifier.height(adaptiveHeight(16).dp))
        Row(
            horizontalArrangement = Arrangement.Center,
            modifier = Modifier
                .fillMaxWidth()
        ) {
            Box(
                contentAlignment = Alignment.Center,
                modifier = Modifier
                    .size(adaptiveWidth(STOPWATCH_BUTTON_SIZE).dp)
                    .clip(CircleShape)
                    .background(darkBackground)
                    .clickable { viewModelStartPause() }
                    .testTag("stopwatch${stopwatchId} start stop button")
            ) {
                Image(
                    painter = painterResource(id = if (isRunning) R.drawable.pause else R.drawable.play),
                    contentDescription = if (isRunning) "Pause stopwatch" else "Start stopwatch",
                    modifier = Modifier
                        .fillMaxHeight(0.35f)
                        .offset(x = if (isRunning) 0.dp else adaptiveWidth(3).dp)
                )
            }
            Spacer(Modifier.width(adaptiveWidth(40).dp))
            Box(
                contentAlignment = Alignment.Center,
                modifier = Modifier
                    .size(adaptiveWidth(STOPWATCH_BUTTON_SIZE).dp)
                    .clip(CircleShape)
                    .background(darkBackground)
                    .clickable { viewModelReset() }
                    .testTag("stopwatch${stopwatchId} reset button")
            ) {
                Image(
                    painter = painterResource(id = R.drawable.reset),
                    contentDescription = "Reset stopwatch",
                    modifier = Modifier
                        .fillMaxHeight(0.35f)
                )
            }
        }
    }
}

