package com.example.befit.stopwatches

import androidx.compose.runtime.MutableLongState
import androidx.compose.runtime.MutableState
import androidx.compose.runtime.mutableLongStateOf
import androidx.compose.runtime.mutableStateOf

class StopwatchState(
    val id: Int,
    var name: String,
    var isRunning: MutableState<Boolean> = mutableStateOf(false),
    var time: MutableLongState = mutableLongStateOf(0L),
    var lastChangeTime: Long = 0L
) {
    fun startPause() {
        if (isRunning.value) {
            isRunning.value = false
        } else {
            isRunning.value = true
            lastChangeTime = System.currentTimeMillis()
        }
    }

    fun reset() {
        isRunning.value = false
        time.longValue = 0L
    }
}