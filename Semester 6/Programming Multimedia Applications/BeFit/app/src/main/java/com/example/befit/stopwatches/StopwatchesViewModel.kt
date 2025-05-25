package com.example.befit.stopwatches

import androidx.compose.foundation.ScrollState
import androidx.compose.runtime.State
import androidx.compose.runtime.mutableStateOf
import androidx.lifecycle.ViewModel
import androidx.lifecycle.viewModelScope
import com.example.befit.constants.StopwatchesRoutes
import com.example.befit.constants.Strings
import com.example.befit.database.Stopwatch
import com.example.befit.database.StopwatchDao
import kotlinx.coroutines.delay
import kotlinx.coroutines.flow.MutableSharedFlow
import kotlinx.coroutines.flow.asSharedFlow
import kotlinx.coroutines.launch

class StopwatchesViewModel(
    private val stopwatchDao: StopwatchDao
) : ViewModel() {
    private val _navigationEvent = MutableSharedFlow<String>()
    val navigationEvent = _navigationEvent.asSharedFlow()

    fun navigateToStart() {
        viewModelScope.launch {
            _navigationEvent.emit(StopwatchesRoutes.START)
        }
    }

    private val _currentRoute = mutableStateOf(StopwatchesRoutes.START)
    val currentRoute: State<String> = _currentRoute

    fun updateCurrentRoute(route: String) {
        if (route != _currentRoute.value) {
            _currentRoute.value = route
        }
    }

    val isEditMode = mutableStateOf(false)

    val scrollState = mutableStateOf(ScrollState(0))

    private val _stopwatches = mutableStateOf<List<StopwatchState>>(emptyList())
    val stopwatches: State<List<StopwatchState>> = _stopwatches

    init {
        viewModelScope.launch {
            refreshStopwatches()
            if (_stopwatches.value.isEmpty()) {
                stopwatchDao.insert(Stopwatch(name = Strings.STOPWATCH + " 1"))
                stopwatchDao.insert(Stopwatch(name = Strings.STOPWATCH + " 2"))
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

    fun addStopwatch(name: String = Strings.STOPWATCH) {
        if (name.isNotEmpty()) {
            viewModelScope.launch {
                stopwatchDao.insert(Stopwatch(name = name))
                refreshStopwatches()
            }
        }
    }

    fun updateStopwatch(id: Int, name: String) {
        if (name.isNotEmpty()) {
            viewModelScope.launch {
                stopwatchDao.update(Stopwatch(id = id, name = name))
                refreshStopwatches()
            }
        }
    }

    fun deleteStopwatch(id: Int) {
        viewModelScope.launch {
            stopwatchDao.delete(Stopwatch(id = id))
            refreshStopwatches()
        }
    }

    fun getStopwatchName(id: Int): String? {
        return _stopwatches.value.find { it.id == id }?.name
    }

}