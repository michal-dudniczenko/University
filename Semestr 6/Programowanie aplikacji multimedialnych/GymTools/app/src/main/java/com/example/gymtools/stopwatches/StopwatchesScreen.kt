package com.example.gymtools.stopwatches

import androidx.compose.foundation.layout.Arrangement
import androidx.compose.foundation.layout.Column
import androidx.compose.foundation.layout.Spacer
import androidx.compose.foundation.layout.fillMaxSize
import androidx.compose.foundation.layout.height
import androidx.compose.runtime.Composable
import androidx.compose.ui.Alignment
import androidx.compose.ui.Modifier
import androidx.compose.ui.unit.dp
import com.example.gymtools.common.adaptiveHeight

@Composable
fun StopwatchesScreen(
    viewModel: StopwatchesViewModel,
    modifier: Modifier = Modifier
) {
    Column(
        horizontalAlignment = Alignment.CenterHorizontally,
        verticalArrangement = Arrangement.Center,
        modifier = modifier
            .fillMaxSize()
    ) {
        Stopwatch(
            stopwatchId = 1,
            viewModel.time1,
            viewModel.isRunning1,
            viewModel::startPause1,
            viewModel::reset1
        )
        Spacer(Modifier.height(adaptiveHeight(120).dp))
        Stopwatch(
            stopwatchId = 2,
            viewModel.time2,
            viewModel.isRunning2,
            viewModel::startPause2,
            viewModel::reset2
        )
    }
}