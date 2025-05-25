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
import com.example.befit.R
import com.example.befit.common.CustomFloatingButton
import com.example.befit.common.CustomText
import com.example.befit.common.adaptiveHeight
import com.example.befit.common.adaptiveWidth
import com.example.befit.common.bigFontSize
import com.example.befit.common.bright
import com.example.befit.common.editColor
import com.example.befit.common.mediumGreen

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
                icon = R.drawable.add,
                description = "Add button",
                onClick = { viewModel.addStopwatch() },
                modifier = Modifier
                    .align(Alignment.BottomEnd)
                    .offset(x = adaptiveWidth(-32).dp, y = adaptiveWidth(-32).dp)
            )
        } else {
            CustomFloatingButton(
                icon = if (isEditMode) R.drawable.edit_white else R.drawable.edit,
                description = "Edit mode",
                color = if (isEditMode) editColor else bright,
                onClick = { isEditMode = !isEditMode },
                modifier = Modifier
                    .align(Alignment.BottomEnd)
                    .offset(x = adaptiveWidth(-32).dp, y = adaptiveWidth(-32).dp)
            )
        }
        if (stopwatches.isEmpty()) {
            Box(
                modifier = Modifier
                    .fillMaxSize()
            ) {
                CustomText(
                    text = "Nothing here yet!",
                    fontSize = bigFontSize,
                    modifier = Modifier
                        .align(Alignment.Center)
                        .offset(y = adaptiveHeight(-75).dp)
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
                            .background(color = mediumGreen)
                            .clickable {
                                viewModel.addStopwatch()
                            }
                    ) {
                        CustomText(
                            text = "Add stopwatch",
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
                        Spacer(Modifier.height(adaptiveHeight(24).dp))
                    }
                    Spacer(modifier = Modifier.height(adaptiveWidth(100).dp))
                }
            }
        }

    }
}