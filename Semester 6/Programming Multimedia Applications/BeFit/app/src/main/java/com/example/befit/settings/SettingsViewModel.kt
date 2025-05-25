package com.example.befit.settings

import androidx.compose.runtime.State
import androidx.compose.runtime.mutableStateOf
import androidx.lifecycle.ViewModel
import com.example.befit.constants.SettingsRoutes

class SettingsViewModel : ViewModel() {

    private val _currentRoute = mutableStateOf(SettingsRoutes.START)
    val currentRoute: State<String> = _currentRoute

    fun updateCurrentRoute(route: String) {
        if (route != _currentRoute.value) {
            _currentRoute.value = route
        }
    }
}