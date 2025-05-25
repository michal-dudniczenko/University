package com.example.befit.health.weighthistory

import androidx.compose.runtime.State
import androidx.compose.runtime.mutableStateOf
import androidx.lifecycle.ViewModel
import androidx.lifecycle.viewModelScope
import com.example.befit.common.CalorieCalculatorRoutes
import com.example.befit.common.WeightHistoryRoutes
import com.example.befit.database.Weight
import com.example.befit.database.WeightDao
import kotlinx.coroutines.flow.MutableSharedFlow
import kotlinx.coroutines.flow.asSharedFlow
import kotlinx.coroutines.launch

class WeightHistoryViewModel(
    private val weightDao: WeightDao
) : ViewModel() {

    private val _navigationEvent = MutableSharedFlow<String>()
    val navigationEvent = _navigationEvent.asSharedFlow()

    fun navigateToStart() {
        viewModelScope.launch {
            _navigationEvent.emit(WeightHistoryRoutes.START)
        }
    }

    private val _currentRoute = mutableStateOf(WeightHistoryRoutes.START)
    val currentRoute: State<String> = _currentRoute

    fun updateCurrentRoute(route: String) {
        if (route != _currentRoute.value) {
            _currentRoute.value = route
        }
    }

    private val _weights = mutableStateOf<List<Weight>>(emptyList())
    val weights: State<List<Weight>> = _weights

    init {
        viewModelScope.launch {
            refreshWeights()
        }
    }

    private suspend fun refreshWeights() {
        _weights.value = weightDao.getAll().sortedByDescending { it.date }
    }

    fun addWeight(date: Long, weight: Float) {
        if (date > 0 && weight > 0) {
            val truncateWeight = (weight * 10).toInt() / 10f
            viewModelScope.launch {
                weightDao.insert(Weight(date = date, weight = truncateWeight))
                refreshWeights()
            }
        }
    }

    fun updateWeight(id: Int, date: Long, weight: Float) {
        if (date > 0 && weight > 0) {
            val truncate = (weight * 10).toInt() / 10f
            viewModelScope.launch {
                weightDao.update(Weight(id = id, date = date, weight = truncate))
                refreshWeights()
            }
        }
    }

    fun deleteWeight(id: Int) {
        viewModelScope.launch {
            weightDao.delete(Weight(id = id))
            refreshWeights()
        }
    }

    fun getWeightById(id: Int): Weight? {
        return _weights.value.find { it.id == id }
    }
}
