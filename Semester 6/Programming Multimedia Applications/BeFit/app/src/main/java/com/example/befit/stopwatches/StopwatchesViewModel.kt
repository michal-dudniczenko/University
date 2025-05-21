package com.example.befit.stopwatches

import androidx.compose.foundation.ScrollState
import androidx.compose.runtime.mutableLongStateOf
import androidx.compose.runtime.mutableStateOf
import androidx.lifecycle.ViewModel
import androidx.lifecycle.viewModelScope
import com.example.befit.database.Program
import com.example.befit.database.Stopwatch
import com.example.befit.database.StopwatchDao
import kotlinx.coroutines.delay
import kotlinx.coroutines.flow.MutableStateFlow
import kotlinx.coroutines.flow.StateFlow
import kotlinx.coroutines.launch

class StopwatchesViewModel(
    private val stopwatchDao: StopwatchDao
) : ViewModel() {

    val isEditMode = mutableStateOf(false)

    val scrollState = mutableStateOf(ScrollState(0))

    fun resetScroll() {
        viewModelScope.launch {
            scrollState.value.scrollTo(0)
        }
    }

    private val _stopwatches = MutableStateFlow<List<StopwatchState>>(emptyList())
    val stopwatches: StateFlow<List<StopwatchState>> = _stopwatches

    init {
        viewModelScope.launch {
            refreshStopwatches()
            if (_stopwatches.value.isEmpty()) {
                stopwatchDao.insert(Stopwatch(name = "Stopwatch 1"))
                stopwatchDao.insert(Stopwatch(name = "Stopwatch 2"))
                refreshStopwatches()
            }
            while (true) {
                val currentTime = System.currentTimeMillis()
                for (stopwatch in _stopwatches.value) {
                    val stopwatchLastChangeTime = stopwatch.lastChangeTime
                    if (stopwatch.isRunning.value && currentTime - stopwatchLastChangeTime >= 1000L) {
                        stopwatch.time.longValue += (currentTime - stopwatchLastChangeTime) / 1000L
                        stopwatch.lastChangeTime = currentTime
                    }
                }
                delay(50L)
            }
        }
    }

    private suspend fun refreshStopwatches() {
        val stopwatchesFromDb = stopwatchDao.getAll()

        // Step 1: Create a mutable copy of the current state
        val updatedList = _stopwatches.value.toMutableList()

        // Step 2: Add or update items from DB
        for (stopwatch in stopwatchesFromDb) {
            val index = updatedList.indexOfFirst { it.id == stopwatch.id }
            if (index == -1) {
                updatedList.add(StopwatchState(id = stopwatch.id, name = stopwatch.name))
            } else if (updatedList[index].name != stopwatch.name) {
                updatedList[index].name = stopwatch.name
            }
        }

        // Step 3: Remove items that are no longer in the DB
        val idsFromDb = stopwatchesFromDb.map { it.id }.toSet()
        updatedList.removeAll { it.id !in idsFromDb }

        // Step 4: Update state
        _stopwatches.value = updatedList
    }

    suspend fun addStopwatch(name: String = "Stopwatch") {
        if (name.isNotEmpty()) {
            stopwatchDao.insert(Stopwatch(name = name))
            refreshStopwatches()
        }
    }

    suspend fun updateStopwatch(id: Int, name: String) {
        if (name.isNotEmpty()) {
            stopwatchDao.update(Stopwatch(id = id, name = name))
            refreshStopwatches()
        }
    }

    suspend fun deleteStopwatch(id: Int) {
        stopwatchDao.delete(Stopwatch(id = id))
        refreshStopwatches()
    }

}