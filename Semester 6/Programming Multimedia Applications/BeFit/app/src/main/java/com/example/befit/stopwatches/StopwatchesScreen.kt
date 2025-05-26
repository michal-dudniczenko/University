package com.example.befit.stopwatches

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
import androidx.compose.foundation.shape.RoundedCornerShape
import androidx.compose.foundation.verticalScroll
import androidx.compose.runtime.Composable
import androidx.compose.runtime.getValue
import androidx.compose.runtime.setValue
import androidx.compose.ui.Alignment
import androidx.compose.ui.Modifier
import androidx.compose.ui.draw.clip
import androidx.compose.ui.unit.dp
import androidx.navigation.NavHostController
import com.example.befit.common.CustomFloatingButton
import com.example.befit.common.CustomText
import com.example.befit.constants.PADDING_BOTTOM
import com.example.befit.constants.Strings
import com.example.befit.constants.Themes
import com.example.befit.constants.adaptiveWidth
import com.example.befit.constants.bigFontSize

@Composable
fun StopwatchesScreen(
    viewModel: StopwatchesViewModel,
    navController: NavHostController,
    modifier: Modifier = Modifier
) {
    val stopwatches by viewModel.stopwatches
    var isEditMode by viewModel.isEditMode
    val scrollState by viewModel.scrollState

    Box(
        modifier = Modifier.fillMaxSize()
    ) {
        if (stopwatches.isEmpty()) {
            CustomFloatingButton(
                icon = Themes.ADD_ON_SECONDARY,
                description = "Add button",
                onClick = { viewModel.addStopwatch() },
                modifier = Modifier
                    .align(Alignment.BottomEnd)
                    .offset(x = (-30).dp, y = (-30).dp)
            )
        } else {
            CustomFloatingButton(
                icon = if (isEditMode) Themes.EDIT_ON_EDIT else Themes.EDIT_ON_SECONDARY,
                description = "Edit mode",
                color = if (isEditMode) Themes.EDIT_COLOR else Themes.SECONDARY,
                onClick = { isEditMode = !isEditMode },
                modifier = Modifier
                    .align(Alignment.BottomEnd)
                    .offset(x = (-30).dp, y = (-30).dp)
            )
        }
        if (stopwatches.isEmpty()) {
            Box(
                modifier = Modifier
                    .fillMaxSize()
                    .padding(bottom = PADDING_BOTTOM.dp)
            ) {
                CustomText(
                    text = Strings.NOTHING_HERE_YET,
                    color = Themes.ON_BACKGROUND,
                    fontSize = bigFontSize,
                    modifier = Modifier
                        .align(Alignment.Center)
                )
            }
        } else {
            Column(
                horizontalAlignment = Alignment.CenterHorizontally,
                modifier = modifier
                    .fillMaxWidth(0.9f)
                    .fillMaxHeight(0.9f)
                    .align(Alignment.Center)
            ) {
                if (isEditMode) {
                    Box(
                        contentAlignment = Alignment.Center,
                        modifier = modifier
                            .fillMaxWidth()
                            .clip(RoundedCornerShape(adaptiveWidth(16).dp))
                            .background(color = Themes.ADD_CONFIRM_COLOR)
                            .clickable {
                                viewModel.addStopwatch()
                            }
                    ) {
                        CustomText(
                            text = Strings.ADD_STOPWATCH,
                            color = Themes.ON_ADD_CONFIRM,
                            modifier = Modifier.padding(adaptiveWidth(16).dp)
                        )
                    }
                    Spacer(modifier = Modifier.height(adaptiveWidth(28).dp))
                }
                Column(
                    horizontalAlignment = Alignment.CenterHorizontally,
                    modifier = Modifier
                        .fillMaxSize()
                        .verticalScroll(scrollState)
                ) {
                    for (stopwatch in stopwatches) {
                        Stopwatch(
                            stopwatchState = stopwatch,
                            viewModel = viewModel,
                            navController = navController
                        )
                        Spacer(Modifier.height(24.dp))
                    }
                    Spacer(modifier = Modifier.height(100.dp))
                }
            }
        }

    }
}