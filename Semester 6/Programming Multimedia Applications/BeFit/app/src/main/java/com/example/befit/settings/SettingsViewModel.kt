package com.example.befit.settings

import androidx.compose.runtime.State
import androidx.compose.runtime.mutableStateOf
import androidx.lifecycle.ViewModel
import androidx.lifecycle.viewModelScope
import com.example.befit.constants.AppThemes
import com.example.befit.constants.Languages
import com.example.befit.constants.SettingsRoutes
import com.example.befit.constants.appLanguage
import com.example.befit.constants.appTheme
import com.example.befit.database.AppSettings
import com.example.befit.database.AppSettingsDao
import kotlinx.coroutines.launch

class SettingsViewModel(
    val appSettingsDao: AppSettingsDao
) : ViewModel() {

    private val _currentRoute = mutableStateOf(SettingsRoutes.START)
    val currentRoute: State<String> = _currentRoute

    fun updateCurrentRoute(route: String) {
        if (route != _currentRoute.value) {
            _currentRoute.value = route
        }
    }

    private val _appSettings = mutableStateOf<AppSettings?>(null)
    val appSettings: State<AppSettings?> = _appSettings

    init {
        viewModelScope.launch {
            val data = appSettingsDao.getAll()
            if (!data.isEmpty()) {
                updateAppSettings(data.first())
            } else {
                val newAppSettings = AppSettings(
                    language = Languages.DEFAULT,
                    theme = AppThemes.DARK,
                    playVideosMuted = true
                )
                appSettingsDao.insert(newAppSettings)
                updateAppSettings(newAppSettings)
            }
        }
    }

    fun updateAppSettings(
        language: String? = null,
        theme: String? = null,
        playVideosMuted: Boolean? = null,
    ) {
        val newAppSettings = AppSettings(
            id = 1,
            language = language ?: appSettings.value?.language ?: Languages.DEFAULT,
            theme = theme ?: appSettings.value?.theme ?: AppThemes.DARK,
            playVideosMuted = playVideosMuted ?: appSettings.value?.playVideosMuted ?: true,
        )

        updateAppSettings(newAppSettings)

        viewModelScope.launch {
            appSettingsDao.update(newAppSettings)
        }

    }

    fun updateAppSettings(
        appSettings: AppSettings
    ) {
        val oldAppSettings = _appSettings.value

        _appSettings.value = appSettings

        if (oldAppSettings?.language != appSettings.language) {
            appLanguage = appSettings.language
        }

        if (oldAppSettings?.theme != appSettings.theme) {
            appTheme = appSettings.theme
        }

    }
}