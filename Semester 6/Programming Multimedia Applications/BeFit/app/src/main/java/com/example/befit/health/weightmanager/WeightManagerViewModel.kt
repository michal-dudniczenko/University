package com.example.befit.health.weightmanager

import androidx.compose.runtime.mutableStateOf
import androidx.lifecycle.ViewModel
import androidx.lifecycle.viewModelScope
import com.example.befit.database.Weight
import com.example.befit.database.WeightDao
import kotlinx.coroutines.flow.MutableStateFlow
import kotlinx.coroutines.flow.StateFlow
import kotlinx.coroutines.launch

class WeightManagerViewModel(
    private val weightDao: WeightDao
) : ViewModel() {

    private val _currentRoute = mutableStateOf("Weight history")
    val currentRoute: String get() = _currentRoute.value

    fun updateCurrentRoute(route: String) {
        _currentRoute.value = route
    }

    private val _weights = MutableStateFlow<List<Weight>>(emptyList())
    val weights: StateFlow<List<Weight>> = _weights

    init {
        viewModelScope.launch {
            refreshWeights()
        }
    }

    private suspend fun refreshWeights() {
        _weights.value = weightDao.getAll().sortedByDescending { it.date }
    }

    suspend fun addWeight(date: Long, weight: Float) {
        if (date > 0 && weight > 0) {
            val truncateWeight = (weight * 10).toInt() / 10f
            weightDao.insert(Weight(date = date, weight = truncateWeight))
            refreshWeights()
        }
    }

    suspend fun updateWeight(id: Int, date: Long, weight: Float) {
        if (date > 0 && weight > 0) {
            val truncate = (weight * 10).toInt() / 10f
            weightDao.update(Weight(id = id, date = date, weight = truncate))
            refreshWeights()
        }
    }

    suspend fun deleteWeight(id: Int) {
        weightDao.delete(Weight(id = id))
        refreshWeights()
    }

    fun getWeightById(id: Int): Weight? {
        return _weights.value.find { it.id == id }
    }
}
