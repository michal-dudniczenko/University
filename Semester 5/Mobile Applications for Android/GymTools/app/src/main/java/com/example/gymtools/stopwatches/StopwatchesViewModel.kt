package com.example.gymtools.stopwatches

import android.util.Log
import androidx.compose.runtime.mutableLongStateOf
import androidx.compose.runtime.mutableStateOf
import androidx.lifecycle.ViewModel
import androidx.lifecycle.viewModelScope
import kotlinx.coroutines.delay
import kotlinx.coroutines.launch

class StopwatchesViewModel : ViewModel() {

    var time1 = mutableLongStateOf(0L)
    var time2 = mutableLongStateOf(0L)

    var isRunning1 = mutableStateOf(false)
    var isRunning2 = mutableStateOf(false)

    private var lastChangeTime1 = 0L
    private var lastChangeTime2 = 0L

    fun startPause1() {
        if (isRunning1.value) {
            isRunning1.value = false
        } else {
            isRunning1.value = true
            lastChangeTime1 = System.currentTimeMillis()
        }
    }

    fun startPause2() {
        if (isRunning2.value) {
            isRunning2.value = false
        } else {
            isRunning2.value = true
            lastChangeTime2 = System.currentTimeMillis()
        }
    }

    fun reset1() {
        isRunning1.value = false
        time1.longValue = 0L
    }

    fun reset2() {
        isRunning2.value = false
        time2.longValue = 0L
    }

    init {
        viewModelScope.launch {
            while (true) {
                val currentTime = System.currentTimeMillis()
                if (isRunning1.value && currentTime - lastChangeTime1 >= 1000L) {
                    time1.longValue += (currentTime - lastChangeTime1) / 1000L
                    lastChangeTime1 = currentTime
                }
                if (isRunning2.value && currentTime - lastChangeTime2 >= 1000L) {
                    time2.longValue += (currentTime - lastChangeTime2) / 1000L
                    lastChangeTime2 = currentTime
                }
                delay(50L)
            }
        }
    }
}